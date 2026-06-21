# Changelog

All notable changes to this project will be documented in this file.

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
