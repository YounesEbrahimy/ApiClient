using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ApiClientLib.Extensions;
using UnityEngine.Networking;
using System.Threading;
using Newtonsoft.Json;
using System.Text;
using System;

namespace ApiClientLib.Helpers
{
    internal static class JsonRequestHandling
    {
        internal static async UniTask<ApiResponse<T>> HandleJsonWebRequestAsync<T>(bool processResponse,
            Action<ApiEventData> onRequestCompleted, Action<int> onRequestStatusCodeResolved, RequestMethod method,
            string url, string baseUrl, object body, IReadOnlyDictionary<string, string> persistentHeaders,
            IReadOnlyDictionary<string, string> customHeaders, IReadOnlyDictionary<string, string> queryParams,
            UrlType urlType, int timeout, CancellationToken ct, int instanceID)
        {
            var startTime = 0L;
            Logging.StartLogTimer(ref startTime);

            string cleanUrl = null;
            UnityWebRequest req = null;
            Exception exception = null;

            try
            {
                cleanUrl = UrlValidation.CombineAndValidateUrl(url, urlType, baseUrl);
                req = CreateJsonWebRequest(cleanUrl, method, body, persistentHeaders, customHeaders, queryParams,
                    timeout);
                return await (processResponse
                    ? SendJsonWebRequestAndProcessResponseAsync<T>(req, ct)
                    : SendJsonWebRequestAndForgetAsync<T>(req, ct));
            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }
            finally
            {
                var statusCode = (int)(req?.responseCode ?? 0);
                onRequestStatusCodeResolved?.Invoke(statusCode);
                Logging.ExecuteLogTrigger(true, processResponse, false, cleanUrl, method, timeout, persistentHeaders,
                    customHeaders, queryParams, req, startTime, onRequestCompleted, instanceID, ex: exception);
                req?.Dispose();
            }
        }

        private static UnityWebRequest CreateJsonWebRequest(string cleanUrl, RequestMethod method, object body,
            IReadOnlyDictionary<string, string> persistentHeaders, IReadOnlyDictionary<string, string> headers,
            IReadOnlyDictionary<string, string> queryParams, int timeout)
        {
            var req = new UnityWebRequest(cleanUrl, method.ToString())
            {
                downloadHandler = new DownloadHandlerBuffer()
            };

            if (body != null)
            {
                try
                {
                    var json = JsonConvert.SerializeObject(body);
                    req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
                }
                catch (Exception e)
                {
                    throw new JsonException($"Failed to serialize request body of type: [{body.GetType()}]", e);
                }
            }

            req.SetRequestHeader("Content-Type", "application/json");
            req.ApplyRequestProperties(persistentHeaders, headers, queryParams, timeout);
            return req;
        }

        private static async UniTask<ApiResponse<T>> SendJsonWebRequestAndForgetAsync<T>(UnityWebRequest req,
            CancellationToken ct)
        {
            return new ApiResponse<T>(await SendJsonWebRequestAsync(req, ct), default);
        }

        private static async UniTask<ApiResponse<T>> SendJsonWebRequestAndProcessResponseAsync<T>(UnityWebRequest req,
            CancellationToken ct)
        {
            var statusCode = await SendJsonWebRequestAsync(req, ct);
            if (req.IsBodyLessResponse())
                throw new ApiException((int)req.responseCode, "Expected response but none was provided by the server");

            var text = req.downloadHandler?.text;

            if (string.IsNullOrEmpty(text))
                throw new JsonException("Received empty or null response body from server.",
                    new NullReferenceException("Server response was null"));

            if (typeof(T) == typeof(string))
                return new ApiResponse<T>(statusCode, (T)(object)text);

            try
            {
                return new ApiResponse<T>(statusCode, JsonConvert.DeserializeObject<T>(text));
            }
            catch (Exception e)
            {
                throw new JsonException($"Failed to deserialize response to type [{typeof(T)}]", e);
            }
        }

        private static async UniTask<int> SendJsonWebRequestAsync(UnityWebRequest request, CancellationToken ct)
        {
            try
            {
                await request.SendWebRequest().ToUniTask(cancellationToken: ct);
                return (int)request.responseCode;
            }
            catch (UnityWebRequestException e)
            {
                e.UnityWebRequest.ThrowIfTimeout();
                throw new ApiException((int)e.ResponseCode,
                    string.IsNullOrEmpty(e.UnityWebRequest.error)
                        ? e.UnityWebRequest.result.ToString()
                        : e.UnityWebRequest.error);
            }
        }
    }
}