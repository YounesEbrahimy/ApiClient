# ApiClient

A Unity HTTP client built on `UnityWebRequest` + `UniTask`. Handles JSON requests, sprite downloading, and disk-based sprite caching. Supports per-environment base URLs.

---

## Dependencies

- [UniTask](https://github.com/Cysharp/UniTask) — async/await on Unity's main thread without `Task`
- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) — JSON serialization
- `UnityEngine.Networking` — built-in, no install needed

---

## Setup
```csharp
// No base URL (you pass full URLs every time)
var client = new ApiClient();

// With base URL — good for env switching
var client = new ApiClient("https://api.myapp.com/v1");

// Change it later (e.g. after reading config)
client.SetBaseUrl("https://staging.myapp.com/v1");

For DI, register `IApiClient` and inject it wherever you need it.

---

## Base URL behavior

- `null` base URL throws `ArgumentNullException` — both in constructor and `SetBaseUrl`.
- Empty string `""` is valid — effectively disables the prefix.
- Trailing slash is normalized automatically: `"https://api.myapp.com/v1/"` and `"https://api.myapp.com/v1"` behave identically.
- Base URL **only affects** `GetAsync`, `PostAsync`, `PutAsync`, `PatchAsync`, `DeleteAsync`.
- `GetSpriteAsync` and `GetCachedSpriteAsync` always use the URL you pass as-is — CDN URLs don't need a base.

---

## HTTP Methods

All methods are `async/await` via `UniTask`. They throw `ApiException` on non-2xx responses.

csharp
// GET — deserializes response JSON into T
var user = await client.GetAsync<UserDto>("/users/42");

// POST / PUT / PATCH — body is serialized to JSON automatically
var created = await client.PostAsync<UserDto>("/users", new { name = "Ali" });
await client.PutAsync<UserDto>("/users/42", updatedUser);
await client.PatchAsync<UserDto>("/users/42", new { name = "Reza" });

// DELETE — with or without a return type
await client.DeleteAsync("/users/42");
var result = await client.DeleteAsync<DeleteResultDto>("/users/42");

### Headers

csharp
// Persistent — sent with every request
client.AddHeader("Authorization", "Bearer <token>");
client.AddHeader("X-App-Version", "2.1.0");

// Per-request — merged with persistent headers, per-request wins on conflict
var data = await client.GetAsync<Dto>("/endpoint", headers: new Dictionary<string, string>
{
{ "X-Request-ID", Guid.NewGuid().ToString() }
});

// Remove or clear persistent headers
client.RemoveHeader("Authorization");
client.ClearHeaders();

### Timeout

Default is 10 seconds. Pass `timeout:` to override (value is in **seconds**).

csharp
var data = await client.GetAsync<Dto>("/slow-endpoint", timeout: 30);

### Cancellation

csharp
var cts = new CancellationTokenSource();
var data = await client.GetAsync<Dto>("/endpoint", ct: cts.Token);

// Cancel from anywhere
cts.Cancel();

---

## Sprites

### Direct download (no cache)

csharp
var sprite = await client.GetSpriteAsync("https://cdn.example.com/avatar.png");
image.sprite = sprite;

### Cached download

Saves to `Application.persistentDataPath/sprite_cache/` as PNG. Uses a JSON index to track URLs and cache timestamps.

csharp
// Default: 14-day cache
var sprite = await client.GetCachedSpriteAsync("https://cdn.example.com/avatar.png");

// Custom TTL
var sprite = await client.GetCachedSpriteAsync("https://cdn.example.com/avatar.png", cacheDays: 7);

Cache hit path: read bytes from disk → decode → return sprite. No network call.  
Cache miss or expired: download → save to disk → update index → return sprite.

---

## Error handling

csharp
try
{
var user = await client.GetAsync<UserDto>("/users/99");
}
catch (ApiException ex)
{
Debug.LogError($"HTTP {(int)ex.StatusCode}: {ex.RawBody}");
}
catch (OperationCanceledException)
{
// request was cancelled
}

---

## Concurrency notes

- Same-URL sprite cache requests are serialized via per-URL `SemaphoreSlim` — no duplicate downloads, no file corruption.
- `_cacheIndex` (the on-disk index dictionary) is protected by a separate lock — safe for concurrent access across different URLs.
- `_fileLocks` grows indefinitely for the lifetime of the client instance. Not a problem for typical game sessions, but worth knowing if you're caching thousands of unique URLs.

---

## Limitations / known tradeoffs

- Sprite caching re-encodes the downloaded texture to PNG. If the source is already a compressed format (ASTC, ETC2), you lose that compression. File sizes may be larger than the originals.
- `Texture2D` objects created by `Sprite.Create` are not automatically destroyed. You own the lifetime — destroy them when done if memory matters.
- No retry logic. Wrap calls yourself if you need it.
- No request queuing or rate limiting.
