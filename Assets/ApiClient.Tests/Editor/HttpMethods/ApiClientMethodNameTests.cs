using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using System.Collections;
using NUnit.Framework;

public class ApiClientMethodNameTests : ApiClientTestBase
{
    [UnityTest]
    public IEnumerator GetAsyncNoBody_CheckMethodName() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodName(
            MockServer,
            Client.GetAsync(MockServer.ServerUrl + "api/test"),
            "GET"
        );
    });

    [UnityTest]
    public IEnumerator GetAsyncWithBody_CheckMethodName() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodName(
            MockServer,
            Client.GetAsync<string>(MockServer.ServerUrl + "api/test"),
            "GET"
        );
    });

    [UnityTest]
    public IEnumerator PostAsyncNoBody_CheckMethodName() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodName(
            MockServer,
            Client.PostAsync(MockServer.ServerUrl + "api/test", string.Empty),
            "POST"
        );
    });

    [UnityTest]
    public IEnumerator PostAsyncWithBody_CheckMethodName() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodName(
            MockServer,
            Client.PostAsync<string>(MockServer.ServerUrl + "api/test", string.Empty),
            "POST"
        );
    });

    [UnityTest]
    public IEnumerator PutAsyncNoBody_CheckMethodName() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodName(
            MockServer,
            Client.PutAsync(MockServer.ServerUrl + "api/test", string.Empty),
            "PUT"
        );
    });

    [UnityTest]
    public IEnumerator PutAsyncWithBody_CheckMethodName() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodName(
            MockServer,
            Client.PutAsync<string>(MockServer.ServerUrl + "api/test", string.Empty),
            "PUT"
        );
    });

    [UnityTest]
    public IEnumerator PatchAsyncNoBody_CheckMethodName() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodName(
            MockServer,
            Client.PatchAsync(MockServer.ServerUrl + "api/test", string.Empty),
            "PATCH"
        );
    });

    [UnityTest]
    public IEnumerator PatchAsyncWithBody_CheckMethodName() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodName(
            MockServer,
            Client.PatchAsync<string>(MockServer.ServerUrl + "api/test", string.Empty),
            "PATCH"
        );
    });

    [UnityTest]
    public IEnumerator DeleteAsyncNoBody_CheckMethodName() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodName(
            MockServer,
            Client.DeleteAsync(MockServer.ServerUrl + "api/test"),
            "DELETE"
        );
    });

    [UnityTest]
    public IEnumerator DeleteAsyncWithBody_CheckMethodName() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodName(
            MockServer,
            Client.DeleteAsync<string>(MockServer.ServerUrl + "api/test"),
            "DELETE"
        );
    });

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static async UniTask TestHttpRequestMethodName(MockHttpServer server, UniTask requestTask, string method)
    {
        // Arrange
        var spyHookTriggered = false;
        server.ResponseStatusCode = 200;
        server.ResponseObject = "Hello";
        server.OnRequestReceived = request =>
        {
            spyHookTriggered = true;
            Assert.AreEqual(request.HttpMethod.ToLower(), method.ToLower());
        };

        // Act
        await requestTask;

        // Assert
        Assert.IsTrue(spyHookTriggered, "The MockServer spy hook was never triggered.");
    }
}