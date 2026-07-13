using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEditor;
using UnityEngine;
using System;

namespace ApiClientLib.Tests.Editor
{
    [Serializable]
    internal sealed class ExamplePostData
    {
        public int id { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public int userId { get; set; }
    }

    public static class ApiClientExampleRequests
    {
        [MenuItem("Window/ApiClient/Run Example Requests")]
        public static void RunExampleRequestsMenu()
        {
            RunRequestsAsync().Forget();
        }

        private static async UniTaskVoid RunRequestsAsync()
        {
            Debug.Log("<color=cyan><b>[ApiClient Example]</b> Starting example requests sequence...</color>");

            var client = new ApiClient();

            // Invalidate cache first to ensure the first cached request is a cache miss
            try
            {
                Debug.Log("<b>[ApiClient Example]</b> Invalidating cache to reset cache test state...");
                await client.InvalidateCacheAsync();
                Debug.Log("<color=green><b>[ApiClient Example]</b> Cache invalidated successfully.</color>");
            }
            catch (Exception ex)
            {
                Debug.LogError($"<b>[ApiClient Example]</b> Failed to invalidate cache: {ex.Message}");
            }

            var ct = CancellationToken.None;

            // Define query params and headers to use as a mixture
            var customHeaders = new Dictionary<string, string>
            {
                { "Authorization", "Bearer example-test-token-1234" },
                { "X-Custom-Client-Header", "ApiClientEditorExample" }
            };

            var queryParams = new Dictionary<string, string>
            {
                { "editor_run", "true" },
                { "timestamp", DateTime.UtcNow.Ticks.ToString() }
            };

            // ─── 1. GET (Fire-and-forget / body-less) ───
            try
            {
                Debug.Log("<b>[1/19] GET (Fire-and-forget)</b> calling...");
                int code = await client.GetAsync(
                    "https://jsonplaceholder.typicode.com/posts/1",
                    customHeaders: customHeaders,
                    queryParams: queryParams,
                    urlType: UrlType.Absolute,
                    ct: ct
                );
                Debug.Log($"<b>[1/19] GET (Fire-and-forget)</b> finished with status code: {code}");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"<b>[1/19] GET (Fire-and-forget)</b> threw exception: {ex.Message}");
            }

            // ─── 2. GET (Response processing) ───
            try
            {
                Debug.Log("<b>[2/19] GET (Response processing)</b> calling...");
                var response = await client.GetAsync<ExamplePostData>(
                    "https://jsonplaceholder.typicode.com/posts/2",
                    customHeaders: customHeaders,
                    queryParams: queryParams,
                    urlType: UrlType.Absolute,
                    ct: ct
                );
                Debug.Log(
                    $"<b>[2/19] GET (Response processing)</b> finished with status code: {response.StatusCode}, title: '{response.Data?.title}'");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"<b>[2/19] GET (Response processing)</b> threw exception: {ex.Message}");
            }

            // ─── 3. POST (Fire-and-forget / body-less) ───
            try
            {
                Debug.Log("<b>[3/19] POST (Fire-and-forget)</b> calling...");
                var body = new ExamplePostData
                {
                    title = "Test Title 1",
                    body = "Test Body Content 1",
                    userId = 10
                };
                int code = await client.PostAsync(
                    "https://jsonplaceholder.typicode.com/posts",
                    body: body,
                    customHeaders: customHeaders,
                    queryParams: queryParams,
                    urlType: UrlType.Absolute,
                    ct: ct
                );
                Debug.Log($"<b>[3/19] POST (Fire-and-forget)</b> finished with status code: {code}");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"<b>[3/19] POST (Fire-and-forget)</b> threw exception: {ex.Message}");
            }

