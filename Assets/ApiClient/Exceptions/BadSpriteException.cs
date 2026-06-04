using System;

namespace ApiClientLib
{
    public class BadSpriteException : Exception
    {
        public BadSpriteException(Exception ex) : base($"{ex.Message ?? string.Empty}", ex)
        {
        }
    }
}