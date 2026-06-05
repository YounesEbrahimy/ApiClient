using System.Collections.Concurrent;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using System.Threading;
using Newtonsoft.Json;
using UnityEngine;
using System.Text;
using System.IO;
using System;

namespace ApiClientLib
{
    public class ApiClient : IApiClient
    {
        public Dictionary<string, string> Headers => _persistentHeaders;
        private readonly Dictionary<string, string> _persistentHeaders = new();

        internal readonly string _cacheDir;
        internal readonly string _cacheIndexPath;

        private Dictionary<string, CacheEntry> _cacheIndex;
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _fileLocks = new();
        private readonly SemaphoreSlim _globalCacheLock = new(1, 1);

        public string BaseUrl => _baseUrl;
        private string _baseUrl = string.Empty;

        // ── Constructors ──────────────────────────────────────────────────────────

        public ApiClient()
        {
            _cacheDir = Path.Combine(Application.persistentDataPath, "api_client_cache");
            _cacheIndexPath = Path.Combine(_cacheDir, "index.json");
            Directory.CreateDirectory(_cacheDir);
        }

        public ApiClient(string baseUrl) : this()
        {
            SetBaseUrl(baseUrl);
        }

        // ── Base Url ──────────────────────────────────────────────────────────────

        public void SetBaseUrl(string baseUrl)
        {
            if (baseUrl == null)
            {
                throw new ArgumentNullException(nameof(baseUrl));
            }

            if (baseUrl.Length == 0)
            {
                _baseUrl = string.Empty;
            }
            else
            {
                _baseUrl = baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/";
            }
        }

        // ── Persistent Headers ────────────────────────────────────────────────────

        public void AddHeader(string key, string value) => _persistentHeaders[key] = value;
        public void RemoveHeader(string key) => _persistentHeaders.Remove(key);
        public void ClearHeaders() => _persistentHeaders.Clear();

        // ── GET Methods ───────────────────────────────────────────────────────────

        public async UniTask GetAsync(string url, Dictionary<string, string> headers = null, int timeout = 10,
            CancellationToken ct = default)
        {
            using var req =
                await SendJsonWebRequestAsync(url, UnityWebRequest.kHttpVerbGET, null, headers, timeout, ct);
        }

        public async UniTask<T> GetAsync<T>(string url, Dictionary<string, string> headers = null, int timeout = 10,
            CancellationToken ct = default)
        {
            using var req =
                await SendJsonWebRequestAsync(url, UnityWebRequest.kHttpVerbGET, null, headers, timeout, ct);
            return ProcessResponse<T>(req);
        }

        // ── POST Methods ──────────────────────────────────────────────────────────

        public async UniTask PostAsync(string url, object body, Dictionary<string, string> headers = null,
            int timeout = 10, CancellationToken ct = default)
        {
            using var req =
                await SendJsonWebRequestAsync(url, UnityWebRequest.kHttpVerbPOST, body, headers, timeout, ct);
        }

        public async UniTask<T> PostAsync<T>(string url, object body, Dictionary<string, string> headers = null,
            int timeout = 10, CancellationToken ct = default)
        {
            using var req =
                await SendJsonWebRequestAsync(url, UnityWebRequest.kHttpVerbPOST, body, headers, timeout, ct);
            return ProcessResponse<T>(req);
        }

        // ── PUT Methods ───────────────────────────────────────────────────────────

        public async UniTask PutAsync(string url, object body, Dictionary<string, string> headers = null,
            int timeout = 10, CancellationToken ct = default)
        {
            using var req =
                await SendJsonWebRequestAsync(url, UnityWebRequest.kHttpVerbPUT, body, headers, timeout, ct);
        }

        public async UniTask<T> PutAsync<T>(string url, object body, Dictionary<string, string> headers = null,
            int timeout = 10, CancellationToken ct = default)
        {
            using var req =
                await SendJsonWebRequestAsync(url, UnityWebRequest.kHttpVerbPUT, body, headers, timeout, ct);
            return ProcessResponse<T>(req);
        }

        // ── PATCH Methods ─────────────────────────────────────────────────────────

        public async UniTask PatchAsync(string url, object body, Dictionary<string, string> headers = null,
            int timeout = 10, CancellationToken ct = default)
        {
            using var req = await SendJsonWebRequestAsync(url, "PATCH", body, headers, timeout, ct);
        }

        public async UniTask<T> PatchAsync<T>(string url, object body, Dictionary<string, string> headers = null,
            int timeout = 10, CancellationToken ct = default)
        {
            using var req = await SendJsonWebRequestAsync(url, "PATCH", body, headers, timeout, ct);
            return ProcessResponse<T>(req);
        }

