using ApiClientLib.SubClasses;
using ApiClientLib;

internal static class ApiClientExtensions
{
    internal static string CacheDirectoryPath(this ApiClient client) =>
        ((CacheManager)client._cacheManager)._cacheDir;

    internal static string CacheIndexPath(this ApiClient client) =>
        ((CacheManager)client._cacheManager)._cacheIndexPath;
}