            // ─── 4. POST (Response processing) ───
            try
            {
                Debug.Log("<b>[4/19] POST (Response processing)</b> calling...");
                var body = new ExamplePostData
                {
                    title = "Test Title 2",
                    body = "Test Body Content 2",
                    userId = 20
                };
                var response = await client.PostAsync<ExamplePostData>(
                    "https://jsonplaceholder.typicode.com/posts",
                    body: body,
                    customHeaders: customHeaders,
                    queryParams: queryParams,
                    urlType: UrlType.Absolute,
                    ct: ct
                );
                Debug.Log(
                    $"<b>[4/19] POST (Response processing)</b> finished with status code: {response.StatusCode}, returned ID: {response.Data?.id}");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"<b>[4/19] POST (Response processing)</b> threw exception: {ex.Message}");
            }

            // ─── 5. PUT (Fire-and-forget / body-less) ───
            try
            {
                Debug.Log("<b>[5/19] PUT (Fire-and-forget)</b> calling...");
                var body = new ExamplePostData
                {
                    id = 1,
                    title = "Updated Title 1",
                    body = "Updated Body Content 1",
                    userId = 1
                };
                int code = await client.PutAsync(
                    "https://jsonplaceholder.typicode.com/posts/1",
                    body: body,
                    customHeaders: customHeaders,
                    queryParams: queryParams,
                    urlType: UrlType.Absolute,
                    ct: ct
                );
                Debug.Log($"<b>[5/19] PUT (Fire-and-forget)</b> finished with status code: {code}");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"<b>[5/19] PUT (Fire-and-forget)</b> threw exception: {ex.Message}");
            }

            // ─── 6. PUT (Response processing) ───
            try
            {
                Debug.Log("<b>[6/19] PUT (Response processing)</b> calling...");
                var body = new ExamplePostData
                {
                    id = 2,
                    title = "Updated Title 2",
                    body = "Updated Body Content 2",
                    userId = 2
                };
                var response = await client.PutAsync<ExamplePostData>(
                    "https://jsonplaceholder.typicode.com/posts/2",
                    body: body,
                    customHeaders: customHeaders,
                    queryParams: queryParams,
                    urlType: UrlType.Absolute,
                    ct: ct
                );
                Debug.Log(
                    $"<b>[6/19] PUT (Response processing)</b> finished with status code: {response.StatusCode}, title: '{response.Data?.title}'");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"<b>[6/19] PUT (Response processing)</b> threw exception: {ex.Message}");
            }

            // ─── 7. PATCH (Fire-and-forget / body-less) ───
            try
            {
                Debug.Log("<b>[7/19] PATCH (Fire-and-forget)</b> calling...");
                var body = new ExamplePostData
                {
                    title = "Patched Title 1"
                };
                int code = await client.PatchAsync(
                    "https://jsonplaceholder.typicode.com/posts/1",
                    body: body,
                    customHeaders: customHeaders,
                    queryParams: queryParams,
                    urlType: UrlType.Absolute,
                    ct: ct
                );
                Debug.Log($"<b>[7/19] PATCH (Fire-and-forget)</b> finished with status code: {code}");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"<b>[7/19] PATCH (Fire-and-forget)</b> threw exception: {ex.Message}");
            }

            // ─── 8. PATCH (Response processing) ───
            try
            {
                Debug.Log("<b>[8/19] PATCH (Response processing)</b> calling...");
                var body = new ExamplePostData
                {
                    title = "Patched Title 2"
                };
                var response = await client.PatchAsync<ExamplePostData>(
                    "https://jsonplaceholder.typicode.com/posts/2",
                    body: body,
                    customHeaders: customHeaders,
                    queryParams: queryParams,
                    urlType: UrlType.Absolute,
                    ct: ct
                );
                Debug.Log(
                    $"<b>[8/19] PATCH (Response processing)</b> finished with status code: {response.StatusCode}, title: '{response.Data?.title}'");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"<b>[8/19] PATCH (Response processing)</b> threw exception: {ex.Message}");
            }

            // ─── 9. DELETE (Fire-and-forget / body-less) ───
            try
            {
                Debug.Log("<b>[9/19] DELETE (Fire-and-forget)</b> calling...");
                int code = await client.DeleteAsync(
                    "https://jsonplaceholder.typicode.com/posts/1",
                    customHeaders: customHeaders,
                    queryParams: queryParams,
                    urlType: UrlType.Absolute,
                    ct: ct
                );
                Debug.Log($"<b>[9/19] DELETE (Fire-and-forget)</b> finished with status code: {code}");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"<b>[9/19] DELETE (Fire-and-forget)</b> threw exception: {ex.Message}");
            }

            // ─── 10. DELETE (Response processing) ───
            try
            {
                Debug.Log("<b>[10/19] DELETE (Response processing)</b> calling...");
                var response = await client.DeleteAsync<ExamplePostData>(
                    "https://jsonplaceholder.typicode.com/posts/2",
                    customHeaders: customHeaders,
                    queryParams: queryParams,
                    urlType: UrlType.Absolute,
                    ct: ct
                );
                Debug.Log(
                    $"<b>[10/19] DELETE (Response processing)</b> finished with status code: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"<b>[10/19] DELETE (Response processing)</b> threw exception: {ex.Message}");
            }

            // ─── 11. Get Sprite (Uncached) ───
            try
            {
                Debug.Log("<b>[11/19] Get Sprite (Uncached)</b> calling...");
                var spriteResponse = await client.GetSpriteAsync(
                    "https://placehold.co/150x150.png",
                    customHeaders: customHeaders,
                    urlType: UrlType.Absolute,
                    ct: ct
                );
                Debug.Log(
                    $"<b>[11/19] Get Sprite (Uncached)</b> finished with status code: {spriteResponse.StatusCode}, Sprite is null? {spriteResponse.Sprite == null}");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"<b>[11/19] Get Sprite (Uncached)</b> threw exception: {ex.Message}");
            }

            // ─── 12. Get Sprite (Cached - Cache Miss) ───
            try
            {
                Debug.Log("<b>[12/19] Get Sprite (Cached - Cache Miss)</b> calling...");
                var spriteResponse = await client.GetCachedSpriteAsync(
                    "https://placehold.co/120x120.png",
                    cacheDays: 7,
                    customHeaders: customHeaders,
                    urlType: UrlType.Absolute,
                    ct: ct
                );
                Debug.Log(
                    $"<b>[12/19] Get Sprite (Cached - Cache Miss)</b> finished with status code: {spriteResponse.StatusCode}, Sprite is null? {spriteResponse.Sprite == null}");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"<b>[12/19] Get Sprite (Cached - Cache Miss)</b> threw exception: {ex.Message}");
            }

            // ─── 13. Get Sprite (Cached - Cache Hit) ───
            try
            {
                Debug.Log("<b>[13/19] Get Sprite (Cached - Cache Hit)</b> calling again...");
                var spriteResponse = await client.GetCachedSpriteAsync(
                    "https://placehold.co/120x120.png",
                    cacheDays: 7,
                    customHeaders: customHeaders,
                    urlType: UrlType.Absolute,
                    ct: ct
                );
                Debug.Log(
                    $"<b>[13/19] Get Sprite (Cached - Cache Hit)</b> finished with status code: {spriteResponse.StatusCode} (should be -1), Sprite is null? {spriteResponse.Sprite == null}");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"<b>[13/19] Get Sprite (Cached - Cache Hit)</b> threw exception: {ex.Message}");
            }

            // ─── 14. Get AudioClip (Uncached) ───
            try
            {
                Debug.Log("<b>[14/19] Get AudioClip (Uncached)</b> calling...");
                var audioResponse = await client.GetAudioClipAsync(
                    "https://ccrma.stanford.edu/~jos/mp3/pno-cs.mp3",
                    audioType: AudioType.MPEG,
                    customHeaders: customHeaders,
                    urlType: UrlType.Absolute,
                    ct: ct
                );
                Debug.Log(
                    $"<b>[14/19] Get AudioClip (Uncached)</b> finished with status code: {audioResponse.StatusCode}, AudioClip is null? {audioResponse.AudioClip == null}");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"<b>[14/19] Get AudioClip (Uncached)</b> threw exception: {ex.Message}");
            }

            // ─── 15. Get AudioClip (Cached - Cache Miss) ───
            try
            {
                Debug.Log("<b>[15/19] Get AudioClip (Cached - Cache Miss)</b> calling...");
                var audioResponse = await client.GetCachedAudioClipAsync(
                    "https://ccrma.stanford.edu/~jos/mp3/pno-cs.mp3",
                    audioType: AudioType.MPEG,
                    cacheDays: 7,
                    customHeaders: customHeaders,
                    urlType: UrlType.Absolute,
                    ct: ct
                );
                Debug.Log(
                    $"<b>[15/19] Get AudioClip (Cached - Cache Miss)</b> finished with status code: {audioResponse.StatusCode}, AudioClip is null? {audioResponse.AudioClip == null}");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"<b>[15/19] Get AudioClip (Cached - Cache Miss)</b> threw exception: {ex.Message}");
            }

            // ─── 16. Get AudioClip (Cached - Cache Hit) ───
            try
            {
                Debug.Log("<b>[16/19] Get AudioClip (Cached - Cache Hit)</b> calling again...");
                var audioResponse = await client.GetCachedAudioClipAsync(
                    "https://ccrma.stanford.edu/~jos/mp3/pno-cs.mp3",
                    audioType: AudioType.MPEG,
                    cacheDays: 7,
                    customHeaders: customHeaders,
                    urlType: UrlType.Absolute,
                    ct: ct
                );
                Debug.Log(
                    $"<b>[16/19] Get AudioClip (Cached - Cache Hit)</b> finished with status code: {audioResponse.StatusCode} (should be -1), AudioClip is null? {audioResponse.AudioClip == null}");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"<b>[16/19] Get AudioClip (Cached - Cache Hit)</b> threw exception: {ex.Message}");
            }

            // ─── 17. Failure: Non-2xx HTTP code (404 Not Found) ───
            try
            {
                Debug.Log("<b>[17/19] GET (Fail: 404 Not Found)</b> calling...");
                int code = await client.GetAsync(
                    "https://httpbin.org/status/404",
                    customHeaders: customHeaders,
                    urlType: UrlType.Absolute,
                    ct: ct
                );
                Debug.Log(
                    $"<b>[17/19] GET (Fail: 404 Not Found)</b> finished with status code: {code} (unexpected success)");
            }
            catch (Exception ex)
            {
                Debug.Log(
                    $"<color=orange><b>[17/19] GET (Fail: 404 Not Found)</b> successfully threw expected exception: {ex.Message}</color>");
            }

            // ─── 18. Failure: Invalid JSON response (image deserialization failure) ───
            try
            {
                Debug.Log(
                    "<b>[18/19] GET (Fail: Invalid JSON)</b> calling and trying to parse PNG as ExamplePostData JSON...");
                var response = await client.GetAsync<ExamplePostData>(
                    "https://placehold.co/150x150.png",
                    customHeaders: customHeaders,
                    urlType: UrlType.Absolute,
                    ct: ct
                );
                Debug.Log(
                    $"<b>[18/19] GET (Fail: Invalid JSON)</b> finished with status code: {response.StatusCode} (unexpected success)");
            }
            catch (Exception ex)
            {
                Debug.Log(
                    $"<color=orange><b>[18/19] GET (Fail: Invalid JSON)</b> successfully threw expected exception: {ex.Message}</color>");
            }

            // ─── 19. Failure: Invalid URL format ───
            try
            {
                Debug.Log("<b>[19/19] GET (Fail: Invalid URL)</b> calling with invalid URL scheme...");
                int code = await client.GetAsync(
                    "invalid-url-scheme://test",
                    customHeaders: customHeaders,
                    urlType: UrlType.Absolute,
                    ct: ct
                );
                Debug.Log(
                    $"<b>[19/19] GET (Fail: Invalid URL)</b> finished with status code: {code} (unexpected success)");
            }
            catch (Exception ex)
            {
                Debug.Log(
                    $"<color=orange><b>[19/19] GET (Fail: Invalid URL)</b> successfully threw expected exception: {ex.Message}</color>");
            }

            Debug.Log("<color=cyan><b>[ApiClient Example]</b> Completed all example requests.</color>");
        }
    }
}