        // ── DELETE Methods ────────────────────────────────────────────────────────

        public async UniTask DeleteAsync(string url, Dictionary<string, string> headers = null, int timeout = 10,
            CancellationToken ct = default)
        {
            using var req =
                await SendJsonWebRequestAsync(url, UnityWebRequest.kHttpVerbDELETE, null, headers, timeout, ct);
        }

        public async UniTask<T> DeleteAsync<T>(string url, Dictionary<string, string> headers = null, int timeout = 10,
            CancellationToken ct = default)
        {
            using var req =
                await SendJsonWebRequestAsync(url, UnityWebRequest.kHttpVerbDELETE, null, headers, timeout, ct);
            return ProcessResponse<T>(req);
        }

        // ── Sprite Methods ────────────────────────────────────────────────────────

        public async UniTask<Sprite> GetSpriteAsync(string url, Dictionary<string, string> headers = null,
            int timeout = 10, CancellationToken ct = default)
        {
            using var req = UnityWebRequestTexture.GetTexture(url);
            ApplyHeadersAndTimeout(req, _persistentHeaders, headers, timeout);

            try
            {
                await req.SendWebRequest().ToUniTask(cancellationToken: ct);
            }
            catch (UnityWebRequestException e)
            {
                e.UnityWebRequest.ThrowIfTimeout();
                if (req.responseCode is >= 200 and < 300)
                {
                    if (IsBodyLessResponse(req.responseCode))
                        throw new ApiException((int)req.responseCode, req.downloadHandler?.error);
                    else
                        throw new BadSpriteException(e);
                }
                else
                {
                    throw new ApiException((int)req.responseCode, req.downloadHandler?.error);
                }
            }

            var tex = DownloadHandlerTexture.GetContent(req);
            try
            {
                return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            }
            catch (Exception e)
            {
                throw new BadSpriteException(e);
            }
        }

        public UniTask<Sprite> GetCachedSpriteAsync(string url, int cacheDays = 14,
            Dictionary<string, string> headers = null, int timeout = 10, CancellationToken ct = default)
        {
            Sprite downloadedSprite = null;

            return GetCachedAssetAsync(
                url,
                fileExtension: "png",
                downloadToBytes: async token =>
                {
                    downloadedSprite = await GetSpriteAsync(url, headers, timeout, token);
                    return downloadedSprite.texture.EncodeToPNG();
                },
                deserializeFromPath: async (path, token) =>
                {
                    if (downloadedSprite != null) return downloadedSprite;

                    var bytes = await UniTask.RunOnThreadPool(
                        () => File.ReadAllBytes(path), cancellationToken: token);
                    return BytesToSprite(bytes);
                },
                cacheDays, ct);
        }

        // ── AudioClip Methods ─────────────────────────────────────────────────────

        public async UniTask<AudioClip> GetAudioClipAsync(string url, AudioType audioType = AudioType.UNKNOWN,
            Dictionary<string, string> headers = null, int timeout = 10, CancellationToken ct = default)
        {
            var resolvedType = audioType == AudioType.UNKNOWN ? DetectAudioType(url) : audioType;
            using var req = UnityWebRequestMultimedia.GetAudioClip(url, resolvedType);
            ApplyHeadersAndTimeout(req, _persistentHeaders, headers, timeout);

            try
            {
                await req.SendWebRequest().ToUniTask(cancellationToken: ct);
            }
            catch (UnityWebRequestException e)
            {
                e.UnityWebRequest.ThrowIfTimeout();
                if (req.responseCode is >= 200 and < 300)
                {
                    if (IsBodyLessResponse(req.responseCode))
                        throw new ApiException((int)req.responseCode, req.downloadHandler?.error);
                    else
                        throw new BadAudioClipException(e);
                }
                else
                {
                    throw new ApiException((int)req.responseCode, req.downloadHandler?.error);
                }
            }

            if (IsBodyLessResponse(req.responseCode))
                throw new ApiException((int)req.responseCode, req.downloadHandler?.error);

            try
            {
                var clip = DownloadHandlerAudioClip.GetContent(req);
                if (req.result == UnityWebRequest.Result.DataProcessingError || clip == null ||
                    clip.loadState == AudioDataLoadState.Failed)
                    throw new Exception("Invalid audio data received.");

                return clip;
            }
            catch (Exception e)
            {
                throw new BadAudioClipException(e);
            }
        }

