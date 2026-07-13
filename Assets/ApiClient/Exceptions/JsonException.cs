using System;

namespace ApiClientLib
{
    public sealed class JsonException : Exception
    {
        public JsonException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}