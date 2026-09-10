# Changelog

All notable changes to this project will be documented in this file.

## [1.6.0]

### Added
- `OnRequestStatusCodeResolved` event on `IApiClient` — triggers whenever an HTTP request resolves or finishes, passing the resulting HTTP status code as an integer (`-1` for cache hits, `0` for connection/network failures).

### Breaking Changes
- **Interface Signature Modifications:** Added `OnRequestStatusCodeResolved` event callback to the `IApiClient` interface. Custom mocks or third-party implementations of `IApiClient` must implement this new member to compile.

## [1.5.0]

### Added
- `OnRequestCompleted` event on `IApiClient` — triggers whenever an HTTP request completes (successfully or with an error), passing a detailed `ApiEventData` payload.
- `InstanceID` property on `IApiClient` — a unique integer ID generated for each client instance to help correlate logs and events.
- `ApiClientLoggerWindow` Editor window — a centralized graphical logging console inside the Unity Editor for inspecting and tracking all API requests, headers, bodies, response times, status codes, and exceptions.
- `ArgumentNullException` validation and documentation on all public API methods when passing a `null` URL or key.

### Changed
- **Internal Refactoring:** Extracted client core logic into separate manager classes (`BaseUrlManager`, `CacheManager`, and `HeaderManager`) and static handlers (`JsonRequestHandling`, `SpriteRequestHandling`, `AudioClipRequestHandling`, `CachedRequestHandling`, `UrlValidation`, and `Logging`) to improve modularity and clean up `ApiClient.cs`.
- **Audio Clip Error Handling:** Standardized error behavior when requesting audio clips; `GetAudioClipAsync` and `GetCachedAudioClipAsync` now throw an `InvalidUrlException` if the request URL does not contain a file extension.
- **Cache Invalidation:** Updated the `InvalidateCacheAsync` signature in the `IApiClient` interface to have a default parameter value (`CancellationToken ct = default`) to match the concrete class.

### Breaking Changes
- **Interface Signature Modifications:** Added `InstanceID` property and `OnRequestCompleted` event callback to the `IApiClient` interface. Custom mocks or third-party implementations of `IApiClient` must implement these new members to compile.
- **Return Type Changes on Request Methods:**
  - Fire-and-forget REST calls (`GetAsync`, `PostAsync`, `PutAsync`, `PatchAsync`, `DeleteAsync`) now return `UniTask<int>` (representing the HTTP status code) instead of `UniTask`.
  - Generic deserialized responses (`GetAsync<T>`, `PostAsync<T>`, `PutAsync<T>`, `PatchAsync<T>`, `DeleteAsync<T>`) now return `UniTask<ApiResponse<T>>` instead of `UniTask<T>`. Access the deserialized model via `response.Data` and the status code via `response.StatusCode`.
  - Sprite download methods (`GetSpriteAsync`, `GetCachedSpriteAsync`) now return `UniTask<SpriteResponse>` instead of `UniTask<Sprite>`. Access the sprite via `response.Sprite`.
  - Audio clip download methods (`GetAudioClipAsync`, `GetCachedAudioClipAsync`) now return `UniTask<AudioClipResponse>` instead of `UniTask<AudioClip>`. Access the clip via `response.AudioClip`.
- **Audio Request URL Constraints:** Both `GetAudioClipAsync` and `GetCachedAudioClipAsync` now strictly validate the presence of a file extension in the URL parameter. Calls using URLs without file extensions will now throw an `InvalidUrlException` immediately.

## [1.1.0]

### Added
- `InvalidateCacheAsync(CancellationToken ct = default)` — clears the entire disk cache directory and resets the cache index, forcing fresh downloads for `GetCachedSpriteAsync` and `GetCachedAudioClipAsync` regardless of `cacheDays`. Safe to call concurrently with in-flight cached requests.
- `UrlType` enum (`Relative`, `Absolute`) and a corresponding `urlType` parameter on every request method (`GetAsync`, `PostAsync`, `PutAsync`, `PatchAsync`, `DeleteAsync` and their generic/typed overloads, plus `GetSpriteAsync`, `GetCachedSpriteAsync`, `GetAudioClipAsync`, `GetCachedAudioClipAsync`), controlling whether `BaseUrl` is prepended to the request URL.

### Changed
- **Breaking:** REST methods (`GetAsync`, `PostAsync`, `PutAsync`, `PatchAsync`, `DeleteAsync`) previously always prepended `BaseUrl`. They now default to `urlType: UrlType.Relative`, which preserves the old behavior, but the URL resolution rule itself changed from "always prepend if `BaseUrl` is set" to "prepend only when `urlType` is `Relative`" — pass `UrlType.Absolute` to opt out per request.
- **Breaking:** Asset download methods (`GetSpriteAsync`, `GetCachedSpriteAsync`, `GetAudioClipAsync`, `GetCachedAudioClipAsync`) previously always ignored `BaseUrl` and required a fully-qualified URL. They now default to `urlType: UrlType.Relative` and will prepend `BaseUrl` unless called with `UrlType.Absolute`. Existing callers passing absolute URLs to these methods must now explicitly pass `urlType: UrlType.Absolute` to preserve prior behavior.

## [1.0.0]

Initial release.

### Added
- Full REST support — GET, POST, PUT, PATCH, DELETE, with fire-and-forget and generic typed-response overloads.
- Automatic JSON serialization/deserialization via Newtonsoft.Json.
- Persistent headers applied to every request, with per-request header and query parameter overrides.
- `GetSpriteAsync` / `GetCachedSpriteAsync` for downloading and disk-caching Unity `Sprite` assets.
- `GetAudioClipAsync` / `GetCachedAudioClipAsync` for downloading and disk-caching Unity `AudioClip` assets, with auto-detection of `AudioType` from file extension.
- `CancellationToken` and configurable `timeout` support on every method.
- Interface-first design (`IApiClient`) for mocking and dependency injection.
