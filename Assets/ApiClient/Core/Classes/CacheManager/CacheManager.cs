using System.Collections.Concurrent;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;
using System;

namespace ApiClientLib.SubClasses
{
    internal sealed class CacheManager : ICacheManager
    {
        internal readonly string _cacheDir;
        internal readonly string _cacheIndexPath;

        private Dictionary<string, CacheEntry> _cacheIndex;
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _fileLocks = new();
        private readonly SemaphoreSlim _globalLock = new(1, 1);

        private const int CacheStatusCode = -1;

        internal CacheManager()
        {
            _cacheDir = Path.Combine(Application.persistentDataPath, "api_client_cache");
            _cacheIndexPath = Path.Combine(_cacheDir, "index.json");
            Directory.CreateDirectory(_cacheDir);
        }

        public async UniTask InvalidateAsync(CancellationToken ct)
        {
            var heldLocks = new List<SemaphoreSlim>();
            try
            {
                bool acquiredNew;
                do
                {
                    acquiredNew = false;
                    foreach (var fileLock in _fileLocks.Values)
                    {
                        if (heldLocks.Contains(fileLock)) continue;
                        await fileLock.WaitAsync(ct);
                        heldLocks.Add(fileLock);
                        acquiredNew = true;
                    }
                } while (acquiredNew);

                await _globalLock.WaitAsync(ct);
                try
                {
                    await UniTask.RunOnThreadPool(() =>
                    {
                        if (Directory.Exists(_cacheDir)) Directory.Delete(_cacheDir, true);
                        Directory.CreateDirectory(_cacheDir);
                    }, cancellationToken: ct);

                    _cacheIndex = new Dictionary<string, CacheEntry>();
                }
                finally
                {
                    _globalLock.Release();
                }
            }
            finally
            {
                foreach (var fileLock in heldLocks) fileLock.Release();
            }
        }

        public async UniTask<(T asset, bool wasCached, int statusCode)> GetOrUpdateAsync<T>(string key, string fileExt,
            string url, int cacheDays, Func<CancellationToken, UniTask<(int statusCode, byte[] bytes)>> downloadAsync,
            Func<string, CancellationToken, UniTask<T>> deserializeAsync, CancellationToken ct)
        {
            await EnsureIndexLoadedAsync();
            var filePath = Path.Combine(_cacheDir, $"{key}.{fileExt}");
            var fileLock = _fileLocks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));

            await fileLock.WaitAsync(ct);
            try
            {
                var isValid = false;
                await _globalLock.WaitAsync(ct);
                try
                {
                    isValid = File.Exists(filePath) && IsValidInternal(key, cacheDays);
                }
                finally
                {
                    _globalLock.Release();
                }

                if (isValid)
                {
                    return (await deserializeAsync(filePath, ct), true, CacheStatusCode);
                }

                var downloadResult = await downloadAsync(ct);
                ct.ThrowIfCancellationRequested();
                await SaveToDiskAsync(downloadResult.bytes, filePath, key, url);

                return (await deserializeAsync(filePath, ct), false, downloadResult.statusCode);
            }
            finally
            {
                fileLock.Release();
            }
        }

        private async UniTask EnsureIndexLoadedAsync()
        {
            if (_cacheIndex != null) return;

            await _globalLock.WaitAsync();
            try
            {
                if (_cacheIndex != null) return;
                if (File.Exists(_cacheIndexPath))
                {
                    var json = await UniTask.RunOnThreadPool(() => File.ReadAllText(_cacheIndexPath));
                    try
                    {
                        _cacheIndex = JsonConvert.DeserializeObject<Dictionary<string, CacheEntry>>(json) ?? new();
                    }
                    catch
                    {
                        ResetCorruptedIndex();
                    }
                }
                else
                {
                    _cacheIndex = new();
                }
            }
            catch
            {
                _cacheIndex = new();
            }
            finally
            {
                _globalLock.Release();
            }
        }

        private void ResetCorruptedIndex()
        {
            _cacheIndex = new();
            try
            {
                File.Delete(_cacheIndexPath);
            }
            catch
            {
                // Ignored, Can't do anything about it
            }
        }

        private bool IsValidInternal(string key, int days) =>
            _cacheIndex.TryGetValue(key, out var e) && DateTime.UtcNow < e.CachedAt.AddDays(days);

        private async UniTask SaveToDiskAsync(byte[] bytes, string path, string key, string url)
        {
            if (!Directory.Exists(_cacheDir)) Directory.CreateDirectory(_cacheDir);

            await _globalLock.WaitAsync();
            try
            {
                _cacheIndex[key] = new CacheEntry { Url = url, CachedAt = DateTime.UtcNow };
                var json = JsonConvert.SerializeObject(_cacheIndex);

                await UniTask.RunOnThreadPool(() =>
                {
                    File.WriteAllBytes(path, bytes);
                    File.WriteAllText(_cacheIndexPath, json);
                });
            }
            finally
            {
                _globalLock.Release();
            }
        }
    }
}