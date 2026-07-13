using System.Collections.Generic;
using UnityEngine.Networking;
using Codice.Utils;
using System;

namespace ApiClientLib.Extensions
{
    internal static class UnityWebRequestExtensions
    {
        internal static void ApplyRequestProperties(this UnityWebRequest request,
            IReadOnlyDictionary<string, string> persistentHeaders, IReadOnlyDictionary<string, string> customHeaders,
            IReadOnlyDictionary<string, string> queryParams, int timeout)
        {
            foreach (var kvp in persistentHeaders) request.SetRequestHeader(kvp.Key, kvp.Value);
            if (customHeaders != null)
            {
                foreach (var kvp in customHeaders) request.SetRequestHeader(kvp.Key, kvp.Value);
            }

            if (queryParams != null)
            {
                var uriBuilder = new UriBuilder(request.url);
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);

                foreach (var qParam in queryParams)
                {
                    query[qParam.Key] = qParam.Value;
                }

                uriBuilder.Query = query.ToString();
                request.url = uriBuilder.ToString();
            }

            request.timeout = timeout;
        }

        internal static void ThrowIfTimeout(this UnityWebRequest request)
        {
            if (request.result != UnityWebRequest.Result.ConnectionError) return;
            if (request.error == null) return;

            var isTimeout =
                // For all requests
                request.error.IndexOf("timeout", StringComparison.OrdinalIgnoreCase) >= 0 ||
                // For Texture requests
                (request.responseCode == 0 &&
                 request.error.IndexOf("Access Denied", StringComparison.OrdinalIgnoreCase) >= 0);

            if (isTimeout)
                throw new TimeoutException($"Request timed out: {request.url}");
        }

        internal static bool IsBodyLessResponse(this UnityWebRequest request)
        {
            return request.responseCode is 204 or 205 or 304 or >= 100 and < 200;
        }
    }
}