using System.Collections.Generic;
using ApiClientLib.SubClasses;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using System.Threading;
using System.Text;
using System.IO;
using System;

namespace ApiClientLib.Helpers
{
    internal static class CachedRequestHandling
    {
        internal static async UniTask<(int statusCode, T asset)> GetCachedAssetAsync<T>(string baseUrl, string url,
            UrlType urlType, ICacheManager cacheManager,
            Func<CancellationToken, UniTask<(int statusCode, byte[] bytes)>> downloadToBytes,
            Func<string, CancellationToken, UniTask<T>> deserializeFromPath, int cacheDays,
            Ref<UnityWebRequest> requestReference, RequestMethod method, int timeout,
            IReadOnlyDictionary<string, string> persistentHeaders, IReadOnlyDictionary<string, string> customHeaders,
            IReadOnlyDictionary<string, string> queryParams, CancellationToken ct, int instanceID,
            Action<ApiEventData> onRequestCompleted, Action<int> onRequestStatusCodeResolved, string fileExtension)
        {
            var startTime = 0L;
            Logging.StartLogTimer(ref startTime);

            string cleanUrl = null;
            try
            {
                cleanUrl = UrlValidation.CombineAndValidateUrl(url, urlType, baseUrl);
                fileExtension ??= GetUrlFileExtension(url) ?? throw new InvalidUrlException(url, null, null,
                    "When Requesting for an AudioClip url, url must contain file extension.");

                var key = ComputeHash(cleanUrl);
                var (result, wasCached, statusCode) = await cacheManager.GetOrUpdateAsync(key, fileExtension, cleanUrl,
                    cacheDays, downloadToBytes, deserializeFromPath, ct);
                var finalStatusCode = wasCached ? -1 : (int)(requestReference.Value?.responseCode ?? statusCode);
                onRequestStatusCodeResolved?.Invoke(finalStatusCode);
                Logging.ExecuteLogTrigger(false, false, wasCached, cleanUrl, method, timeout, persistentHeaders,
                    customHeaders, queryParams, requestReference.Value, startTime, onRequestCompleted, instanceID);
                return (statusCode, result);
            }
            catch (Exception e)
            {
                var finalStatusCode = (int)(requestReference.Value?.responseCode ?? 0);
                onRequestStatusCodeResolved?.Invoke(finalStatusCode);
                Logging.ExecuteLogTrigger(false, false, false, cleanUrl, method, timeout, persistentHeaders,
                    customHeaders, queryParams, requestReference.Value, startTime, onRequestCompleted, instanceID,
                    ex: e);
                throw;
            }
            finally
            {
                requestReference.Value?.Dispose();
            }
        }

        internal static string GetUrlFileExtension(string url, string fallback = null)
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

        internal static string ComputeHash(string input)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }
    }
}