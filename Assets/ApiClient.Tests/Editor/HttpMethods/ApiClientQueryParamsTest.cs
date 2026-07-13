using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
using System.Collections;
using System.Text;
using System;

public class ApiClientQueryParamsTest : ApiClientTestBase
{
    [UnityTest]
    public IEnumerator GetAsyncNoBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        await TestRequestQueryParameters(MockServer,
            () => Client.GetAsync("api/v1/headers", queryParams: ReadyQueryParameters),
            Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator GetAsyncWithBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        await TestRequestQueryParameters(MockServer,
            () => Client.GetAsync<string>("api/v1/headers", queryParams: ReadyQueryParameters),
            Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator PostAsyncNoBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        await TestRequestQueryParameters(MockServer,
            () => Client.PostAsync("api/v1/headers", string.Empty, queryParams: ReadyQueryParameters),
            Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator PostAsyncWithBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        await TestRequestQueryParameters(MockServer,
            () => Client.PostAsync<string>("api/v1/headers", string.Empty, queryParams: ReadyQueryParameters),
            Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator PutAsyncNoBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        await TestRequestQueryParameters(MockServer,
            () => Client.PutAsync("api/v1/headers", string.Empty, queryParams: ReadyQueryParameters),
            Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator PutAsyncWithBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        await TestRequestQueryParameters(MockServer,
            () => Client.PutAsync<string>("api/v1/headers", string.Empty, queryParams: ReadyQueryParameters),
            Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator PatchAsyncNoBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        await TestRequestQueryParameters(MockServer,
            () => Client.PatchAsync("api/v1/headers", string.Empty, queryParams: ReadyQueryParameters),
            Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator PatchAsyncWithBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        await TestRequestQueryParameters(MockServer,
            () => Client.PatchAsync<string>("api/v1/headers", string.Empty, queryParams: ReadyQueryParameters),
            Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator DeleteAsyncNoBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        await TestRequestQueryParameters(MockServer,
            () => Client.DeleteAsync("api/v1/headers", queryParams: ReadyQueryParameters),
            Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator DeleteAsyncWithBody_MergesPersistentAndCustomHeadersCorrectly() =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestQueryParameters(MockServer,
                () => Client.DeleteAsync<string>("api/v1/headers", queryParams: ReadyQueryParameters),
                Encoding.UTF8.GetBytes("Hello"));
        });

    [UnityTest]
    public IEnumerator GetSpriteAsync_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        await TestRequestQueryParameters(MockServer,
            () => Client.GetSpriteAsync("api/v1/headers.png", queryParams: ReadyQueryParameters), RealPngBytes);
    });

    [UnityTest]
    public IEnumerator GetCachedSpriteAsync_MergesPersistentAndCustomHeadersCorrectly() =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestQueryParameters(MockServer,
                () => Client.GetCachedSpriteAsync("api/v1/headers.png", queryParams: ReadyQueryParameters),
                RealPngBytes);
        });

    [UnityTest]
    public IEnumerator GetAudioClipAsync_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        await TestRequestQueryParameters(MockServer,
            () => Client.GetAudioClipAsync("api/v1/headers.mp3", queryParams: ReadyQueryParameters), RealMp3Bytes);
    });

    [UnityTest]
    public IEnumerator GetCachedAudioClipAsync_MergesPersistentAndCustomHeadersCorrectly() =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestQueryParameters(MockServer,
                () => Client.GetCachedAudioClipAsync("api/v1/headers.mp3", queryParams: ReadyQueryParameters),
                RealMp3Bytes);
        });

    // ── Helpers ───────────────────────────────────────────────────────────────

    private const string QueryParam_Size = "size";
    private const string Value_Size = "large";
    private const string QueryParam_Id = "id";
    private const string Value_Id = "abc123";

    private static Dictionary<string, string> ReadyQueryParameters => new()
    {
        { QueryParam_Id, Value_Id },
        { QueryParam_Size, Value_Size },
    }; // Ready Query Parameters

    private static async UniTask TestRequestQueryParameters(MockHttpServer server, Func<UniTask> requestFunc,
        byte[] responseBytes)
    {
        // Arrange
        var spyHookTriggered = false;

        server.OnRequestReceived = request =>
        {
            spyHookTriggered = true;
            Assert.AreEqual(Value_Size, request.QueryString[QueryParam_Size]);
            Assert.AreEqual(Value_Id, request.QueryString[QueryParam_Id]);
        };

        server.ResponseStatusCode = 200;
        server.ResponseBytes = responseBytes;

        // Act
        await requestFunc();

        // Assert
        Assert.IsTrue(spyHookTriggered, "The MockServer spy hook was never triggered.");
    }
}