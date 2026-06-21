using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace ApiClientLib
{
    /// <summary>
    /// Defines a Unity-friendly async HTTP client that supports JSON-based REST operations
    /// as well as downloading and caching Unity assets such as <see cref="Sprite"/> and <see cref="AudioClip"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// All HTTP methods are built on top of <see cref="UnityEngine.Networking.UnityWebRequest"/> and are
    /// awaitable via <see cref="UniTask"/>, making them safe to use on the Unity main thread without blocking.
    /// </para>
    /// <para>
    /// <b>URL Resolution:</b> The <see cref="BaseUrl"/> is prepended to the <c>url</c> parameter only when
    /// the request specifies <see cref="UrlType.Relative"/> as the URL type. If <see cref="UrlType.Absolute"/>
    /// is specified, the <see cref="BaseUrl"/> is not applied and the provided <c>url</c> is used as-is.
    /// The final URL is validated before the request is sent; an <see cref="InvalidUrlException"/> is thrown
    /// if it does not form a valid absolute HTTP or HTTPS address.
    /// </para>
    /// <para>
    /// <b>Persistent Headers:</b> Headers added via <see cref="AddHeader"/> are automatically applied to every
    /// outgoing request. Per-request <c>headers</c> parameters are merged on top and take precedence over
    /// persistent headers when keys conflict.
    /// </para>
    /// <para>
    /// <b>Serialization:</b> Request bodies are serialized to JSON using Newtonsoft.Json.
    /// Response bodies are deserialized from JSON into the requested type <c>T</c>.
    /// Passing <c>string</c> as <c>T</c> returns the raw response text without deserialization.
    /// </para>
    /// </remarks>
    public interface IApiClient
    {
        // ── Base URL ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Gets the base URL that is prepended to all relative request URLs.
        /// </summary>
        /// <remarks>
        /// A trailing slash is always enforced on the stored value.
        /// Returns an empty string if no base URL has been set.
        /// </remarks>
        string BaseUrl { get; }

        /// <summary>
        /// Sets the base URL that will be prepended to all relative request URLs.
        /// </summary>
        /// <param name="baseUrl">
        /// The base URL to set. A trailing slash is appended automatically if absent.
        /// Pass an empty string to clear the base URL.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if <paramref name="baseUrl"/> is <c>null</c>.
        /// </exception>
        void SetBaseUrl(string baseUrl);

        // ── Persistent Headers ────────────────────────────────────────────────────

        /// <summary>
        /// Gets a snapshot of all currently registered persistent headers.
        /// </summary>
        /// <remarks>
        /// Persistent headers are automatically included in every outgoing request.
        /// Modify them via <see cref="AddHeader"/>, <see cref="RemoveHeader"/>, or <see cref="ClearHeaders"/>.
        /// </remarks>
        Dictionary<string, string> Headers { get; }

        /// <summary>
        /// Adds or overwrites a persistent header that will be sent with every subsequent request.
        /// </summary>
        /// <param name="key">The header name (e.g. <c>"Authorization"</c>).</param>
        /// <param name="value">The header value (e.g. <c>"Bearer &lt;token&gt;"</c>).</param>
        void AddHeader(string key, string value);

        /// <summary>
        /// Removes a previously registered persistent header by its key.
        /// </summary>
        /// <param name="key">The header name to remove. No-op if the key does not exist.</param>
        void RemoveHeader(string key);

        /// <summary>
        /// Removes all persistent headers that were previously added via <see cref="AddHeader"/>.
        /// </summary>
        void ClearHeaders();

        // ── Cache Control ─────────────────────────────────────────────────────────

        /// <summary>
        /// Deletes the cache folder and its contents, and resets the indexes.
        /// </summary>
        /// <param name="ct">Token used to cancel the operation.</param>
        UniTask InvalidateCacheAsync(CancellationToken ct);

        // ── GET ───────────────────────────────────────────────────────────────────

        /// <summary>
        /// Sends an HTTP GET request and discards the response body.
        /// </summary>
        /// <param name="url">
        /// The relative or absolute endpoint URL. Relative paths are appended to <see cref="BaseUrl"/>.
        /// </param>
        /// <param name="headers">
        /// Optional per-request headers. Merged with persistent headers; these take precedence on key conflicts.
        /// </param>
        /// <param name="queryParams">
        /// Optional query parameters appended to the URL (e.g. <c>{ "page", "1" }</c>).
        /// </param>
        /// <param name="urlType">Determines whether url should be appended to the base url.</param>
        /// <param name="timeout">Request timeout in seconds. Defaults to <c>10</c>.</param>
        /// <param name="ct">Token used to cancel the in-flight request.</param>
        /// <exception cref="ApiException">Thrown on a non-2xx HTTP response.</exception>
        /// <exception cref="InvalidUrlException">Thrown if the resolved URL is not a valid absolute HTTP/HTTPS address.</exception>
        /// <exception cref="System.OperationCanceledException">Thrown if <paramref name="ct"/> is cancelled.</exception>
        /// <exception cref="System.TimeoutException">Thrown if the request times out.</exception>
        UniTask GetAsync(string url, Dictionary<string, string> headers = null,
            Dictionary<string, string> queryParams = null, UrlType urlType = UrlType.Relative, int timeout = 10,
            CancellationToken ct = default);

        /// <summary>
        /// Sends an HTTP GET request and deserializes the JSON response body to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type to deserialize the response into.
        /// Use <c>string</c> to receive the raw response text without deserialization.
        /// </typeparam>
        /// <param name="url">
        /// The relative or absolute endpoint URL. Relative paths are appended to <see cref="BaseUrl"/>.
        /// </param>
        /// <param name="headers">
        /// Optional per-request headers. Merged with persistent headers; these take precedence on key conflicts.
        /// </param>
        /// <param name="queryParams">
        /// Optional query parameters appended to the URL (e.g. <c>{ "page", "1" }</c>).
        /// </param>
        /// <param name="urlType">Determines whether url should be appended to the base url.</param>
        /// <param name="timeout">Request timeout in seconds. Defaults to <c>10</c>.</param>
        /// <param name="ct">Token used to cancel the in-flight request.</param>
        /// <returns>The response body deserialized as <typeparamref name="T"/>.</returns>
        /// <exception cref="ApiException">
        /// Thrown on a non-2xx HTTP response, or if the server returns a body-less status code
        /// (e.g. 204 No Content) when a response body is expected.
        /// </exception>
        /// <exception cref="JsonException">Thrown if the response body cannot be deserialized into <typeparamref name="T"/>.</exception>
        /// <exception cref="InvalidUrlException">Thrown if the resolved URL is not a valid absolute HTTP/HTTPS address.</exception>
        /// <exception cref="System.OperationCanceledException">Thrown if <paramref name="ct"/> is cancelled.</exception>
        /// <exception cref="System.TimeoutException">Thrown if the request times out.</exception>
        UniTask<T> GetAsync<T>(string url, Dictionary<string, string> headers = null,
            Dictionary<string, string> queryParams = null, UrlType urlType = UrlType.Relative, int timeout = 10,
            CancellationToken ct = default);

        // ── POST ──────────────────────────────────────────────────────────────────

        /// <summary>
        /// Sends an HTTP POST request with a JSON-serialized body and discards the response body.
        /// </summary>
        /// <param name="url">
        /// The relative or absolute endpoint URL. Relative paths are appended to <see cref="BaseUrl"/>.
        /// </param>
        /// <param name="body">
        /// The request payload. Serialized to JSON via Newtonsoft.Json before sending.
        /// Pass <c>null</c> to send a body-less request.
        /// </param>
        /// <param name="headers">
        /// Optional per-request headers. Merged with persistent headers; these take precedence on key conflicts.
        /// </param>
        /// <param name="queryParams">
        /// Optional query parameters appended to the URL.
        /// </param>
        /// <param name="urlType">Determines whether url should be appended to the base url.</param>
        /// <param name="timeout">Request timeout in seconds. Defaults to <c>10</c>.</param>
        /// <param name="ct">Token used to cancel the in-flight request.</param>
        /// <exception cref="ApiException">Thrown on a non-2xx HTTP response.</exception>
        /// <exception cref="JsonException">Thrown if <paramref name="body"/> cannot be serialized to JSON.</exception>
        /// <exception cref="InvalidUrlException">Thrown if the resolved URL is not a valid absolute HTTP/HTTPS address.</exception>
        /// <exception cref="System.OperationCanceledException">Thrown if <paramref name="ct"/> is cancelled.</exception>
        /// <exception cref="System.TimeoutException">Thrown if the request times out.</exception>
        UniTask PostAsync(string url, object body, Dictionary<string, string> headers = null,
            Dictionary<string, string> queryParams = null, UrlType urlType = UrlType.Relative, int timeout = 10,
            CancellationToken ct = default);

        /// <summary>
        /// Sends an HTTP POST request with a JSON-serialized body and deserializes the JSON response to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type to deserialize the response into.
        /// Use <c>string</c> to receive the raw response text without deserialization.
        /// </typeparam>
        /// <param name="url">
        /// The relative or absolute endpoint URL. Relative paths are appended to <see cref="BaseUrl"/>.
        /// </param>
        /// <param name="body">
        /// The request payload. Serialized to JSON via Newtonsoft.Json before sending.
        /// Pass <c>null</c> to send a body-less request.
        /// </param>
        /// <param name="headers">
        /// Optional per-request headers. Merged with persistent headers; these take precedence on key conflicts.
        /// </param>
        /// <param name="queryParams">
        /// Optional query parameters appended to the URL.
        /// </param>
        /// <param name="urlType">Determines whether url should be appended to the base url.</param>
        /// <param name="timeout">Request timeout in seconds. Defaults to <c>10</c>.</param>
        /// <param name="ct">Token used to cancel the in-flight request.</param>
        /// <returns>The response body deserialized as <typeparamref name="T"/>.</returns>
        /// <exception cref="ApiException">
        /// Thrown on a non-2xx HTTP response, or if the server returns a body-less status code
        /// when a response body is expected.
        /// </exception>
        /// <exception cref="JsonException">Thrown if <paramref name="body"/> cannot be serialized, or if the response cannot be deserialized into <typeparamref name="T"/>.</exception>
        /// <exception cref="InvalidUrlException">Thrown if the resolved URL is not a valid absolute HTTP/HTTPS address.</exception>
        /// <exception cref="System.OperationCanceledException">Thrown if <paramref name="ct"/> is cancelled.</exception>
        /// <exception cref="System.TimeoutException">Thrown if the request times out.</exception>
        UniTask<T> PostAsync<T>(string url, object body, Dictionary<string, string> headers = null,
            Dictionary<string, string> queryParams = null, UrlType urlType = UrlType.Relative, int timeout = 10,
            CancellationToken ct = default);

        // ── PUT ───────────────────────────────────────────────────────────────────

        /// <summary>
        /// Sends an HTTP PUT request with a JSON-serialized body and discards the response body.
        /// </summary>
        /// <param name="url">
        /// The relative or absolute endpoint URL. Relative paths are appended to <see cref="BaseUrl"/>.
        /// </param>
        /// <param name="body">
        /// The request payload. Serialized to JSON via Newtonsoft.Json before sending.
        /// Pass <c>null</c> to send a body-less request.
        /// </param>
        /// <param name="headers">
        /// Optional per-request headers. Merged with persistent headers; these take precedence on key conflicts.
        /// </param>
        /// <param name="queryParams">
        /// Optional query parameters appended to the URL.
        /// </param>
        /// <param name="urlType">Determines whether url should be appended to the base url.</param>
        /// <param name="timeout">Request timeout in seconds. Defaults to <c>10</c>.</param>
        /// <param name="ct">Token used to cancel the in-flight request.</param>
        /// <exception cref="ApiException">Thrown on a non-2xx HTTP response.</exception>
        /// <exception cref="JsonException">Thrown if <paramref name="body"/> cannot be serialized to JSON.</exception>
        /// <exception cref="InvalidUrlException">Thrown if the resolved URL is not a valid absolute HTTP/HTTPS address.</exception>
        /// <exception cref="System.OperationCanceledException">Thrown if <paramref name="ct"/> is cancelled.</exception>
        /// <exception cref="System.TimeoutException">Thrown if the request times out.</exception>
        UniTask PutAsync(string url, object body, Dictionary<string, string> headers = null,
            Dictionary<string, string> queryParams = null, UrlType urlType = UrlType.Relative, int timeout = 10,
            CancellationToken ct = default);

        /// <summary>
        /// Sends an HTTP PUT request with a JSON-serialized body and deserializes the JSON response to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type to deserialize the response into.
        /// Use <c>string</c> to receive the raw response text without deserialization.
        /// </typeparam>
        /// <param name="url">
        /// The relative or absolute endpoint URL. Relative paths are appended to <see cref="BaseUrl"/>.
        /// </param>
        /// <param name="body">
        /// The request payload. Serialized to JSON via Newtonsoft.Json before sending.
        /// Pass <c>null</c> to send a body-less request.
        /// </param>
        /// <param name="headers">
        /// Optional per-request headers. Merged with persistent headers; these take precedence on key conflicts.
        /// </param>
        /// <param name="queryParams">
        /// Optional query parameters appended to the URL.
        /// </param>
        /// <param name="urlType">Determines whether url should be appended to the base url.</param>
        /// <param name="timeout">Request timeout in seconds. Defaults to <c>10</c>.</param>
        /// <param name="ct">Token used to cancel the in-flight request.</param>
        /// <returns>The response body deserialized as <typeparamref name="T"/>.</returns>
        /// <exception cref="ApiException">
        /// Thrown on a non-2xx HTTP response, or if the server returns a body-less status code
        /// when a response body is expected.
        /// </exception>
        /// <exception cref="JsonException">Thrown if <paramref name="body"/> cannot be serialized, or if the response cannot be deserialized into <typeparamref name="T"/>.</exception>
        /// <exception cref="InvalidUrlException">Thrown if the resolved URL is not a valid absolute HTTP/HTTPS address.</exception>
        /// <exception cref="System.OperationCanceledException">Thrown if <paramref name="ct"/> is cancelled.</exception>
        /// <exception cref="System.TimeoutException">Thrown if the request times out.</exception>
        UniTask<T> PutAsync<T>(string url, object body, Dictionary<string, string> headers = null,
            Dictionary<string, string> queryParams = null, UrlType urlType = UrlType.Relative, int timeout = 10,
            CancellationToken ct = default);

        // ── PATCH ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Sends an HTTP PATCH request with a JSON-serialized body and discards the response body.
        /// </summary>
        /// <param name="url">
        /// The relative or absolute endpoint URL. Relative paths are appended to <see cref="BaseUrl"/>.
        /// </param>
        /// <param name="body">
        /// The request payload. Serialized to JSON via Newtonsoft.Json before sending.
        /// Pass <c>null</c> to send a body-less request.
        /// </param>
        /// <param name="headers">
        /// Optional per-request headers. Merged with persistent headers; these take precedence on key conflicts.
        /// </param>
        /// <param name="queryParams">
        /// Optional query parameters appended to the URL.
        /// </param>
        /// <param name="urlType">Determines whether url should be appended to the base url.</param>
        /// <param name="timeout">Request timeout in seconds. Defaults to <c>10</c>.</param>
        /// <param name="ct">Token used to cancel the in-flight request.</param>
        /// <exception cref="ApiException">Thrown on a non-2xx HTTP response.</exception>
        /// <exception cref="JsonException">Thrown if <paramref name="body"/> cannot be serialized to JSON.</exception>
        /// <exception cref="InvalidUrlException">Thrown if the resolved URL is not a valid absolute HTTP/HTTPS address.</exception>
        /// <exception cref="System.OperationCanceledException">Thrown if <paramref name="ct"/> is cancelled.</exception>
        /// <exception cref="System.TimeoutException">Thrown if the request times out.</exception>
        UniTask PatchAsync(string url, object body, Dictionary<string, string> headers = null,
            Dictionary<string, string> queryParams = null, UrlType urlType = UrlType.Relative, int timeout = 10,
            CancellationToken ct = default);

        /// <summary>
        /// Sends an HTTP PATCH request with a JSON-serialized body and deserializes the JSON response to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type to deserialize the response into.
        /// Use <c>string</c> to receive the raw response text without deserialization.
        /// </typeparam>
        /// <param name="url">
        /// The relative or absolute endpoint URL. Relative paths are appended to <see cref="BaseUrl"/>.
        /// </param>
        /// <param name="body">
        /// The request payload. Serialized to JSON via Newtonsoft.Json before sending.
        /// Pass <c>null</c> to send a body-less request.
        /// </param>
        /// <param name="headers">
        /// Optional per-request headers. Merged with persistent headers; these take precedence on key conflicts.
        /// </param>
        /// <param name="queryParams">
        /// Optional query parameters appended to the URL.
        /// </param>
        /// <param name="urlType">Determines whether url should be appended to the base url.</param>
        /// <param name="timeout">Request timeout in seconds. Defaults to <c>10</c>.</param>
        /// <param name="ct">Token used to cancel the in-flight request.</param>
        /// <returns>The response body deserialized as <typeparamref name="T"/>.</returns>
        /// <exception cref="ApiException">
        /// Thrown on a non-2xx HTTP response, or if the server returns a body-less status code
        /// when a response body is expected.
        /// </exception>
        /// <exception cref="JsonException">Thrown if <paramref name="body"/> cannot be serialized, or if the response cannot be deserialized into <typeparamref name="T"/>.</exception>
        /// <exception cref="InvalidUrlException">Thrown if the resolved URL is not a valid absolute HTTP/HTTPS address.</exception>
        /// <exception cref="System.OperationCanceledException">Thrown if <paramref name="ct"/> is cancelled.</exception>
        /// <exception cref="System.TimeoutException">Thrown if the request times out.</exception>
        UniTask<T> PatchAsync<T>(string url, object body, Dictionary<string, string> headers = null,
            Dictionary<string, string> queryParams = null, UrlType urlType = UrlType.Relative, int timeout = 10,
            CancellationToken ct = default);

        // ── DELETE ────────────────────────────────────────────────────────────────

        /// <summary>
        /// Sends an HTTP DELETE request and discards the response body.
        /// </summary>
        /// <param name="url">
        /// The relative or absolute endpoint URL. Relative paths are appended to <see cref="BaseUrl"/>.
        /// </param>
        /// <param name="headers">
        /// Optional per-request headers. Merged with persistent headers; these take precedence on key conflicts.
        /// </param>
        /// <param name="queryParams">
        /// Optional query parameters appended to the URL.
        /// </param>
        /// <param name="urlType">Determines whether url should be appended to the base url.</param>
        /// <param name="timeout">Request timeout in seconds. Defaults to <c>10</c>.</param>
        /// <param name="ct">Token used to cancel the in-flight request.</param>
        /// <exception cref="ApiException">Thrown on a non-2xx HTTP response.</exception>
        /// <exception cref="InvalidUrlException">Thrown if the resolved URL is not a valid absolute HTTP/HTTPS address.</exception>
        /// <exception cref="System.OperationCanceledException">Thrown if <paramref name="ct"/> is cancelled.</exception>
        /// <exception cref="System.TimeoutException">Thrown if the request times out.</exception>
        UniTask DeleteAsync(string url, Dictionary<string, string> headers = null,
            Dictionary<string, string> queryParams = null, UrlType urlType = UrlType.Relative, int timeout = 10,
            CancellationToken ct = default);

        /// <summary>
        /// Sends an HTTP DELETE request and deserializes the JSON response body to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type to deserialize the response into.
        /// Use <c>string</c> to receive the raw response text without deserialization.
        /// </typeparam>
        /// <param name="url">
        /// The relative or absolute endpoint URL. Relative paths are appended to <see cref="BaseUrl"/>.
        /// </param>
        /// <param name="headers">
        /// Optional per-request headers. Merged with persistent headers; these take precedence on key conflicts.
        /// </param>
        /// <param name="queryParams">
        /// Optional query parameters appended to the URL.
        /// </param>
        /// <param name="urlType">Determines whether url should be appended to the base url.</param>
        /// <param name="timeout">Request timeout in seconds. Defaults to <c>10</c>.</param>
        /// <param name="ct">Token used to cancel the in-flight request.</param>
        /// <returns>The response body deserialized as <typeparamref name="T"/>.</returns>
        /// <exception cref="ApiException">
        /// Thrown on a non-2xx HTTP response, or if the server returns a body-less status code
        /// when a response body is expected.
        /// </exception>
        /// <exception cref="JsonException">Thrown if the response cannot be deserialized into <typeparamref name="T"/>.</exception>
        /// <exception cref="InvalidUrlException">Thrown if the resolved URL is not a valid absolute HTTP/HTTPS address.</exception>
        /// <exception cref="System.OperationCanceledException">Thrown if <paramref name="ct"/> is cancelled.</exception>
        /// <exception cref="System.TimeoutException">Thrown if the request times out.</exception>
        UniTask<T> DeleteAsync<T>(string url, Dictionary<string, string> headers = null,
            Dictionary<string, string> queryParams = null, UrlType urlType = UrlType.Relative, int timeout = 10,
            CancellationToken ct = default);

        // ── Sprite ────────────────────────────────────────────────────────────────

        /// <summary>
        /// Downloads an image from the given URL and returns it as a Unity <see cref="Sprite"/>.
        /// </summary>
        /// <remarks>
        /// Uses <c>UnityWebRequestTexture</c> internally. The sprite is created with a centered pivot
        /// (<c>0.5, 0.5</c>) and a rect covering the full texture dimensions.
        /// The response is not cached; use <see cref="GetCachedSpriteAsync"/> for repeated access to the same URL.
        /// </remarks>
        /// <param name="url">Relative or Absolute URL of the image resource.</param>
        /// <param name="headers">
        /// Optional per-request headers. Merged with persistent headers; these take precedence on key conflicts.
        /// </param>
        /// <param name="queryParams">Optional query parameters appended to the URL.</param>
        /// <param name="urlType">Determines whether url should be appended to the base url.</param>
        /// <param name="timeout">Request timeout in seconds. Defaults to <c>10</c>.</param>
        /// <param name="ct">Token used to cancel the in-flight request.</param>
        /// <returns>A <see cref="Sprite"/> created from the downloaded image data.</returns>
        /// <exception cref="ApiException">Thrown on a non-2xx HTTP response or a body-less response.</exception>
        /// <exception cref="BadSpriteException">Thrown if the downloaded data cannot be decoded into a valid texture.</exception>
        /// <exception cref="System.OperationCanceledException">Thrown if <paramref name="ct"/> is cancelled.</exception>
        /// <exception cref="System.TimeoutException">Thrown if the request times out.</exception>
        UniTask<Sprite> GetSpriteAsync(string url, Dictionary<string, string> headers = null,
            Dictionary<string, string> queryParams = null, UrlType urlType = UrlType.Relative, int timeout = 10,
            CancellationToken ct = default);

        /// <summary>
        /// Downloads an image and returns it as a Unity <see cref="Sprite"/>, serving from a local disk
        /// cache if a valid cached copy already exists.
        /// </summary>
        /// <remarks>
        /// <para>
        /// On a cache miss the image is downloaded, encoded as PNG, and saved to
        /// <c>Application.persistentDataPath/api_client_cache/</c> using a SHA-256 hash of the URL as the filename.
        /// A cache index file (<c>index.json</c>) records each entry's original URL and the time it was cached.
        /// </para>
        /// <para>
        /// On a cache hit the sprite is reconstructed from the locally stored PNG, avoiding a network round-trip.
        /// </para>
        /// <para>
        /// Cache reads and writes are protected by per-file and global semaphores, making concurrent calls
        /// for the same URL safe.
        /// </para>
        /// </remarks>
        /// <param name="url">Relative or Absolute URL of the image resource.</param>
        /// <param name="cacheDays">
        /// Number of days a cached entry is considered valid before a fresh download is triggered.
        /// Defaults to <c>14</c>.
        /// </param>
        /// <param name="headers">
        /// Optional per-request headers used when a network download is required.
        /// Merged with persistent headers; these take precedence on key conflicts.
        /// </param>
        /// <param name="queryParams">Optional query parameters appended to the URL on a cache miss.</param>
        /// <param name="urlType">Determines whether url should be appended to the base url.</param>
        /// <param name="timeout">Request timeout in seconds. Defaults to <c>10</c>.</param>
        /// <param name="ct">Token used to cancel the in-flight request or cache I/O.</param>
        /// <returns>A <see cref="Sprite"/> either loaded from cache or freshly downloaded.</returns>
        /// <exception cref="ApiException">Thrown on a non-2xx HTTP response or a body-less response during download.</exception>
        /// <exception cref="BadSpriteException">Thrown if the downloaded or cached data cannot be decoded into a valid texture.</exception>
        /// <exception cref="System.OperationCanceledException">Thrown if <paramref name="ct"/> is cancelled.</exception>
        /// <exception cref="System.TimeoutException">Thrown if the request times out.</exception>
        UniTask<Sprite> GetCachedSpriteAsync(string url, int cacheDays = 14, Dictionary<string, string> headers = null,
            Dictionary<string, string> queryParams = null, UrlType urlType = UrlType.Relative, int timeout = 10,
            CancellationToken ct = default);

        // ── AudioClip ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Downloads an audio file from the given URL and returns it as a Unity <see cref="AudioClip"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If <paramref name="audioType"/> is <see cref="AudioType.UNKNOWN"/> (the default), the audio format
        /// is inferred from the URL file extension. Supported extensions and their mappings are:
        /// <c>.mp3</c> / <c>.mpeg</c> → <see cref="AudioType.MPEG"/>,
        /// <c>.ogg</c> → <see cref="AudioType.OGGVORBIS"/>,
        /// <c>.aac</c> → <see cref="AudioType.ACC"/>,
        /// <c>.wav</c> → <see cref="AudioType.WAV"/>,
        /// <c>.aiff</c> / <c>.aif</c> → <see cref="AudioType.AIFF"/>.
        /// If the extension is unrecognised, <see cref="AudioType.UNKNOWN"/> is passed to
        /// <c>UnityWebRequestMultimedia</c>, which may or may not succeed depending on the platform.
        /// </para>
        /// <para>
        /// The response is not cached; use <see cref="GetCachedAudioClipAsync"/> for repeated access.
        /// </para>
        /// </remarks>
        /// <param name="url">Relative or Absolute URL of the image resource.</param>
        /// <param name="audioType">
        /// The audio format hint. Pass <see cref="AudioType.UNKNOWN"/> to let the client auto-detect
        /// from the URL file extension. Defaults to <see cref="AudioType.UNKNOWN"/>.
        /// </param>
        /// <param name="headers">
        /// Optional per-request headers. Merged with persistent headers; these take precedence on key conflicts.
        /// </param>
        /// <param name="queryParams">Optional query parameters appended to the URL.</param>
        /// <param name="urlType">Determines whether url should be appended to the base url.</param>
        /// <param name="timeout">Request timeout in seconds. Defaults to <c>10</c>.</param>
        /// <param name="ct">Token used to cancel the in-flight request.</param>
        /// <returns>An <see cref="AudioClip"/> loaded from the downloaded audio data.</returns>
        /// <exception cref="ApiException">Thrown on a non-2xx HTTP response or a body-less response.</exception>
        /// <exception cref="BadAudioClipException">Thrown if the downloaded data cannot be decoded into a valid <see cref="AudioClip"/>.</exception>
        /// <exception cref="System.OperationCanceledException">Thrown if <paramref name="ct"/> is cancelled.</exception>
        /// <exception cref="System.TimeoutException">Thrown if the request times out.</exception>
        UniTask<AudioClip> GetAudioClipAsync(string url, AudioType audioType = AudioType.UNKNOWN,
            Dictionary<string, string> headers = null, Dictionary<string, string> queryParams = null,
            UrlType urlType = UrlType.Relative, int timeout = 10, CancellationToken ct = default);

        /// <summary>
        /// Downloads an audio file and returns it as a Unity <see cref="AudioClip"/>, serving from a local
        /// disk cache if a valid cached copy already exists.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The URL <b>must</b> include a recognisable file extension (e.g. <c>.mp3</c>, <c>.wav</c>).
        /// The extension is used both to name the cache file on disk and, when
        /// <paramref name="audioType"/> is <see cref="AudioType.UNKNOWN"/>, to determine the audio format.
        /// An <see cref="InvalidUrlException"/> is thrown immediately if no extension can be extracted.
        /// </para>
        /// <para>
        /// On a cache miss the raw audio bytes are downloaded and saved to
        /// <c>Application.persistentDataPath/api_client_cache/</c> using a SHA-256 hash of the URL as
        /// the base filename with the original extension preserved. The clip is then loaded from the
        /// saved file via <c>UnityWebRequestMultimedia</c>.
        /// </para>
        /// <para>
        /// On a cache hit the clip is loaded directly from disk, avoiding a network round-trip.
        /// Cache reads and writes are protected by per-file and global semaphores, making concurrent
        /// calls for the same URL safe.
        /// </para>
        /// </remarks>
        /// <param name="url">
        /// Relative or Absolute URL of the audio resource. Must contain a file extension.
        /// </param>
        /// <param name="audioType">
        /// The audio format hint. Pass <see cref="AudioType.UNKNOWN"/> to auto-detect from the URL
        /// file extension. Defaults to <see cref="AudioType.UNKNOWN"/>.
        /// </param>
        /// <param name="cacheDays">
        /// Number of days a cached entry is considered valid before a fresh download is triggered.
        /// Defaults to <c>14</c>.
        /// </param>
        /// <param name="headers">
        /// Optional per-request headers used when a network download is required.
        /// Merged with persistent headers; these take precedence on key conflicts.
        /// </param>
        /// <param name="queryParams">Optional query parameters appended to the URL on a cache miss.</param>
        /// <param name="urlType">Determines whether url should be appended to the base url.</param>
        /// <param name="timeout">Request timeout in seconds. Defaults to <c>10</c>.</param>
        /// <param name="ct">Token used to cancel the in-flight request or cache I/O.</param>
        /// <returns>An <see cref="AudioClip"/> either loaded from cache or freshly downloaded.</returns>
        /// <exception cref="InvalidUrlException">
        /// Thrown immediately if <paramref name="url"/> does not contain a file extension.
        /// </exception>
        /// <exception cref="ApiException">Thrown on a non-2xx HTTP response during download.</exception>
        /// <exception cref="BadAudioClipException">Thrown if the cached or downloaded data cannot be decoded into a valid <see cref="AudioClip"/>.</exception>
        /// <exception cref="System.OperationCanceledException">Thrown if <paramref name="ct"/> is cancelled.</exception>
        /// <exception cref="System.TimeoutException">Thrown if the request times out.</exception>
        UniTask<AudioClip> GetCachedAudioClipAsync(string url, AudioType audioType = AudioType.UNKNOWN,
            int cacheDays = 14, Dictionary<string, string> headers = null,
            Dictionary<string, string> queryParams = null, UrlType urlType = UrlType.Relative, int timeout = 10,
            CancellationToken ct = default);
    }
}