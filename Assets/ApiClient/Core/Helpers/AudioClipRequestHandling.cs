using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ApiClientLib.Extensions;
using UnityEngine.Networking;
using System.Threading;
using UnityEngine;
using System;

namespace ApiClientLib.Helpers
{
    internal static class AudioClipRequestHandling
    {
        internal static async UniTask<(int statusCode, AudioClip clip)> SendAudioClipRequestAsync(
            UnityWebRequest request, IReadOnlyDictionary<string, string> persistentHeaders,
            IReadOnlyDictionary<string, string> customHeaders, IReadOnlyDictionary<string, string> queryParams,
            int timeout, CancellationToken ct)
        {
            var (completedRequest, clip) =
                await SendRequestInternalAsync(request, persistentHeaders, customHeaders, queryParams, timeout, ct);
            return new((int)completedRequest.responseCode, clip);
        }

        internal static async UniTask<(int statusCode, byte[] bytes)> SendRawBytesRequestAsync(UnityWebRequest request,
            IReadOnlyDictionary<string, string> persistentHeaders, IReadOnlyDictionary<string, string> customHeaders,
            IReadOnlyDictionary<string, string> queryParams, int timeout, CancellationToken ct)
        {
            var (completedRequest, clip) =
                await SendRequestInternalAsync(request, persistentHeaders, customHeaders, queryParams, timeout, ct);

            try
            {
                return new((int)completedRequest.responseCode, completedRequest.downloadHandler.data);
            }
            finally
            {
                if (clip != null)
                {
                    if (Application.isEditor)
                    {
                        UnityEngine.Object.DestroyImmediate(clip);
                    }
                    else
                    {
                        UnityEngine.Object.Destroy(clip);
                    }
                }
            }
        }

        private static async UniTask<(UnityWebRequest request, AudioClip clip)> SendRequestInternalAsync(
            UnityWebRequest request, IReadOnlyDictionary<string, string> persistentHeaders,
            IReadOnlyDictionary<string, string> customHeaders, IReadOnlyDictionary<string, string> queryParams,
            int timeout, CancellationToken ct)
        {
            request.ApplyRequestProperties(persistentHeaders, customHeaders, queryParams, timeout);

            try
            {
                await request.SendWebRequest().ToUniTask(cancellationToken: ct);
            }
            catch (UnityWebRequestException e)
            {
                e.UnityWebRequest.ThrowIfTimeout();
                if (request.responseCode is >= 200 and < 300)
                {
                    if (request.IsBodyLessResponse())
                        throw new ApiException((int)request.responseCode,
                            "Expected audio clip data but none was provided by the server");
                    else
                        throw new BadAudioClipException(e);
                }
                else
                {
                    throw new ApiException((int)e.ResponseCode,
                        string.IsNullOrEmpty(e.UnityWebRequest.error)
                            ? e.UnityWebRequest.result.ToString()
                            : e.UnityWebRequest.error);
                }
            }

            if (request.IsBodyLessResponse())
                throw new ApiException((int)request.responseCode,
                    "Expected audio clip data but none was provided by the server");

            try
            {
                var clip = DownloadHandlerAudioClip.GetContent(request);
                if (request.result == UnityWebRequest.Result.DataProcessingError || clip == null ||
                    clip.loadState == AudioDataLoadState.Failed)
                    throw new Exception("Invalid audio data received.");
                return (request, clip);
            }
            catch (Exception e)
            {
                throw new BadAudioClipException(e);
            }
        }

        internal static async UniTask<AudioClip> LoadAudioClipFromPathAsync(string filePath, AudioType audioType,
            CancellationToken ct)
        {
            using var req = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, audioType);
            ((DownloadHandlerAudioClip)req.downloadHandler).streamAudio = false;

            try
            {
                await req.SendWebRequest().ToUniTask(cancellationToken: ct);
            }
            catch (UnityWebRequestException e)
            {
                throw new BadAudioClipException(e);
            }

            try
            {
                return DownloadHandlerAudioClip.GetContent(req);
            }
            catch (Exception e)
            {
                throw new BadAudioClipException(e);
            }
        }

        internal static AudioType DetectAudioType(string url) =>
            CachedRequestHandling.GetUrlFileExtension(url, "audio") switch
            {
                "mp3" or "mpeg" => AudioType.MPEG,
                "ogg" => AudioType.OGGVORBIS,
                "acc" => AudioType.ACC,
                "wav" => AudioType.WAV,
                "aiff" or "aif" => AudioType.AIFF,
                _ => AudioType.UNKNOWN
            };
    }
}