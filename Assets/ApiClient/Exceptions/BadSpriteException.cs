using Cysharp.Threading.Tasks;
using System;

namespace ApiClientLib
{
    public sealed class BadSpriteException : Exception
    {
        public BadSpriteException(UnityWebRequestException ex) : base(
            $"{ex.UnityWebRequest.downloadHandler.error ?? ex.UnityWebRequest.error}", ex)
        {
        }

        public BadSpriteException(Exception ex) : base($"{ex.Message ?? string.Empty}", ex)
        {
        }
    }
}