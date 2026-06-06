using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
using System.Collections;
using System.Text;

public class ApiClientHeadersTest : ApiClientTestBase
{
    [UnityTest]
    public IEnumerator GetAsyncNoBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        Client.AddHeader(Header_Auth, Value_Auth);
        await TestRequestHeaders(Client.GetAsync(MockServer.ServerUrl + "api/v1/headers", headers: OneOffHeaders),
            Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator GetAsyncWithBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        Client.AddHeader(Header_Auth, Value_Auth);
        await TestRequestHeaders(
            Client.GetAsync<string>(MockServer.ServerUrl + "api/v1/headers", headers: OneOffHeaders),
            Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator PostAsyncNoBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        Client.AddHeader(Header_Auth, Value_Auth);
        await TestRequestHeaders(
            Client.PostAsync(MockServer.ServerUrl + "api/v1/headers", string.Empty, headers: OneOffHeaders),
            Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator PostAsyncWithBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        Client.AddHeader(Header_Auth, Value_Auth);
        await TestRequestHeaders(
            Client.PostAsync<string>(MockServer.ServerUrl + "api/v1/headers", string.Empty, headers: OneOffHeaders),
            Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator PutAsyncNoBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        Client.AddHeader(Header_Auth, Value_Auth);
        await TestRequestHeaders(
            Client.PutAsync(MockServer.ServerUrl + "api/v1/headers", string.Empty, headers: OneOffHeaders),
            Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator PutAsyncWithBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        Client.AddHeader(Header_Auth, Value_Auth);
        await TestRequestHeaders(
            Client.PutAsync<string>(MockServer.ServerUrl + "api/v1/headers", string.Empty, headers: OneOffHeaders),
            Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator PatchAsyncNoBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        Client.AddHeader(Header_Auth, Value_Auth);
        await TestRequestHeaders(
            Client.PatchAsync(MockServer.ServerUrl + "api/v1/headers", string.Empty, headers: OneOffHeaders),
            Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator PatchAsyncWithBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        Client.AddHeader(Header_Auth, Value_Auth);
        await TestRequestHeaders(
            Client.PatchAsync<string>(MockServer.ServerUrl + "api/v1/headers", string.Empty, headers: OneOffHeaders),
            Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator DeleteAsyncNoBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        Client.AddHeader(Header_Auth, Value_Auth);
        await TestRequestHeaders(Client.DeleteAsync(MockServer.ServerUrl + "api/v1/headers", headers: OneOffHeaders),
            Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator DeleteAsyncWithBody_MergesPersistentAndCustomHeadersCorrectly() =>
        UniTask.ToCoroutine(async () =>
        {
            Client.AddHeader(Header_Auth, Value_Auth);
            await TestRequestHeaders(
                Client.DeleteAsync<string>(MockServer.ServerUrl + "api/v1/headers", headers: OneOffHeaders),
                Encoding.UTF8.GetBytes("Hello"));
        });

    [UnityTest]
    public IEnumerator GetSpriteAsync_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        Client.AddHeader(Header_Auth, Value_Auth);
        await TestRequestHeaders(
            Client.GetSpriteAsync(MockServer.ServerUrl + "api/v1/headers.png", headers: OneOffHeaders), RealPngBytes);
    });

    [UnityTest]
    public IEnumerator GetCachedSpriteAsync_MergesPersistentAndCustomHeadersCorrectly() =>
        UniTask.ToCoroutine(async () =>
        {
            Client.AddHeader(Header_Auth, Value_Auth);
            await TestRequestHeaders(
                Client.GetCachedSpriteAsync(MockServer.ServerUrl + "api/v1/headers.png", headers: OneOffHeaders),
                RealPngBytes);
        });

    [UnityTest]
    public IEnumerator GetAudioClipAsync_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        Client.AddHeader(Header_Auth, Value_Auth);
        await TestRequestHeaders(
            Client.GetAudioClipAsync(MockServer.ServerUrl + "api/v1/headers.mp3", headers: OneOffHeaders),
            RealMp3Bytes);
    });

    [UnityTest]
    public IEnumerator GetCachedAudioClipAsync_MergesPersistentAndCustomHeadersCorrectly() =>
        UniTask.ToCoroutine(async () =>
        {
            Client.AddHeader(Header_Auth, Value_Auth);
            await TestRequestHeaders(
                Client.GetCachedAudioClipAsync(MockServer.ServerUrl + "api/v1/headers.mp3", headers: OneOffHeaders),
                RealMp3Bytes);
        });

    // ── Helpers ───────────────────────────────────────────────────────────────

    private const string Header_Auth = "Authorization";
    private const string Value_Auth = "Bearer my-token";
    private const string Header_ReqId = "X-Request-Id";
    private const string Value_ReqId = "0123456789";

    private static Dictionary<string, string> OneOffHeaders =>
        new() { { Header_ReqId, Value_ReqId } }; // OneOff Headers

    private async UniTask TestRequestHeaders(UniTask task, byte[] responseBytes)
    {
        // Arrange
        var spyHookTriggered = false;

        MockServer.OnRequestReceived = request =>
        {
            spyHookTriggered = true;
            Assert.AreEqual(Value_Auth, request.Headers[Header_Auth]);
            Assert.AreEqual(Value_ReqId, request.Headers[Header_ReqId]);
        };

        MockServer.ResponseStatusCode = 200;
        MockServer.ResponseBytes = responseBytes;

        // Act
        await task;

        // Assert
        Assert.IsTrue(spyHookTriggered, "The MockServer spy hook was never triggered.");
    }
}