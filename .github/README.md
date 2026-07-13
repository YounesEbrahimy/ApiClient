# 🌐 ApiClient
<div align=center>

![Unity Tests](https://github.com/YounesEbrahimy/ApiClient/actions/workflows/test.yml/badge.svg)
[![License: MIT](https://img.shields.io/badge/License-MIT-orange.svg)](https://opensource.org/licenses/MIT)
[![Releases](https://img.shields.io/github/release/younesebrahimy/apiclient.svg)](https://github.com/YounesEbrahimy/ApiClient/releases)
[![Unity](https://img.shields.io/badge/Unity-6000.3+-yellow.svg)](https://unity3d.com/pt/get-unity/download/archive)

</div>
<table>
  <tr>
    <td width=125>
      <img src="images/logo.png" alt="Logo">
    </td>
    <td valign="middle">
      <p>A lightweight, async HTTP client for Unity built on top of <strong><ins>UnityWebRequest</ins></strong> and <a href="https://github.com/Cysharp/UniTask">UniTask</a>. It handles JSON REST requests, image and audio asset downloads, and persistent disk caching — all without blocking the main thread.</p>
    </td>
  </tr>
</table>

---

## 🔥 Features

- Full REST support — GET, POST, PUT, PATCH, DELETE
- Generic typed responses with automatic JSON deserialization via [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
- Persistent headers that apply to every request automatically
- Per-request header and query parameter overrides
- Download Unity `Sprite` and `AudioClip` assets directly from URLs
- Disk-based caching for sprites and audio clips with configurable expiry
- `CancellationToken` support on every method for clean lifecycle management
- Interface-first design (`IApiClient`) for easy mocking and dependency injection

---

## ⚙️ Requirements

- Unity **6000.3** or newer
- [UniTask](https://github.com/Cysharp/UniTask) — `com.cysharp.unitask`
- [Newtonsoft.Json for Unity](https://docs.unity3d.com/Packages/com.unity.nuget.newtonsoft-json@latest) — `com.unity.nuget.newtonsoft-json`

---

## 💾 Installation

### Via Unity Package Manager (Git URL)

1. Open **Window → Package Manager**
2. Click the **+** button → **Add package from git URL ...**
3. Enter:
   ```
   https://github.com/YounesEbrahimy/ApiClient.git?path=/Assets/ApiClient/#1.5.0
   ```

### Manual

Download the latest .unitypackage file form [Releases](https://github.com/YounesEbrahimy/ApiClient/releases), And import it in your project.

> Make sure UniTask and Newtonsoft.Json are already installed before importing.

---

## 🪄 Quick Start

```csharp
// Create a client with a base URL
IApiClient client = new ApiClient("https://api.example.com");

// Add an auth header that will be sent with every request
client.AddHeader("Authorization", "Bearer your-token-here");

// Make a typed GET request
var response = await client.GetAsync<UserDto>("users/42");
var user = response.Data;
Debug.Log($"User loaded: {user.Name} (Status: {response.StatusCode})");
```

---

## 📄 API Reference

### Constructors

```csharp
// Parameterless — base URL can be set later via SetBaseUrl()
IApiClient client = new ApiClient();

// With a base URL set upfront
IApiClient client = new ApiClient("https://api.example.com");
```

---

### Base URL

#### `string BaseUrl { get; }`

Returns the current base URL. A trailing slash is always enforced on the stored value. Returns an empty string if none has been set.

#### `void SetBaseUrl(string baseUrl)`

Sets the base URL prepended to all relative request URLs. A trailing slash is appended automatically if absent. Pass an empty string to clear it.

```csharp
client.SetBaseUrl("https://api.example.com/v2");
Console.WriteLine(client.BaseUrl); // "https://api.example.com/v2/"
```

> **URL Resolution:** Every request method accepts a `urlType` parameter (`UrlType.Relative` by default). When `urlType` is `UrlType.Relative`, the `url` parameter is treated as a relative path and appended to `BaseUrl`. When `urlType` is `UrlType.Absolute`, `BaseUrl` is ignored and `url` is used as-is, so it must be a fully-qualified `http://` or `https://` address. An `InvalidUrlException` is thrown if the resolved URL is not a valid absolute HTTP/HTTPS address.

---

### Persistent Headers

Persistent headers are automatically included in every outgoing request. Per-request `headers` parameters are merged on top and take precedence when keys conflict.

#### `Dictionary<string, string> Headers { get; }`

Returns all currently registered persistent headers.

#### `void AddHeader(string key, string value)`

Adds or overwrites a persistent header.

#### `void RemoveHeader(string key)`

Removes a persistent header by key. No-op if the key does not exist.

#### `void ClearHeaders()`

Removes all persistent headers.

```csharp
client.AddHeader("Authorization", "Bearer token123");
client.AddHeader("X-App-Version", "2.0.1");

// Override for a single request only — use the per-request headers parameter instead
var response = await client.GetAsync<DataDto>("data", headers: new Dictionary<string, string>
{
    { "Authorization", "Bearer different-token" }
});

client.RemoveHeader("X-App-Version");
client.ClearHeaders();
```

---

### Properties & Events

#### `int InstanceID { get; }`

A unique integer identifier for the `ApiClient` instance. Useful for correlating events and logs.

#### `event Action<ApiEventData> OnRequestCompleted`

Event triggered when any HTTP request completes (successfully or with an error). The event payload <see cref="ApiEventData"/> contains detailed information about URLs, headers, payload, status codes, duration, and exceptions. (Note: For cache hits, the `StatusCode` is `-1` and the request duration is near 0.)

```csharp
client.OnRequestCompleted += (ApiEventData eventData) =>
{
    Debug.Log($"[{eventData.Method}] {eventData.URL} completed in {eventData.Duration}s. Success: {eventData.Success}, Status: {eventData.StatusCode}");
    if (!eventData.Success && eventData.Exception != null)
    {
        Debug.LogError($"Error: {eventData.ErrorMessage}");
    }
};
```

---

### Cache Control

#### `UniTask InvalidateCacheAsync(CancellationToken ct = default)`

Deletes the entire disk cache directory and its contents, then recreates it and resets the cache index. Use this to force fresh downloads for `GetCachedSpriteAsync` and `GetCachedAudioClipAsync` regardless of `cacheDays`. Safe to call while other cached requests are in flight — it waits for in-progress cache reads/writes to finish before clearing.

```csharp
await client.InvalidateCacheAsync();
```

---

### HTTP Methods

All HTTP methods share these common parameters:

| Parameter | Type | Default | Description |
|---|---|---|---|
| `url` | `string` | — | Relative or absolute endpoint URL |
| `headers` | `Dictionary<string, string>` | `null` | Per-request headers, merged over persistent headers |
| `queryParams` | `Dictionary<string, string>` | `null` | Key-value pairs appended to the URL as a query string |
| `urlType` | `UrlType` | `UrlType.Relative` | Whether `url` is appended to `BaseUrl` (`Relative`) or used as-is (`Absolute`) |
| `timeout` | `int` | `10` | Request timeout in seconds |
| `ct` | `CancellationToken` | `default` | Token to cancel the request |

All request methods return wrapper classes containing the response details. Fire-and-forget methods return `UniTask<int>` where the `int` represents the HTTP status code. Generic response-returning methods return `UniTask<ApiResponse<T>>` containing both the status code and the deserialized body. Passing `string` as `T` returns the raw response text in `ApiResponse<string>.Data` without JSON deserialization.

---

#### GET

```csharp
// Fire-and-forget (discards response body, returns status code)
UniTask<int> GetAsync(string url, ...)

// Returns response wrapper with deserialized response body
UniTask<ApiResponse<T>> GetAsync<T>(string url, ...)
```

```csharp
// Simple typed GET
var response = await client.GetAsync<ProductDto>("products/7");
var product = response.Data;

// With query parameters
var responseList = await client.GetAsync<List<ProductDto>>("products", queryParams: new Dictionary<string, string>
{
    { "category", "electronics" },
    { "page", "1" }
});
var results = responseList.Data;

// Raw string response
var responseRaw = await client.GetAsync<string>("health");
var raw = responseRaw.Data;
```

---

#### POST

```csharp
// Fire-and-forget (returns status code)
UniTask<int> PostAsync(string url, object body, ...)

// Returns response wrapper with deserialized response body
UniTask<ApiResponse<T>> PostAsync<T>(string url, object body, ...)
```

The `body` is serialized to JSON automatically. Pass `null` for a body-less request.

```csharp
var newUser = new CreateUserRequest { Name = "Alice", Email = "alice@example.com" };

// No response body expected (returns status code)
int statusCode = await client.PostAsync("users", newUser);

// With response body
var response = await client.PostAsync<UserDto>("users", newUser);
var created = response.Data;
Console.WriteLine(created.Id);
```

---

#### PUT

```csharp
UniTask<int> PutAsync(string url, object body, ...)
UniTask<ApiResponse<T>> PutAsync<T>(string url, object body, ...)
```

```csharp
var update = new UpdateUserRequest { Name = "Alice Smith" };
var response = await client.PutAsync<UserDto>("users/42", update);
var updated = response.Data;
```

---

#### PATCH

```csharp
UniTask<int> PatchAsync(string url, object body, ...)
UniTask<ApiResponse<T>> PatchAsync<T>(string url, object body, ...)
```

```csharp
// Partial update — only send the fields that changed (returns status code)
var patch = new { Email = "new@example.com" };
int statusCode = await client.PatchAsync("users/42", patch);
```

---

#### DELETE

```csharp
UniTask<int> DeleteAsync(string url, ...)
UniTask<ApiResponse<T>> DeleteAsync<T>(string url, ...)
```

```csharp
// No response body (returns status code)
int statusCode = await client.DeleteAsync("users/42");

// With response body (e.g. API returns the deleted resource)
var response = await client.DeleteAsync<UserDto>("users/42");
var deleted = response.Data;
```

---

### Asset Downloads

#### `UniTask<SpriteResponse> GetSpriteAsync(string url, ...)`

Downloads an image and returns it as a `SpriteResponse` containing the `Sprite`. The sprite is created with a centered pivot (`0.5, 0.5`) covering the full texture dimensions. The response is not cached — use `GetCachedSpriteAsync` if the same URL will be accessed more than once.

```csharp
var response = await client.GetSpriteAsync("https://cdn.example.com/avatar/42.png");
avatarImage.sprite = response.Sprite;
```

---

#### `UniTask<SpriteResponse> GetCachedSpriteAsync(string url, int cacheDays = 14, ...)`

Same as `GetSpriteAsync` but stores the downloaded image to disk as a PNG and serves it from the local cache on subsequent calls, avoiding redundant network requests. 

> **Cache Hit Status:** On a cache hit, no network request is sent and the returned `SpriteResponse.StatusCode` is `-1`.

| Parameter | Default | Description |
|---|---|---|
| `cacheDays` | `14` | Days before the cached file is considered stale and re-downloaded |

Cache files are stored in `Application.persistentDataPath/api_client_cache/` using a SHA-256 hash of the URL as the filename. A cache index (`index.json`) tracks each entry's original URL and cache timestamp. Concurrent calls for the same URL are safe — reads and writes are protected by per-file and global semaphores.

```csharp
// First call downloads and caches; subsequent calls within 14 days load from disk
var response = await client.GetCachedSpriteAsync("https://cdn.example.com/avatar/42.png");
var sprite = response.Sprite;

// Custom expiry
var responseCustom = await client.GetCachedSpriteAsync("https://cdn.example.com/banner.png", cacheDays: 7);
var spriteCustom = responseCustom.Sprite;
```

---

#### `UniTask<AudioClipResponse> GetAudioClipAsync(string url, AudioType audioType = AudioType.UNKNOWN, ...)`

Downloads an audio file and returns it as an `AudioClipResponse` containing the `AudioClip`. The response is not cached — use `GetCachedAudioClipAsync` for repeated access.

When `audioType` is `AudioType.UNKNOWN` (the default), the format is auto-detected from the URL file extension:

| Extension | AudioType |
|---|---|
| `.mp3`, `.mpeg` | `MPEG` |
| `.ogg` | `OGGVORBIS` |
| `.aac` | `ACC` |
| `.wav` | `WAV` |
| `.aiff`, `.aif` | `AIFF` |

```csharp
// Auto-detect format from URL extension
var response = await client.GetAudioClipAsync("https://cdn.example.com/sfx/explosion.mp3");
var clip = response.AudioClip;

// Explicit format
var responseTheme = await client.GetAudioClipAsync("https://cdn.example.com/music/theme", AudioType.OGGVORBIS);
var clipTheme = responseTheme.AudioClip;
```

---

#### `UniTask<AudioClipResponse> GetCachedAudioClipAsync(string url, AudioType audioType = AudioType.UNKNOWN, int cacheDays = 14, ...)`

Same as `GetAudioClipAsync` but caches the raw audio bytes to disk. Subsequent calls within the expiry window load the clip from the local file, skipping the network entirely.

> **Important:** The URL **must** include a file extension (e.g. `.mp3`, `.wav`). This is used to name the cache file and, when `audioType` is `UNKNOWN`, to detect the format. An `InvalidUrlException` is thrown immediately if no extension is found.

> **Cache Hit Status:** On a cache hit, no network request is sent and the returned `AudioClipResponse.StatusCode` is `-1`.

```csharp
var response = await client.GetCachedAudioClipAsync("https://cdn.example.com/music/theme.ogg");
audioSource.clip = response.AudioClip;
audioSource.Play();

// Custom cache duration
var responseClick = await client.GetCachedAudioClipAsync(
    "https://cdn.example.com/sfx/click.wav",
    cacheDays: 30
);
var clipClick = responseClick.AudioClip;
```

---

### Cancellation and Timeouts

Every method accepts a `CancellationToken` and an integer `timeout` (in seconds). Cancelling via token throws `OperationCanceledException`. Exceeding the timeout throws `System.TimeoutException`.

```csharp
using var cts = new CancellationTokenSource();

// Cancel after 5 seconds
cts.CancelAfter(TimeSpan.FromSeconds(5));

try
{
    var data = await client.GetAsync<DataDto>("slow-endpoint", ct: cts.Token);
}
catch (OperationCanceledException)
{
    Debug.Log("Request was cancelled.");
}

// Or rely on the built-in timeout parameter (throws TimeoutException)
try
{
    var data = await client.GetAsync<DataDto>("slow-endpoint", timeout: 3);
}
catch (TimeoutException)
{
    Debug.Log("Request timed out.");
}
```

---

### Error Handling

| Exception | When thrown |
|---|---|
| `ApiException` | Non-2xx HTTP response, or a body-less response (e.g. 204) when a response body was expected |
| `JsonException` | Request body could not be serialized, or response body could not be deserialized |
| `InvalidUrlException` | The resolved URL is not a valid absolute HTTP/HTTPS address, or a cached audio URL has no file extension |
| `BadSpriteException` | Downloaded image data could not be decoded into a valid texture |
| `BadAudioClipException` | Downloaded or cached audio data could not be decoded into a valid `AudioClip` |
| `System.TimeoutException` | Request exceeded the `timeout` duration |
| `OperationCanceledException` | Request was cancelled via `CancellationToken` |

```csharp
try
{
    var user = await client.GetAsync<UserDto>("users/99", timeout: 5, ct: this.GetCancellationTokenOnDestroy());
}
catch (ApiException e)
{
    Debug.LogError($"HTTP {e.StatusCode}: {e.Message}");
}
catch (JsonException e)
{
    Debug.LogError($"Deserialization failed: {e.Message}");
}
catch (TimeoutException)
{
    Debug.LogWarning("Request timed out.");
}
catch (OperationCanceledException)
{
    // Normal — object was destroyed or token was cancelled
}
```

---

## 🖥️ Diagnostics & Logging (Unity Editor)

ApiClient features a custom Unity Editor window to inspect and log all network activities.

### ApiClient Logger Window

To open the logger window in Unity, go to **Window → API Client Logger**. This window provides:
- A real-time log list of all outgoing and incoming requests.
- Correlation of logs by specific client `InstanceID` and timestamp.
- Detailed inspection panel displaying request headers, query parameters, request body, response code, response headers, response body, duration, and exceptions.

---

## 🛡️ License

[MIT](../LICENSE)
