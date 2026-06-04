using System;

namespace ApiClientLib
{
    public class JsonException : Exception
    {
        public JsonException(Exception ex) : base($"{ex.Message ?? string.Empty}", ex)
        {
        }
    }
}