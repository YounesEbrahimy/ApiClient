using System.Collections.Generic;
using ApiClientLib.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using System.Threading;
using UnityEngine;
using System;

namespace ApiClientLib.Helpers
{
    internal static class SpriteRequestHandling
    {
        internal static async UniTask<(int statusCode, Sprite sprite)> SendSpriteRequestAsync(UnityWebRequest req,
            IReadOnlyDictionary<string, string> persistentHeaders, IReadOnlyDictionary<string, string> customHeaders,
            IReadOnlyDictionary<string, string> queryParams, int timeout, CancellationToken ct)
        {
            req.ApplyRequestProperties(persistentHeaders, customHeaders, queryParams, timeout);

            try
            {
                await req.SendWebRequest().ToUniTask(cancellationToken: ct);
            }
            catch (UnityWebRequestException e)
            {
                e.UnityWebRequest.ThrowIfTimeout();
                if (req.responseCode is >= 200 and < 300)
                {
                    if (req.IsBodyLessResponse())
                        throw new ApiException((int)req.responseCode,
                            "Expected image data but none was provided by the server");
                    else
                        throw new BadSpriteException(e);
                }
                else
                {
                    throw new ApiException((int)e.ResponseCode,
                        string.IsNullOrEmpty(e.UnityWebRequest.error)
                            ? e.UnityWebRequest.result.ToString()
                            : e.UnityWebRequest.error);
                }
            }

            var tex = DownloadHandlerTexture.GetContent(req);
            Sprite result;
            try
            {
                result = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            }
            catch (Exception e)
            {
                throw new BadSpriteException(e);
            }

            return ((int)req.responseCode, result);
        }

        internal static Sprite BytesToSprite(byte[] bytes)
        {
            var tex = new Texture2D(2, 2);
            tex.LoadImage(bytes);
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }
    }
}