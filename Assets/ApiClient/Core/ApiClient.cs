using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ApiClientLib.SubClasses;
using UnityEngine.Networking;
using ApiClientLib.Helpers;
using System.Threading;
using UnityEngine;
using System.IO;
using System;

namespace ApiClientLib
{
    public sealed class ApiClient : IApiClient
    {
        // ── Constructors ──────────────────────────────────────────────────────────

        public ApiClient()
        {
        }

        public ApiClient(string baseUrl) : this()
        {
            SetBaseUrl(baseUrl);
        }

        // ── Instance ID ───────────────────────────────────────────────────────────

        private static int _nextId = -1;
        public int InstanceID { get; } = Interlocked.Increment(ref _nextId);

        // ── Events ────────────────────────────────────────────────────────────────

        public event Action<ApiEventData> OnRequestCompleted;
        public event Action<int> OnRequestStatusCodeResolved;

        // ── Base Url ──────────────────────────────────────────────────────────────

        private readonly IBaseUrlManager _baseUrlManager = new BaseUrlManager();
        public string BaseUrl => _baseUrlManager.BaseUrl;
        public void SetBaseUrl(string baseUrl) => _baseUrlManager.SetBaseUrl(baseUrl);

        // ── Persistent Headers ────────────────────────────────────────────────────

        private readonly IHeaderManager _headerManager = new HeaderManager();
        public IReadOnlyDictionary<string, string> Headers => _headerManager.Headers;
        public void AddHeader(string key, string value) => _headerManager.AddHeader(key, value);
        public void RemoveHeader(string key) => _headerManager.RemoveHeader(key);
        public void ClearHeaders() => _headerManager.ClearHeaders();

        // ── Cache ─────────────────────────────────────────────────────────────────

        internal readonly ICacheManager _cacheManager = new CacheManager();

        public async UniTask InvalidateCacheAsync(CancellationToken ct = default) =>
            await _cacheManager.InvalidateAsync(ct);

        // ── GET Methods ───────────────────────────────────────────────────────────

        public async UniTask<int> GetAsync(string url, IReadOnlyDictionary<string, string> customHeaders = null,
            IReadOnlyDictionary<string, string> queryParams = null, UrlType urlType = UrlType.Relative,
            int timeout = 10, CancellationToken ct = default)
        {
            return (await JsonRequestHandling.HandleJsonWebRequestAsync<AsyncUnit>(false, OnRequestCompleted,
                OnRequestStatusCodeResolved, RequestMethod.GET,
                url, BaseUrl, null, Headers, customHeaders, queryParams, urlType, timeout, ct, InstanceID)).StatusCode;
        }

        public async UniTask<ApiResponse<T>> GetAsync<T>(string url,
            IReadOnlyDictionary<string, string> customHeaders = null,
            IReadOnlyDictionary<string, string> queryParams = null, UrlType urlType = UrlType.Relative,
            int timeout = 10, CancellationToken ct = default)
        {
            return await JsonRequestHandling.HandleJsonWebRequestAsync<T>(true, OnRequestCompleted,
                OnRequestStatusCodeResolved, RequestMethod.GET,
                url, BaseUrl, null, Headers, customHeaders, queryParams, urlType, timeout, ct, InstanceID);
        }

        // ── POST Methods ──────────────────────────────────────────────────────────

        public async UniTask<int> PostAsync(string url, object body,
            IReadOnlyDictionary<string, string> customHeaders = null,
            IReadOnlyDictionary<string, string> queryParams = null, UrlType urlType = UrlType.Relative,
            int timeout = 10, CancellationToken ct = default)
        {
            return (await JsonRequestHandling.HandleJsonWebRequestAsync<AsyncUnit>(false, OnRequestCompleted,
                OnRequestStatusCodeResolved, RequestMethod.POST, url, BaseUrl, body, Headers, customHeaders,
                queryParams, urlType, timeout, ct, InstanceID)).StatusCode;
        }

        public async UniTask<ApiResponse<T>> PostAsync<T>(string url, object body,
            IReadOnlyDictionary<string, string> customHeaders = null,
            IReadOnlyDictionary<string, string> queryParams = null, UrlType urlType = UrlType.Relative,
            int timeout = 10, CancellationToken ct = default)
        {
            return await JsonRequestHandling.HandleJsonWebRequestAsync<T>(true, OnRequestCompleted,
                OnRequestStatusCodeResolved, RequestMethod.POST,
                url, BaseUrl, body, Headers, customHeaders, queryParams, urlType, timeout, ct, InstanceID);
        }

        // ── PUT Methods ───────────────────────────────────────────────────────────

        public async UniTask<int> PutAsync(string url, object body,
            IReadOnlyDictionary<string, string> customHeaders = null,
            IReadOnlyDictionary<string, string> queryParams = null, UrlType urlType = UrlType.Relative,
            int timeout = 10, CancellationToken ct = default)
        {
            return (await JsonRequestHandling.HandleJsonWebRequestAsync<AsyncUnit>(false, OnRequestCompleted,
                OnRequestStatusCodeResolved, RequestMethod.PUT,
                url, BaseUrl, body, Headers, customHeaders, queryParams, urlType, timeout, ct, InstanceID)).StatusCode;
        }

