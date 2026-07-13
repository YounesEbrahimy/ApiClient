using Cysharp.Threading.Tasks;
using System.Threading;
using System;

namespace ApiClientLib.SubClasses
{
    internal interface ICacheManager
    {
        UniTask InvalidateAsync(CancellationToken ct);

        UniTask<(T asset, bool wasCached, int statusCode)> GetOrUpdateAsync<T>(string key, string fileExt, string url,
            int cacheDays,
            Func<CancellationToken, UniTask<(int statusCode, byte[] bytes)>> downloadAsync,
            Func<string, CancellationToken, UniTask<T>> deserializeAsync, CancellationToken ct);
    }
}