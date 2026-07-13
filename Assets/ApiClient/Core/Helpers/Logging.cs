using System.Collections.Generic;
using UnityEngine.Networking;
using System.Diagnostics;
using System.Text;
using System;

#if UNITY_EDITOR
using ApiClientLib.Editor;
#endif

namespace ApiClientLib.Helpers
{
    internal static class Logging
    {
        private const string EditorSymbol = "UNITY_EDITOR";
        private const string LoggingSymbol = "APICLIENT_LOGGING_ENABLED";

        [Conditional(EditorSymbol), Conditional(LoggingSymbol)]
        internal static void StartLogTimer(ref long startTime)
        {
            startTime = Stopwatch.GetTimestamp();
        }

        [Conditional(EditorSymbol), Conditional(LoggingSymbol)]
        internal static void ExecuteLogTrigger(bool isJsonRequest, bool processJsonResponse, bool fromCache,
            string cleanUrl, RequestMethod method, int timeout, IReadOnlyDictionary<string, string> persistentHeaders,
            IReadOnlyDictionary<string, string> customHeaders, IReadOnlyDictionary<string, string> queryParams,
            UnityWebRequest req, long startTime, Action<ApiEventData> eventInvoker, int instanceID, Exception ex = null)
        {
            var duration = (float)(Stopwatch.GetTimestamp() - startTime) / Stopwatch.Frequency;
            var success = ex == null;

            var combinedHeaders = new Dictionary<string, string>();
            foreach (var kvp in persistentHeaders) combinedHeaders[kvp.Key] = kvp.Value;
            if (customHeaders != null)
            {
                foreach (var kvp in customHeaders) combinedHeaders[kvp.Key] = kvp.Value;
            }

            var data = new ApiEventData(
                success,
                fromCache,
                method,
                cleanUrl,
                timeout,
                req?.uploadHandler?.data is byte[] bytes ? Encoding.UTF8.GetString(bytes) : null,
                combinedHeaders,
                queryParams,
                duration,
                DateTime.Now,
                instanceID,
                exception: ex,
                errorMessage: success ? null : ex.Message,
                statusCode: fromCache ? -1 : (int)(req?.responseCode ?? 0),
                responseBody: isJsonRequest && processJsonResponse ? req?.downloadHandler?.text : null,
                responseHeaders: (req?.GetResponseHeaders() == null || req.GetResponseHeaders().Count == 0)
                    ? null
                    : req.GetResponseHeaders()
            );

            ApiClientLoggerWindow.Log(data);
            eventInvoker?.Invoke(data);
        }
    }
}