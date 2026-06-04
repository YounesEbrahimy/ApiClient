using UnityEngine.Networking;
using System;

namespace ApiClientLib
{
    internal static class UnityWebRequestExtensions
    {
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
    }
}