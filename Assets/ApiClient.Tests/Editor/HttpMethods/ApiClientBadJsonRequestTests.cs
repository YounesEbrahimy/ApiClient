using System.Runtime.Serialization;
using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using System.Collections;
using NUnit.Framework;
using ApiClientLib;
using System;

public class ApiClientBadJsonRequestTests : ApiClientTestBase
{
    [UnityTest]
    public IEnumerator PostAsyncNoBody_WhenBadJsonRequest_ThrowsJsonException() => UniTask.ToCoroutine(async () =>
    {
        await TestBadJsonRequest(Client.PostAsync("api/v1/test", new InvalidJsonObject()));
    });

    [UnityTest]
    public IEnumerator PostAsyncWithBody_WhenBadJsonRequest_ThrowsJsonException() => UniTask.ToCoroutine(async () =>
    {
        await TestBadJsonRequest(Client.PostAsync<TestPayload>("api/v1/test", new InvalidJsonObject()));
    });

    [UnityTest]
    public IEnumerator PutAsyncNoBody_WhenBadJsonRequest_ThrowsJsonException() => UniTask.ToCoroutine(async () =>
    {
        await TestBadJsonRequest(Client.PutAsync("api/v1/test", new InvalidJsonObject()));
    });

    [UnityTest]
    public IEnumerator PutAsyncWithBody_WhenBadJsonRequest_ThrowsJsonException() => UniTask.ToCoroutine(async () =>
    {
        await TestBadJsonRequest(Client.PutAsync<TestPayload>("api/v1/test", new InvalidJsonObject()));
    });

    [UnityTest]
    public IEnumerator PatchAsyncNoBody_WhenBadJsonRequest_ThrowsJsonException() => UniTask.ToCoroutine(async () =>
    {
        await TestBadJsonRequest(Client.PatchAsync("api/v1/test", new InvalidJsonObject()));
    });

    [UnityTest]
    public IEnumerator PatchAsyncWithBody_WhenBadJsonRequest_ThrowsJsonException() => UniTask.ToCoroutine(async () =>
    {
        await TestBadJsonRequest(Client.PatchAsync<TestPayload>("api/v1/test", new InvalidJsonObject()));
    });

    // ── Helpers ───────────────────────────────────────────────────────────────

    [Serializable]
    private class InvalidJsonObject : ISerializable
    {
        public void GetObjectData(SerializationInfo info, StreamingContext context)
            => throw new Exception("Boom!");
    }

    private static async UniTask TestBadJsonRequest(UniTask requestTask)
    {
        try
        {
            // Act
            await requestTask;
            Assert.Fail("Expected JsonException, but no exception was thrown.");
        }
        catch (JsonException ex)
        {
            Assert.Pass(); // This is what we want
        }
        catch (Exception e)
        {
            // Assert, Only exceptions of type JsonException must be thrown
            Assert.Fail($"Unexpected exception: {e.Message}");
        }
    }
}