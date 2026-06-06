using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
using System.Collections;
using System.Text;

public class ApiClientQueryParamsTest : ApiClientTestBase
{
    [UnityTest]
    public IEnumerator GetAsyncNoBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        await TestRequestQueryParameters(
            Client.GetAsync(MockServer.ServerUrl + "api/v1/headers", queryParams: ReadyQueryParameters),
            Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator GetAsyncWithBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        await TestRequestQueryParameters(
            Client.GetAsync<string>(MockServer.ServerUrl + "api/v1/headers", queryParams: ReadyQueryParameters),
            Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator PostAsyncNoBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        await TestRequestQueryParameters(
            Client.PostAsync(MockServer.ServerUrl + "api/v1/headers", string.Empty,
                queryParams: ReadyQueryParameters), Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator PostAsyncWithBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        await TestRequestQueryParameters(
            Client.PostAsync<string>(MockServer.ServerUrl + "api/v1/headers", string.Empty,
                queryParams: ReadyQueryParameters), Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator PutAsyncNoBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        await TestRequestQueryParameters(
            Client.PutAsync(MockServer.ServerUrl + "api/v1/headers", string.Empty,
                queryParams: ReadyQueryParameters), Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator PutAsyncWithBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        await TestRequestQueryParameters(
            Client.PutAsync<string>(MockServer.ServerUrl + "api/v1/headers", string.Empty,
                queryParams: ReadyQueryParameters), Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator PatchAsyncNoBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        await TestRequestQueryParameters(
            Client.PatchAsync(MockServer.ServerUrl + "api/v1/headers", string.Empty,
                queryParams: ReadyQueryParameters), Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator PatchAsyncWithBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        await TestRequestQueryParameters(
            Client.PatchAsync<string>(MockServer.ServerUrl + "api/v1/headers", string.Empty,
                queryParams: ReadyQueryParameters), Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator DeleteAsyncNoBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        await TestRequestQueryParameters(
            Client.DeleteAsync(MockServer.ServerUrl + "api/v1/headers", queryParams: ReadyQueryParameters),
            Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator DeleteAsyncWithBody_MergesPersistentAndCustomHeadersCorrectly() =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestQueryParameters(
                Client.DeleteAsync<string>(MockServer.ServerUrl + "api/v1/headers",
                    queryParams: ReadyQueryParameters), Encoding.UTF8.GetBytes("Hello"));
        });

    [UnityTest]
    public IEnumerator GetSpriteAsync_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        await TestRequestQueryParameters(
            Client.GetSpriteAsync(MockServer.ServerUrl + "api/v1/headers.png", queryParams: ReadyQueryParameters),
            RealPngBytes);
    });

    [UnityTest]
    public IEnumerator GetCachedSpriteAsync_MergesPersistentAndCustomHeadersCorrectly() =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestQueryParameters(
                Client.GetCachedSpriteAsync(MockServer.ServerUrl + "api/v1/headers.png",
                    queryParams: ReadyQueryParameters), RealPngBytes);
        });

    [UnityTest]
    public IEnumerator GetAudioClipAsync_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        await TestRequestQueryParameters(
            Client.GetAudioClipAsync(MockServer.ServerUrl + "api/v1/headers.mp3",
                queryParams: ReadyQueryParameters), RealMp3Bytes);
    });

    [UnityTest]
    public IEnumerator GetCachedAudioClipAsync_MergesPersistentAndCustomHeadersCorrectly() =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestQueryParameters(
                Client.GetCachedAudioClipAsync(MockServer.ServerUrl + "api/v1/headers.mp3",
                    queryParams: ReadyQueryParameters), RealMp3Bytes);
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

    private async UniTask TestRequestQueryParameters(UniTask task, byte[] responseBytes)
    {
        // Arrange
        var spyHookTriggered = false;

        MockServer.OnRequestReceived = request =>
        {
            spyHookTriggered = true;
            Assert.AreEqual(Value_Size, request.QueryString[QueryParam_Size]);
            Assert.AreEqual(Value_Id, request.QueryString[QueryParam_Id]);
        };

        MockServer.ResponseStatusCode = 200;
        MockServer.ResponseBytes = responseBytes;

        // Act
        await task;

        // Assert
        Assert.IsTrue(spyHookTriggered, "The MockServer spy hook was never triggered.");
    }
}