using System;

namespace ApiClientLib
{
    public class ApiException : Exception
    {
        public int StatusCode { get; }
        public string ResponseMessage { get; }

        public ApiException(int statusCode, string responseMessage)
            : base($"API error {statusCode}: {responseMessage ?? string.Empty}")
        {
            StatusCode = statusCode;
            ResponseMessage = responseMessage ?? string.Empty;
        }
    }
}