using UnityEngine;

namespace ApiClientLib
{
    public sealed class AudioClipResponse : ApiResponseBase
    {
        public AudioClipResponse(int statusCode, AudioClip audioClip) : base(statusCode)
        {
            AudioClip = audioClip;
        }

        public AudioClip AudioClip { get; }
    }
}