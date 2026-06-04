using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace ApiClientLib
{
    public interface IApiClient
    {
        string BaseUrl { get; }
        void SetBaseUrl(string baseUrl);

        void AddHeader(string key, string value);
        void RemoveHeader(string key);
        void ClearHeaders();

        UniTask GetAsync(string url, Dictionary<string, string> headers = null, int timeout = 10,
            CancellationToken ct = default);

        UniTask<T> GetAsync<T>(string url, Dictionary<string, string> headers = null, int timeout = 10,
            CancellationToken ct = default);

        UniTask PostAsync(string url, object body, Dictionary<string, string> headers = null, int timeout = 10,
            CancellationToken ct = default);

        UniTask<T> PostAsync<T>(string url, object body, Dictionary<string, string> headers = null, int timeout = 10,
            CancellationToken ct = default);

        UniTask PutAsync(string url, object body, Dictionary<string, string> headers = null, int timeout = 10,
            CancellationToken ct = default);

        UniTask<T> PutAsync<T>(string url, object body, Dictionary<string, string> headers = null, int timeout = 10,
            CancellationToken ct = default);

        UniTask PatchAsync(string url, object body, Dictionary<string, string> headers = null, int timeout = 10,
            CancellationToken ct = default);

        UniTask<T> PatchAsync<T>(string url, object body, Dictionary<string, string> headers = null, int timeout = 10,
            CancellationToken ct = default);

        UniTask DeleteAsync(string url, Dictionary<string, string> headers = null, int timeout = 10,
            CancellationToken ct = default);

        UniTask<T> DeleteAsync<T>(string url, Dictionary<string, string> headers = null, int timeout = 10,
            CancellationToken ct = default);

        UniTask<Sprite> GetSpriteAsync(string url, Dictionary<string, string> headers = null, int timeout = 10,
            CancellationToken ct = default);

        UniTask<Sprite> GetCachedSpriteAsync(string url, int cacheDays = 14, Dictionary<string, string> headers = null,
            int timeout = 10, CancellationToken ct = default);
    }
}