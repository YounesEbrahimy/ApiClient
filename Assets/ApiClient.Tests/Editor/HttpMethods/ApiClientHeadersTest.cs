using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
using System.Collections;
using System.Text;
using System;

public class ApiClientHeadersTest : ApiClientTestBase
{
    [UnityTest]
    public IEnumerator GetAsyncNoBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        Client.AddHeader(Header_Auth, Value_Auth);
        Client.AddHeader(Header_ReqId,
            Value_ReqId_Persistent); // OneOff headers must take priority over persistent headers.
        await TestRequestHeaders(() => Client.GetAsync("api/v1/headers", customHeaders: OneOffHeaders),
            Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator GetAsyncWithBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        Client.AddHeader(Header_Auth, Value_Auth);
        Client.AddHeader(Header_ReqId,
            Value_ReqId_Persistent); // OneOff headers must take priority over persistent headers.
        await TestRequestHeaders(
            () => Client.GetAsync<string>("api/v1/headers", customHeaders: OneOffHeaders),
            Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator PostAsyncNoBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        Client.AddHeader(Header_Auth, Value_Auth);
        Client.AddHeader(Header_ReqId,
            Value_ReqId_Persistent); // OneOff headers must take priority over persistent headers.
        await TestRequestHeaders(
            () => Client.PostAsync("api/v1/headers", string.Empty, customHeaders: OneOffHeaders),
            Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator PostAsyncWithBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        Client.AddHeader(Header_Auth, Value_Auth);
        Client.AddHeader(Header_ReqId,
            Value_ReqId_Persistent); // OneOff headers must take priority over persistent headers.
        await TestRequestHeaders(
            () => Client.PostAsync<string>("api/v1/headers", string.Empty, customHeaders: OneOffHeaders),
            Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator PutAsyncNoBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        Client.AddHeader(Header_Auth, Value_Auth);
        Client.AddHeader(Header_ReqId,
            Value_ReqId_Persistent); // OneOff headers must take priority over persistent headers.
        await TestRequestHeaders(
            () => Client.PutAsync("api/v1/headers", string.Empty, customHeaders: OneOffHeaders),
            Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator PutAsyncWithBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        Client.AddHeader(Header_Auth, Value_Auth);
        Client.AddHeader(Header_ReqId,
            Value_ReqId_Persistent); // OneOff headers must take priority over persistent headers.
        await TestRequestHeaders(
            () => Client.PutAsync<string>("api/v1/headers", string.Empty, customHeaders: OneOffHeaders),
            Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator PatchAsyncNoBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        Client.AddHeader(Header_Auth, Value_Auth);
        Client.AddHeader(Header_ReqId,
            Value_ReqId_Persistent); // OneOff headers must take priority over persistent headers.
        await TestRequestHeaders(
            () => Client.PatchAsync("api/v1/headers", string.Empty, customHeaders: OneOffHeaders),
            Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator PatchAsyncWithBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        Client.AddHeader(Header_Auth, Value_Auth);
        Client.AddHeader(Header_ReqId,
            Value_ReqId_Persistent); // OneOff headers must take priority over persistent headers.
        await TestRequestHeaders(
            () => Client.PatchAsync<string>("api/v1/headers", string.Empty, customHeaders: OneOffHeaders),
            Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator DeleteAsyncNoBody_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        Client.AddHeader(Header_Auth, Value_Auth);
        Client.AddHeader(Header_ReqId,
            Value_ReqId_Persistent); // OneOff headers must take priority over persistent headers.
        await TestRequestHeaders(() => Client.DeleteAsync("api/v1/headers", customHeaders: OneOffHeaders),
            Encoding.UTF8.GetBytes("Hello"));
    });

    [UnityTest]
    public IEnumerator DeleteAsyncWithBody_MergesPersistentAndCustomHeadersCorrectly() =>
        UniTask.ToCoroutine(async () =>
        {
            Client.AddHeader(Header_Auth, Value_Auth);
            Client.AddHeader(Header_ReqId,
                Value_ReqId_Persistent); // OneOff headers must take priority over persistent headers.
            await TestRequestHeaders(
                () => Client.DeleteAsync<string>("api/v1/headers", customHeaders: OneOffHeaders),
                Encoding.UTF8.GetBytes("Hello"));
        });

    [UnityTest]
    public IEnumerator GetSpriteAsync_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        Client.AddHeader(Header_Auth, Value_Auth);
        Client.AddHeader(Header_ReqId,
            Value_ReqId_Persistent); // OneOff headers must take priority over persistent headers.
        await TestRequestHeaders(
            () => Client.GetSpriteAsync("api/v1/headers.png", customHeaders: OneOffHeaders), RealPngBytes);
    });

    [UnityTest]
    public IEnumerator GetCachedSpriteAsync_MergesPersistentAndCustomHeadersCorrectly() =>
        UniTask.ToCoroutine(async () =>
        {
            Client.AddHeader(Header_Auth, Value_Auth);
            Client.AddHeader(Header_ReqId,
                Value_ReqId_Persistent); // OneOff headers must take priority over persistent headers.
            await TestRequestHeaders(
                () => Client.GetCachedSpriteAsync("api/v1/headers.png", customHeaders: OneOffHeaders),
                RealPngBytes);
        });

    [UnityTest]
    public IEnumerator GetAudioClipAsync_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        Client.AddHeader(Header_Auth, Value_Auth);
        Client.AddHeader(Header_ReqId,
            Value_ReqId_Persistent); // OneOff headers must take priority over persistent headers.
        await TestRequestHeaders(
            () => Client.GetAudioClipAsync("api/v1/headers.mp3", customHeaders: OneOffHeaders),
            RealMp3Bytes);
    });

    [UnityTest]
    public IEnumerator GetCachedAudioClipAsync_MergesPersistentAndCustomHeadersCorrectly() =>
        UniTask.ToCoroutine(async () =>
        {
            Client.AddHeader(Header_Auth, Value_Auth);
            Client.AddHeader(Header_ReqId,
                Value_ReqId_Persistent); // OneOff headers must take priority over persistent headers.
            await TestRequestHeaders(
                () => Client.GetCachedAudioClipAsync("api/v1/headers.mp3", customHeaders: OneOffHeaders),
                RealMp3Bytes);
        });

    // ── Helpers ───────────────────────────────────────────────────────────────

    private const string Header_Auth = "Authorization";
    private const string Value_Auth = "Bearer my-token";
    private const string Header_ReqId = "X-Request-Id";
    private const string Value_ReqId = "0123456789";
    private const string Value_ReqId_Persistent = "PersistentId";

    private static Dictionary<string, string> OneOffHeaders =>
        new() { { Header_ReqId, Value_ReqId } }; // OneOff Headers

    private async UniTask TestRequestHeaders(Func<UniTask> requestFunc, byte[] responseBytes)
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
        await requestFunc();

        // Assert
        Assert.IsTrue(spyHookTriggered, "The MockServer spy hook was never triggered.");
    }
}