        public async UniTask<ApiResponse<T>> PutAsync<T>(string url, object body,
            IReadOnlyDictionary<string, string> customHeaders = null,
            IReadOnlyDictionary<string, string> queryParams = null, UrlType urlType = UrlType.Relative,
            int timeout = 10, CancellationToken ct = default)
        {
            return await JsonRequestHandling.HandleJsonWebRequestAsync<T>(true, OnRequestCompleted,
                OnRequestStatusCodeResolved, RequestMethod.PUT,
                url, BaseUrl, body, Headers, customHeaders, queryParams, urlType, timeout, ct, InstanceID);
        }

        // ── PATCH Methods ─────────────────────────────────────────────────────────

        public async UniTask<int> PatchAsync(string url, object body,
            IReadOnlyDictionary<string, string> customHeaders = null,
            IReadOnlyDictionary<string, string> queryParams = null, UrlType urlType = UrlType.Relative,
            int timeout = 10, CancellationToken ct = default)
        {
            return (await JsonRequestHandling.HandleJsonWebRequestAsync<AsyncUnit>(false, OnRequestCompleted,
                OnRequestStatusCodeResolved, RequestMethod.PATCH, url, BaseUrl, body, Headers, customHeaders,
                queryParams, urlType, timeout, ct, InstanceID)).StatusCode;
        }

        public async UniTask<ApiResponse<T>> PatchAsync<T>(string url, object body,
            IReadOnlyDictionary<string, string> customHeaders = null,
            IReadOnlyDictionary<string, string> queryParams = null, UrlType urlType = UrlType.Relative,
            int timeout = 10, CancellationToken ct = default)
        {
            return await JsonRequestHandling.HandleJsonWebRequestAsync<T>(true, OnRequestCompleted,
                OnRequestStatusCodeResolved, RequestMethod.PATCH,
                url, BaseUrl, body, Headers, customHeaders, queryParams, urlType, timeout, ct, InstanceID);
        }

        // ── DELETE Methods ────────────────────────────────────────────────────────

        public async UniTask<int> DeleteAsync(string url, IReadOnlyDictionary<string, string> customHeaders = null,
            IReadOnlyDictionary<string, string> queryParams = null, UrlType urlType = UrlType.Relative,
            int timeout = 10, CancellationToken ct = default)
        {
            return (await JsonRequestHandling.HandleJsonWebRequestAsync<AsyncUnit>(false, OnRequestCompleted,
                OnRequestStatusCodeResolved, RequestMethod.DELETE, url, BaseUrl, null, Headers, customHeaders,
                queryParams, urlType, timeout, ct, InstanceID)).StatusCode;
        }

        public async UniTask<ApiResponse<T>> DeleteAsync<T>(string url,
            IReadOnlyDictionary<string, string> customHeaders = null,
            IReadOnlyDictionary<string, string> queryParams = null, UrlType urlType = UrlType.Relative,
            int timeout = 10, CancellationToken ct = default)
        {
            return await JsonRequestHandling.HandleJsonWebRequestAsync<T>(true, OnRequestCompleted,
                OnRequestStatusCodeResolved, RequestMethod.DELETE, url, BaseUrl, null, Headers, customHeaders,
                queryParams, urlType, timeout, ct, InstanceID);
        }

        // ── Sprite Methods ────────────────────────────────────────────────────────

        public async UniTask<SpriteResponse> GetSpriteAsync(string url,
            IReadOnlyDictionary<string, string> customHeaders = null,
            IReadOnlyDictionary<string, string> queryParams = null, UrlType urlType = UrlType.Relative,
            int timeout = 10, CancellationToken ct = default)
        {
            var startTime = 0L;
            Logging.StartLogTimer(ref startTime);

            string cleanUrl = null;
            UnityWebRequest req = null;
            Exception exception = null;

            try
            {
                cleanUrl = UrlValidation.CombineAndValidateUrl(url, urlType, BaseUrl);
                req = UnityWebRequestTexture.GetTexture(cleanUrl);
                var result =
                    await SpriteRequestHandling.SendSpriteRequestAsync(req, Headers, customHeaders, queryParams,
                        timeout, ct);
                return new SpriteResponse(result.statusCode, result.sprite);
            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }
            finally
            {
                var statusCode = (int)(req?.responseCode ?? 0);
                OnRequestStatusCodeResolved?.Invoke(statusCode);
                Logging.ExecuteLogTrigger(false, false, false, cleanUrl, RequestMethod.GET_SPRITE, timeout, Headers,
                    customHeaders, queryParams, req, startTime, OnRequestCompleted, InstanceID, ex: exception);
                req?.Dispose();
            }
        }

