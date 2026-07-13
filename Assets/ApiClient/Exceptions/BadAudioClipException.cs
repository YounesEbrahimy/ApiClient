using Cysharp.Threading.Tasks;
using System;

namespace ApiClientLib
{
    public sealed class BadAudioClipException : Exception
    {
        public BadAudioClipException(UnityWebRequestException ex) : base(
            $"{ex.UnityWebRequest.downloadHandler.error ?? ex.UnityWebRequest.error}", ex)
        {
        }

        public BadAudioClipException(Exception ex) : base($"{ex.Message ?? string.Empty}", ex)
        {
        }
    }
}