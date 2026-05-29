using System;
using System.Net;

public class ApiException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public ApiException(HttpStatusCode statusCode, string message)
        : base($"[{(int)statusCode} {statusCode}] {message}")
    {
        StatusCode = statusCode;
    }
}