        public async UniTask<SpriteResponse> GetCachedSpriteAsync(string url, int cacheDays = 14,
            IReadOnlyDictionary<string, string> customHeaders = null,
            IReadOnlyDictionary<string, string> queryParams = null, UrlType urlType = UrlType.Relative,
            int timeout = 10, CancellationToken ct = default)
        {
            Sprite downloadedSprite = null;
            var requestReference = new Ref<UnityWebRequest>();

            var operationResult = await CachedRequestHandling.GetCachedAssetAsync(
                BaseUrl,
                url,
                urlType,
                _cacheManager,
                downloadToBytes: async token =>
                {
                    var cleanUrl = UrlValidation.CombineAndValidateUrl(url, urlType, BaseUrl);
                    requestReference.Value = UnityWebRequestTexture.GetTexture(cleanUrl);
                    var downloadResult = await SpriteRequestHandling.SendSpriteRequestAsync(requestReference.Value,
                        Headers, customHeaders, queryParams, timeout, token);
                    downloadedSprite = downloadResult.sprite;
                    return new(downloadResult.statusCode, downloadedSprite.texture.EncodeToPNG());
                },
                deserializeFromPath: async (path, token) =>
                {
                    if (downloadedSprite != null) return downloadedSprite;

                    var bytes = await UniTask.RunOnThreadPool(
                        () => File.ReadAllBytes(path), cancellationToken: token);
                    return SpriteRequestHandling.BytesToSprite(bytes);
                }, cacheDays, requestReference, RequestMethod.GET_SPRITE, timeout, Headers, customHeaders, queryParams,
                ct, InstanceID, OnRequestCompleted, OnRequestStatusCodeResolved, "png");
            return new SpriteResponse(operationResult.statusCode, operationResult.asset);
        }

        // ── AudioClip Methods ─────────────────────────────────────────────────────

        public async UniTask<AudioClipResponse> GetAudioClipAsync(string url, AudioType audioType = AudioType.UNKNOWN,
            IReadOnlyDictionary<string, string> customHeaders = null,
            IReadOnlyDictionary<string, string> queryParams = null, UrlType urlType = UrlType.Relative,
            int timeout = 10, CancellationToken ct = default)
        {
            var startTime = 0L;
            Logging.StartLogTimer(ref startTime);

            string cleanUrl = null;
            UnityWebRequest req = null;
            Exception exception = null;

            try
            {
                cleanUrl = UrlValidation.CombineAndValidateUrl(url, urlType, BaseUrl);

                var ext = CachedRequestHandling.GetUrlFileExtension(url);
                if (ext == null)
                    throw new InvalidUrlException(url, null, null,
                        "When Requesting for an AudioClip url, url must contain file extension.");
                var resolvedType = audioType == AudioType.UNKNOWN
                    ? AudioClipRequestHandling.DetectAudioType(url)
                    : audioType;

                req = UnityWebRequestMultimedia.GetAudioClip(cleanUrl, resolvedType);
                var result =
                    await AudioClipRequestHandling.SendAudioClipRequestAsync(req, Headers, customHeaders, queryParams,
                        timeout, ct);
                return new AudioClipResponse(result.statusCode, result.clip);
            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }
            finally
            {
                var statusCode = (int)(req?.responseCode ?? 0);
                OnRequestStatusCodeResolved?.Invoke(statusCode);
                Logging.ExecuteLogTrigger(false, false, false, cleanUrl, RequestMethod.GET_AUDIOCLIP, timeout, Headers,
                    customHeaders, queryParams, req, startTime, OnRequestCompleted, InstanceID, ex: exception);
                req?.Dispose();
            }
        }

        public async UniTask<AudioClipResponse> GetCachedAudioClipAsync(string url,
            AudioType audioType = AudioType.UNKNOWN, int cacheDays = 14,
            IReadOnlyDictionary<string, string> customHeaders = null,
            IReadOnlyDictionary<string, string> queryParams = null, UrlType urlType = UrlType.Relative,
            int timeout = 10, CancellationToken ct = default)
        {
            var resolvedType = audioType == AudioType.UNKNOWN
                ? AudioClipRequestHandling.DetectAudioType(url)
                : audioType;
            var requestReference = new Ref<UnityWebRequest>();

            var operationResult = await CachedRequestHandling.GetCachedAssetAsync(
                BaseUrl,
                url,
                urlType,
                _cacheManager,
                downloadToBytes: async token =>
                {
                    requestReference.Value =
                        UnityWebRequestMultimedia.GetAudioClip(
                            UrlValidation.CombineAndValidateUrl(url, urlType, BaseUrl), resolvedType);
                    return await AudioClipRequestHandling.SendRawBytesRequestAsync(requestReference.Value, Headers,
                        customHeaders, queryParams, timeout, token);
                },
                deserializeFromPath: (path, token) =>
                {
                    return AudioClipRequestHandling.LoadAudioClipFromPathAsync(path, resolvedType, token);
                }, cacheDays, requestReference, RequestMethod.GET_AUDIOCLIP, timeout, Headers, customHeaders,
                queryParams, ct, InstanceID, OnRequestCompleted, OnRequestStatusCodeResolved, null);
            return new AudioClipResponse(operationResult.statusCode, operationResult.asset);
        }
    }
}