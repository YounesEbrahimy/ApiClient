using System.Collections.Generic;
using System;

namespace ApiClientLib
{
    public sealed class ApiEventData
    {
        public ApiEventData(bool success, bool fromCache, RequestMethod method, string url, int timeout,
            string requestBody, IReadOnlyDictionary<string, string> requestHeaders,
            IReadOnlyDictionary<string, string> queryParams, float duration, DateTime timestamp, int instanceID,
            Exception exception = null, string errorMessage = null, int statusCode = -1, string responseBody = null,
            IReadOnlyDictionary<string, string> responseHeaders = null)
        {
            Success = success;
            FromCache = fromCache;
            Method = method;
            URL = url;
            Timeout = timeout;
            RequestBody = requestBody;
            RequestHeaders = requestHeaders;
            QueryParams = queryParams;
            Exception = exception;
            ErrorMessage = errorMessage;
            Duration = duration;
            Timestamp = timestamp;
            InstanceID = instanceID;
            StatusCode = statusCode;
            ResponseBody = responseBody;
            ResponseHeaders = responseHeaders;
        }

        public bool Success { get; }
        public bool FromCache { get; }
        public RequestMethod Method { get; }
        public string URL { get; }
        public int Timeout { get; }
        public string RequestBody { get; }
        public IReadOnlyDictionary<string, string> RequestHeaders { get; }
        public IReadOnlyDictionary<string, string> QueryParams { get; }
        public int StatusCode { get; }
        public string ResponseBody { get; }
        public IReadOnlyDictionary<string, string> ResponseHeaders { get; }
        public Exception Exception { get; }
        public string ErrorMessage { get; }
        public float Duration { get; }
        public DateTime Timestamp { get; }
        public int InstanceID { get; }
    }
}