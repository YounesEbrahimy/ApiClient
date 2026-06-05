using System;

namespace ApiClientLib
{
    public class BadAudioClipException : Exception
    {
        public BadAudioClipException(Exception ex) : base($"{ex.Message ?? string.Empty}", ex)
        {
        }
    }
}