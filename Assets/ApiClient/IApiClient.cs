using System.Threading.Tasks;
using UnityEngine;

public interface IApiClient
{
    /// <summary>Base URL prepended to all non-sprite requests.</summary>
    string BaseUrl { get; }

    /// <summary>Sets the base URL. Throws <see cref="ArgumentNullException"/> if null.</summary>
    void SetBaseUrl(string baseUrl);

    /// <summary>GET request. <paramref name="timeout"/> in seconds.</summary>
    Task<string> GetAsync(string url, int timeout = 30);

    /// <summary>POST request. <paramref name="timeout"/> in seconds.</summary>
    Task<string> PostAsync(string url, string jsonBody, int timeout = 30);

    /// <summary>PUT request. <paramref name="timeout"/> in seconds.</summary>
    Task<string> PutAsync(string url, string jsonBody, int timeout = 30);

    /// <summary>PATCH request. <paramref name="timeout"/> in seconds.</summary>
    Task<string> PatchAsync(string url, string jsonBody, int timeout = 30);

    /// <summary>DELETE request. <paramref name="timeout"/> in seconds.</summary>
    Task<string> DeleteAsync(string url, int timeout = 30);

    /// <summary>DELETE request with body. <paramref name="timeout"/> in seconds.</summary>
    Task<string> DeleteAsync(string url, string jsonBody, int timeout = 30);

    /// <summary>Downloads a sprite from a full URL (BaseUrl not applied).</summary>
    Task<Sprite> GetSpriteAsync(string url, int timeout = 30);

    /// <summary>Downloads and caches a sprite to disk. BaseUrl not applied.</summary>
    Task<Sprite> GetCachedSpriteAsync(string url, int timeout = 30);
}