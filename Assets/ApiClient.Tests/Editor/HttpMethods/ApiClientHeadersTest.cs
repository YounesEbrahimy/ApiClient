using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
using System.Collections;

public class ApiClientHeadersTest : ApiClientTestBase
{
    [UnityTest]
    public IEnumerator ApplyHeaders_MergesPersistentAndCustomHeadersCorrectly() => UniTask.ToCoroutine(async () =>
    {
        // Arrange
        const string Header_Auth = "Authorization";
        const string Value_Auth = "Bearer my-token";
        const string Header_ReqId = "X-Request-Id";
        const string Value_Ver = "0123456789";
        var spyHookTriggered = false;

        MockServer.OnRequestReceived = request =>
        {
            spyHookTriggered = true;
            Assert.AreEqual(Value_Auth, request.Headers[Header_Auth]);
            Assert.AreEqual(Value_Ver, request.Headers[Header_ReqId]);
        };

        MockServer.ResponseStatusCode = 200;
        MockServer.ResponseObject = string.Empty;

        // Act
        Client.AddHeader(Header_Auth, Value_Auth); // Persistent

        var customHeaders = new Dictionary<string, string>
        {
            { Header_ReqId, Value_Ver }
        }; // One-off

        await Client.GetAsync<TestPayload>("api/v1/headers", headers: customHeaders);

        // Assert
        Assert.IsTrue(spyHookTriggered, "The MockServer spy hook was never triggered.");
    });
}