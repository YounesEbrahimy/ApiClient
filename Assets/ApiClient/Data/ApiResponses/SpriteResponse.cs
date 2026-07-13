using UnityEngine;

namespace ApiClientLib
{
    public sealed class SpriteResponse : ApiResponseBase
    {
        public SpriteResponse(int statusCode, Sprite sprite) : base(statusCode)
        {
            Sprite = sprite;
        }

        public Sprite Sprite { get; }
    }
}