        public UniTask<AudioClip> GetCachedAudioClipAsync(string url, AudioType audioType = AudioType.UNKNOWN,
            int cacheDays = 14, Dictionary<string, string> headers = null, int timeout = 10,
            CancellationToken ct = default)
        {
            var ext = GetUrlFileExtension(url);
            if (ext == null)
                throw new InvalidUrlException(url, null, null,
                    "When Requesting for an AudioClip url, url must contain file extension.");

            var resolvedType = audioType == AudioType.UNKNOWN ? DetectAudioType(url) : audioType;

            return GetCachedAssetAsync(
                url,
                fileExtension: ext,
                downloadToBytes: async token =>
                {
                    using var req = UnityWebRequest.Get(url);
                    ApplyHeadersAndTimeout(req, _persistentHeaders, headers, timeout);
                    try
                    {
                        await req.SendWebRequest().ToUniTask(cancellationToken: token);
                    }
                    catch (UnityWebRequestException e)
                    {
                        e.UnityWebRequest.ThrowIfTimeout();
                        throw new ApiException((int)req.responseCode, req.downloadHandler?.error);
                    }

                    return req.downloadHandler.data;
                },
                deserializeFromPath: (path, token) =>
                    LoadAudioClipFromPathAsync(path, resolvedType, token),
                cacheDays, ct);
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        internal string CombineUrl(string relativeUrl)
        {
            return ValidatedUrl(_baseUrl, relativeUrl);
        }

        internal static string ValidatedUrl(string baseUrl, string relativeUrl)
        {
            relativeUrl ??= string.Empty;
            var mergedUrl = baseUrl + relativeUrl;
            var isValid = Uri.TryCreate(mergedUrl, UriKind.Absolute, out var uriResult) &&
                          (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return isValid
                ? mergedUrl
                : throw new InvalidUrlException(mergedUrl, baseUrl, relativeUrl,
                    $"Invalid URL: '{mergedUrl}'. Must be an absolute HTTP or HTTPS URL.");
        }

        private async UniTask<UnityWebRequest> SendJsonWebRequestAsync(string url, string method, object body,
            Dictionary<string, string> headers = null, int timeout = 10, CancellationToken ct = default)
        {
            var req = CreateJsonWebRequest(CombineUrl(url), body, method);
            ApplyHeadersAndTimeout(req, _persistentHeaders, headers, timeout);

            try
            {
                await req.SendWebRequest().ToUniTask(cancellationToken: ct);
            }
            catch (UnityWebRequestException e)
            {
                e.UnityWebRequest.ThrowIfTimeout();
                throw new ApiException((int)e.ResponseCode, e.UnityWebRequest.downloadHandler?.error);
            }

            return req;
        }

        private static UnityWebRequest CreateJsonWebRequest(string url, object body, string method)
        {
            var req = new UnityWebRequest(url, method);
            if (body != null)
            {
                string json;
                try
                {
                    json = JsonConvert.SerializeObject(body);
                }
                catch (Exception e)
                {
                    throw new JsonException(e);
                }

                var bodyRaw = Encoding.UTF8.GetBytes(json);
                req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }

            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            return req;
        }

        private static void ApplyHeadersAndTimeout(UnityWebRequest req, Dictionary<string, string> persistentHeaders,
            Dictionary<string, string> customHeaders, int timeout)
        {
            foreach (var kvp in persistentHeaders) req.SetRequestHeader(kvp.Key, kvp.Value);
            if (customHeaders != null)
            {
                foreach (var kvp in customHeaders) req.SetRequestHeader(kvp.Key, kvp.Value);
            }

            req.timeout = timeout;
        }

        private T ProcessResponse<T>(UnityWebRequest req)
        {
            if (IsBodyLessResponse(req.responseCode))
                throw new ApiException((int)req.responseCode, "Expected response but none was provided by the server");

            var text = req.downloadHandler?.text;

            if (string.IsNullOrEmpty(text))
                throw new JsonException(new NullReferenceException("Server response was null"));

            if (typeof(T) == typeof(string)) return (T)(object)text;
            try
            {
                return JsonConvert.DeserializeObject<T>(text);
            }
            catch (Exception e)
            {
                throw new JsonException(e);
            }
        }

        internal static bool IsBodyLessResponse(long statusCode)
        {
            return statusCode is 204 or 205 or 304 or >= 100 and < 200;
        }

        private async UniTask EnsureCacheIndexLoadedAsync()
        {
            if (_cacheIndex != null) return;

            await _globalCacheLock.WaitAsync();
            try
            {
                if (_cacheIndex != null) return;

                if (File.Exists(_cacheIndexPath))
                {
                    var json = await UniTask.RunOnThreadPool(() => File.ReadAllText(_cacheIndexPath));
                    try
                    {
                        _cacheIndex = JsonConvert.DeserializeObject<Dictionary<string, CacheEntry>>(json);
                    }
                    catch (Exception e)
                    {
                        _cacheIndex = new();
                        try
                        {
                            File.Delete(_cacheIndexPath);
                        }
                        catch (Exception ex)
                        {
                            // ignored, since client can't do anything about it.
                        }
                    }
                }
                else
                {
                    _cacheIndex = new();
                }
            }
            catch (Exception e)
            {
                _cacheIndex = new();
            }
            finally
            {
                _globalCacheLock.Release();
            }
        }

        private async UniTask<T> GetCachedAssetAsync<T>(string url, string fileExtension,
            Func<CancellationToken, UniTask<byte[]>> downloadToBytes,
            Func<string, CancellationToken, UniTask<T>> deserializeFromPath, int cacheDays, CancellationToken ct)
        {
            var key = ComputeHash(url);
            var filePath = Path.Combine(_cacheDir, $"{key}.{fileExtension}");

            await EnsureCacheIndexLoadedAsync();

            var fileLock = _fileLocks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
            await fileLock.WaitAsync(ct);

            try
            {
                bool isLocalCacheValid;
                await _globalCacheLock.WaitAsync(ct);
                try
                {
                    isLocalCacheValid = File.Exists(filePath) && IsCacheValidInternal(key, cacheDays);
                }
                finally
                {
                    _globalCacheLock.Release();
                }

                if (isLocalCacheValid)
                    return await deserializeFromPath(filePath, ct);

                var bytes = await downloadToBytes(ct);
                ct.ThrowIfCancellationRequested();
                await SaveAssetToDiskAsync(bytes, filePath, key, url);

                return await deserializeFromPath(filePath, ct);
            }
            finally
            {
                fileLock.Release();
            }
        }

        private bool IsCacheValidInternal(string key, int cacheDays) =>
            _cacheIndex.TryGetValue(key, out var entry) && DateTime.UtcNow < entry.CachedAt.AddDays(cacheDays);

        private async UniTask SaveAssetToDiskAsync(byte[] bytes, string filePath, string key, string url)
        {
            if (!Directory.Exists(_cacheDir))
                Directory.CreateDirectory(_cacheDir);

            await _globalCacheLock.WaitAsync();
            try
            {
                _cacheIndex[key] = new CacheEntry { Url = url, CachedAt = DateTime.UtcNow };
                var indexJson = JsonConvert.SerializeObject(_cacheIndex);

                await UniTask.RunOnThreadPool(() =>
                {
                    File.WriteAllBytes(filePath, bytes);
                    File.WriteAllText(_cacheIndexPath, indexJson);
                });
            }
            finally
            {
                _globalCacheLock.Release();
            }
        }

        private static Sprite BytesToSprite(byte[] bytes)
        {
            var tex = new Texture2D(2, 2);
            tex.LoadImage(bytes);
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }

        internal static string ComputeHash(string input)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }

        private static async UniTask<AudioClip> LoadAudioClipFromPathAsync(
            string filePath, AudioType audioType, CancellationToken ct)
        {
            using var req = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, audioType);
            ((DownloadHandlerAudioClip)req.downloadHandler).streamAudio = false;

            try
            {
                await req.SendWebRequest().ToUniTask(cancellationToken: ct);
            }
            catch (UnityWebRequestException e)
            {
                throw new BadAudioClipException(e);
            }

            try
            {
                return DownloadHandlerAudioClip.GetContent(req);
            }
            catch (Exception e)
            {
                throw new BadAudioClipException(e);
            }
        }

        private static AudioType DetectAudioType(string url) => GetUrlFileExtension(url, "audio") switch
        {
            "mp3" or "mpeg" => AudioType.MPEG,
            "ogg" => AudioType.OGGVORBIS,
            "acc" => AudioType.ACC,
            "wav" => AudioType.WAV,
            "aiff" or "aif" => AudioType.AIFF,
            _ => AudioType.UNKNOWN
        };

        private static string GetUrlFileExtension(string url, string fallback = null)
        {
            try
            {
                var ext = Path.GetExtension(url.Split('?')[0]).TrimStart('.');
                return string.IsNullOrWhiteSpace(ext) ? fallback : ext.ToLowerInvariant();
            }
            catch
            {
                return fallback;
            }
        }
    }
}