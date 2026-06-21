using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using System.Collections;
using NUnit.Framework;
using ApiClientLib;
using System.IO;
using System;

[TestFixture]
public class ApiClientResponseTests : ApiClientTestBase
{
    [UnityTest]
    [TestCase(200, TestPayload.PayloadKey.Leon, ExpectedResult = null)]
    [TestCase(201, TestPayload.PayloadKey.Ada, ExpectedResult = null)]
    [TestCase(204, TestPayload.PayloadKey.Ashley, ExpectedResult = null)]
    [TestCase(256, TestPayload.PayloadKey.Leon, ExpectedResult = null)]
    [TestCase(402, TestPayload.PayloadKey.Leon, ExpectedResult = null)]
    [TestCase(403, TestPayload.PayloadKey.Ada, ExpectedResult = null)]
    [TestCase(404, TestPayload.PayloadKey.Ashley, ExpectedResult = null)]
    [TestCase(500, TestPayload.PayloadKey.Leon, ExpectedResult = null)]
    public IEnumerator GetAsync_IfSuccessful_IgnoresServerResponse(int code, TestPayload.PayloadKey responseKey) =>
        UniTask.ToCoroutine(async () =>
        {
            // Arrange
            ArrangeMockServer(MockServer, responseKey, code);

            try
            {
                // Act
                await Client.GetAsync("api/v1/test");
            }
            catch (ApiException e)
            {
                // Assert, Fail if throws while status code indicates success
                ThrowIfStatusCodeIsSuccess(e.StatusCode);
            }
            catch (Exception e)
            {
                // Assert, Only exceptions of type ApiException must be thrown
                Assert.Fail();
            }
        });

    [UnityTest]
    [TestCase(200, TestPayload.PayloadKey.Leon, ExpectedResult = null)]
    [TestCase(201, TestPayload.PayloadKey.Ada, ExpectedResult = null)]
    [TestCase(204, TestPayload.PayloadKey.Ashley, ExpectedResult = null)]
    [TestCase(256, TestPayload.PayloadKey.Leon, ExpectedResult = null)]
    [TestCase(402, TestPayload.PayloadKey.Leon, ExpectedResult = null)]
    [TestCase(403, TestPayload.PayloadKey.Ada, ExpectedResult = null)]
    [TestCase(404, TestPayload.PayloadKey.Ashley, ExpectedResult = null)]
    [TestCase(500, TestPayload.PayloadKey.Leon, ExpectedResult = null)]
    public IEnumerator GetAsync_IfSuccessful_ReturnsDeserializedObject(int code, TestPayload.PayloadKey responseKey) =>
        UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var responsePayload = ArrangeMockServer(MockServer, responseKey, code);

            try
            {
                // Act
                var result = await Client.GetAsync<TestPayload>("api/v1/test");

                // Assertions
                CheckAssertionsIfStatusCodeIndicatesContent(responsePayload, result, code);
            }
            catch (ApiException e)
            {
                // Assert, Fail if throws while status code indicates success with content
                ThrowIfStatusCodeIsSuccessAndIndicatesContent(e.StatusCode);
            }
            catch (Exception e)
            {
                // Assert, Only exceptions of type ApiException must be thrown
                Assert.Fail();
            }
        });

    [UnityTest]
    [TestCase(TestPayload.PayloadKey.Leon, 200, TestPayload.PayloadKey.Ada, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Ada, 201, TestPayload.PayloadKey.Ashley, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Ashley, 201, TestPayload.PayloadKey.Leon, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Leon, 204, TestPayload.PayloadKey.Ada, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Leon, 402, TestPayload.PayloadKey.Ada, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Ada, 403, TestPayload.PayloadKey.Ashley, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Ashley, 404, TestPayload.PayloadKey.Leon, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Leon, 500, TestPayload.PayloadKey.Ada, ExpectedResult = null)]
    public IEnumerator PostAsync_IfSuccessful_IgnoresServerResponse(TestPayload.PayloadKey sendKey, int code,
        TestPayload.PayloadKey responseKey) => UniTask.ToCoroutine(async () =>
    {
        // Arrange
        var sendPayload = ArrangeSendPayload(sendKey);
        ArrangeMockServer(MockServer, responseKey, code);

        try
        {
            // Act
            await Client.PostAsync("api/v1/test", sendPayload);
        }
        catch (ApiException e)
        {
            // Assert, Fail if throws while status code indicates success
            ThrowIfStatusCodeIsSuccess(e.StatusCode);
        }
        catch (Exception e)
        {
            // Assert, Only exceptions of type ApiException must be thrown
            Assert.Fail();
        }
    });

    [UnityTest]
    [TestCase(TestPayload.PayloadKey.Leon, 200, TestPayload.PayloadKey.Ada, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Ada, 201, TestPayload.PayloadKey.Ashley, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Ashley, 201, TestPayload.PayloadKey.Leon, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Leon, 204, TestPayload.PayloadKey.Ada, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Leon, 402, TestPayload.PayloadKey.Ada, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Ada, 403, TestPayload.PayloadKey.Ashley, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Ashley, 404, TestPayload.PayloadKey.Leon, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Leon, 500, TestPayload.PayloadKey.Ada, ExpectedResult = null)]
    public IEnumerator PostAsync_IfSuccessful_ReturnsDeserializedObject(TestPayload.PayloadKey sendKey, int code,
        TestPayload.PayloadKey responseKey) => UniTask.ToCoroutine(async () =>
    {
        // Arrange
        var sendPayload = ArrangeSendPayload(sendKey);
        var responsePayload = ArrangeMockServer(MockServer, responseKey, code);

        try
        {
            // Act
            var result = await Client.PostAsync<TestPayload>("api/v1/test", sendPayload);

            // Assertions
            CheckAssertionsIfStatusCodeIndicatesContent(responsePayload, result, code);
        }
        catch (ApiException e)
        {
            // Assert, Fail if throws while status code indicates success with content
            ThrowIfStatusCodeIsSuccessAndIndicatesContent(e.StatusCode);
        }
        catch (Exception e)
        {
            // Assert, Only exceptions of type ApiException must be thrown
            Assert.Fail();
        }
    });

    [UnityTest]
    [TestCase(TestPayload.PayloadKey.Leon, 200, TestPayload.PayloadKey.Ada, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Ada, 201, TestPayload.PayloadKey.Ashley, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Ashley, 201, TestPayload.PayloadKey.Leon, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Leon, 204, TestPayload.PayloadKey.Ada, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Leon, 402, TestPayload.PayloadKey.Ada, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Ada, 403, TestPayload.PayloadKey.Ashley, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Ashley, 404, TestPayload.PayloadKey.Leon, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Leon, 500, TestPayload.PayloadKey.Ada, ExpectedResult = null)]
    public IEnumerator PutAsync_IfSuccessful_IgnoresServerResponse(TestPayload.PayloadKey sendKey, int code,
        TestPayload.PayloadKey responseKey) => UniTask.ToCoroutine(async () =>
    {
        // Arrange
        var sendPayload = ArrangeSendPayload(sendKey);
        ArrangeMockServer(MockServer, responseKey, code);

        try
        {
            // Act
            await Client.PutAsync("api/v1/test", sendPayload);
        }
        catch (ApiException e)
        {
            // Assert, Fail if throws while status code indicates success
            ThrowIfStatusCodeIsSuccess(e.StatusCode);
        }
        catch (Exception e)
        {
            // Assert, Only exceptions of type ApiException must be thrown
            Assert.Fail();
        }
    });

    [UnityTest]
    [TestCase(TestPayload.PayloadKey.Leon, 200, TestPayload.PayloadKey.Ada, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Ada, 201, TestPayload.PayloadKey.Ashley, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Ashley, 201, TestPayload.PayloadKey.Leon, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Leon, 204, TestPayload.PayloadKey.Ada, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Leon, 402, TestPayload.PayloadKey.Ada, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Ada, 403, TestPayload.PayloadKey.Ashley, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Ashley, 404, TestPayload.PayloadKey.Leon, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Leon, 500, TestPayload.PayloadKey.Ada, ExpectedResult = null)]
    public IEnumerator PutAsync_IfSuccessful_ReturnsDeserializedObject(TestPayload.PayloadKey sendKey, int code,
        TestPayload.PayloadKey responseKey) => UniTask.ToCoroutine(async () =>
    {
        // Arrange
        var sendPayload = ArrangeSendPayload(sendKey);
        var responsePayload = ArrangeMockServer(MockServer, responseKey, code);

        try
        {
            // Act
            var result = await Client.PutAsync<TestPayload>("api/v1/test", sendPayload);

            // Assertions
            CheckAssertionsIfStatusCodeIndicatesContent(responsePayload, result, code);
        }
        catch (ApiException e)
        {
            // Assert, Fail if throws while status code indicates success with content
            ThrowIfStatusCodeIsSuccessAndIndicatesContent(e.StatusCode);
        }
        catch (Exception e)
        {
            // Assert, Only exceptions of type ApiException must be thrown
            Assert.Fail();
        }
    });

    [UnityTest]
    [TestCase(TestPayload.PayloadKey.Leon, 200, TestPayload.PayloadKey.Ada, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Ada, 201, TestPayload.PayloadKey.Ashley, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Ashley, 201, TestPayload.PayloadKey.Leon, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Leon, 204, TestPayload.PayloadKey.Ada, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Leon, 402, TestPayload.PayloadKey.Ada, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Ada, 403, TestPayload.PayloadKey.Ashley, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Ashley, 404, TestPayload.PayloadKey.Leon, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Leon, 500, TestPayload.PayloadKey.Ada, ExpectedResult = null)]
    public IEnumerator PatchAsync_IfSuccessful_IgnoresServerResponse(TestPayload.PayloadKey sendKey, int code,
        TestPayload.PayloadKey responseKey) => UniTask.ToCoroutine(async () =>
    {
        // Arrange
        var sendPayload = ArrangeSendPayload(sendKey);
        ArrangeMockServer(MockServer, responseKey, code);

        try
        {
            // Act
            await Client.PatchAsync("api/v1/test", sendPayload);
        }
        catch (ApiException e)
        {
            // Assert, Fail if throws while status code indicates success
            ThrowIfStatusCodeIsSuccess(e.StatusCode);
        }
        catch (Exception e)
        {
            // Assert, Only exceptions of type ApiException must be thrown
            Assert.Fail();
        }
    });

    [UnityTest]
    [TestCase(TestPayload.PayloadKey.Leon, 200, TestPayload.PayloadKey.Ada, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Ada, 201, TestPayload.PayloadKey.Ashley, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Ashley, 201, TestPayload.PayloadKey.Leon, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Leon, 204, TestPayload.PayloadKey.Ada, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Leon, 402, TestPayload.PayloadKey.Ada, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Ada, 403, TestPayload.PayloadKey.Ashley, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Ashley, 404, TestPayload.PayloadKey.Leon, ExpectedResult = null)]
    [TestCase(TestPayload.PayloadKey.Leon, 500, TestPayload.PayloadKey.Ada, ExpectedResult = null)]
    public IEnumerator PatchAsync_IfSuccessful_ReturnsDeserializedObject(TestPayload.PayloadKey sendKey, int code,
        TestPayload.PayloadKey responseKey) => UniTask.ToCoroutine(async () =>
    {
        // Arrange
        var sendPayload = ArrangeSendPayload(sendKey);
        var responsePayload = ArrangeMockServer(MockServer, responseKey, code);

        try
        {
            // Act
            var result = await Client.PatchAsync<TestPayload>("api/v1/test", sendPayload);

            // Assertions
            CheckAssertionsIfStatusCodeIndicatesContent(responsePayload, result, code);
        }
        catch (ApiException e)
        {
            // Assert, Fail if throws while status code indicates success with content
            ThrowIfStatusCodeIsSuccessAndIndicatesContent(e.StatusCode);
        }
        catch (Exception e)
        {
            // Assert, Only exceptions of type ApiException must be thrown
            Assert.Fail();
        }
    });

    [UnityTest]
    [TestCase(200, TestPayload.PayloadKey.Leon, ExpectedResult = null)]
    [TestCase(201, TestPayload.PayloadKey.Ada, ExpectedResult = null)]
    [TestCase(204, TestPayload.PayloadKey.Ashley, ExpectedResult = null)]
    [TestCase(256, TestPayload.PayloadKey.Leon, ExpectedResult = null)]
    [TestCase(402, TestPayload.PayloadKey.Leon, ExpectedResult = null)]
    [TestCase(403, TestPayload.PayloadKey.Ada, ExpectedResult = null)]
    [TestCase(404, TestPayload.PayloadKey.Ashley, ExpectedResult = null)]
    [TestCase(500, TestPayload.PayloadKey.Leon, ExpectedResult = null)]
    public IEnumerator DeleteAsync_IfSuccessful_IgnoresServerResponse(int code,
        TestPayload.PayloadKey responseKey) => UniTask.ToCoroutine(async () =>
    {
        // Arrange
        ArrangeMockServer(MockServer, responseKey, code);

        try
        {
            // Act
            await Client.DeleteAsync("api/v1/test");
        }
        catch (ApiException e)
        {
            // Assert, Fail if throws while status code indicates success
            ThrowIfStatusCodeIsSuccess(e.StatusCode);
        }
        catch (Exception e)
        {
            // Assert, Only exceptions of type ApiException must be thrown
            Assert.Fail();
        }
    });

    [UnityTest]
    [TestCase(200, TestPayload.PayloadKey.Leon, ExpectedResult = null)]
    [TestCase(201, TestPayload.PayloadKey.Ada, ExpectedResult = null)]
    [TestCase(204, TestPayload.PayloadKey.Ashley, ExpectedResult = null)]
    [TestCase(256, TestPayload.PayloadKey.Leon, ExpectedResult = null)]
    [TestCase(402, TestPayload.PayloadKey.Leon, ExpectedResult = null)]
    [TestCase(403, TestPayload.PayloadKey.Ada, ExpectedResult = null)]
    [TestCase(404, TestPayload.PayloadKey.Ashley, ExpectedResult = null)]
    [TestCase(500, TestPayload.PayloadKey.Leon, ExpectedResult = null)]
    public IEnumerator DeleteAsync_IfSuccessful_ReturnsDeserializedObject(int code,
        TestPayload.PayloadKey responseKey) => UniTask.ToCoroutine(async () =>
    {
        // Arrange
        var responsePayload = ArrangeMockServer(MockServer, responseKey, code);

        try
        {
            // Act
            var result = await Client.DeleteAsync<TestPayload>("api/v1/test");

            // Assertions
            CheckAssertionsIfStatusCodeIndicatesContent(responsePayload, result, code);
        }
        catch (ApiException e)
        {
            // Assert, Fail if throws while status code indicates success with content
            ThrowIfStatusCodeIsSuccessAndIndicatesContent(e.StatusCode);
        }
        catch (Exception e)
        {
            // Assert, Only exceptions of type ApiException must be thrown
            Assert.Fail();
        }
    });

    [UnityTest]
    [TestCase(200, ExpectedResult = null)]
    [TestCase(201, ExpectedResult = null)]
    [TestCase(202, ExpectedResult = null)]
    [TestCase(403, ExpectedResult = null)]
    [TestCase(404, ExpectedResult = null)]
    [TestCase(500, ExpectedResult = null)]
    public IEnumerator GetSpriteAsync_IfSuccessful_ReturnsSprite(int code) => UniTask.ToCoroutine(async () =>
    {
        // Arrange
        MockServer.ResponseStatusCode = code;
        MockServer.ResponseBytes = RealPngBytes;

        try
        {
            // Act
            _ = await Client.GetSpriteAsync(MockServer.ServerUrl + "test.png");
        }
        catch (ApiException e)
        {
            // Assert, Fail if throws while status code indicates success with content
            ThrowIfStatusCodeIsSuccessAndIndicatesContent(e.StatusCode);
        }
        catch (Exception e)
        {
            // Assert, Only exceptions of type ApiException must be thrown
            Assert.Fail();
        }
    });

    [UnityTest]
    [TestCase(200, ExpectedResult = null)]
    [TestCase(201, ExpectedResult = null)]
    [TestCase(202, ExpectedResult = null)]
    [TestCase(403, ExpectedResult = null)]
    [TestCase(404, ExpectedResult = null)]
    [TestCase(500, ExpectedResult = null)]
    public IEnumerator GetCachedSpriteAsync_IfSuccessful_CachesAndReturnsSprite(int code) =>
        UniTask.ToCoroutine(async () =>
        {
            // Arrange
            MockServer.ResponseStatusCode = code;
            MockServer.ResponseBytes = RealPngBytes;
            var url = MockServer.ServerUrl + "test.png";

            try
            {
                // Act
                _ = await Client.GetCachedSpriteAsync(url, urlType: UrlType.Absolute);

                var key = ApiClient.ComputeHash(url);
                var filePath = Path.Combine(Client._cacheDir, $"{key}.png");
                if (!File.Exists(filePath))
                    throw new FileNotFoundException();
            }
            catch (ApiException e)
            {
                // Assert, Fail if throws while status code indicates success with content
                ThrowIfStatusCodeIsSuccessAndIndicatesContent(e.StatusCode);
            }
            catch (Exception e)
            {
                // Assert, Only exceptions of type ApiException must be thrown
                Assert.Fail();
            }
        });

    [UnityTest]
    [TestCase(200, ExpectedResult = null)]
    [TestCase(201, ExpectedResult = null)]
    [TestCase(202, ExpectedResult = null)]
    [TestCase(403, ExpectedResult = null)]
    [TestCase(404, ExpectedResult = null)]
    [TestCase(500, ExpectedResult = null)]
    public IEnumerator GetAudioClipAsync_IfSuccessful_ReturnsSprite(int code) => UniTask.ToCoroutine(async () =>
    {
        // Arrange
        MockServer.ResponseStatusCode = code;
        MockServer.ResponseBytes = RealMp3Bytes;

        try
        {
            // Act
            _ = await Client.GetAudioClipAsync(MockServer.ServerUrl + "test.mp3");
        }
        catch (ApiException e)
        {
            // Assert, Fail if throws while status code indicates success with content
            ThrowIfStatusCodeIsSuccessAndIndicatesContent(e.StatusCode);
        }
        catch (Exception e)
        {
            // Assert, Only exceptions of type ApiException must be thrown
            Assert.Fail();
        }
    });

    [UnityTest]
    [TestCase(200, ExpectedResult = null)]
    [TestCase(201, ExpectedResult = null)]
    [TestCase(202, ExpectedResult = null)]
    [TestCase(403, ExpectedResult = null)]
    [TestCase(404, ExpectedResult = null)]
    [TestCase(500, ExpectedResult = null)]
    public IEnumerator GetCachedAudioClipAsync_IfSuccessful_CachesAndReturnsSprite(int code) =>
        UniTask.ToCoroutine(async () =>
        {
            // Arrange
            MockServer.ResponseStatusCode = code;
            MockServer.ResponseBytes = RealMp3Bytes;
            var url = MockServer.ServerUrl + "test.mp3";

            try
            {
                // Act
                _ = await Client.GetCachedAudioClipAsync(url, urlType: UrlType.Absolute);

                var key = ApiClient.ComputeHash(url);
                var filePath = Path.Combine(Client._cacheDir, $"{key}.mp3");
                if (!File.Exists(filePath))
                    throw new FileNotFoundException();
            }
            catch (ApiException e)
            {
                // Assert, Fail if throws while status code indicates success with content
                ThrowIfStatusCodeIsSuccessAndIndicatesContent(e.StatusCode);
            }
            catch (Exception e)
            {
                // Assert, Only exceptions of type ApiException must be thrown
                Assert.Fail();
            }
        });

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static TestPayload ArrangeSendPayload(TestPayload.PayloadKey sendKey)
    {
        return TestPayload.GetPayloadByKey(sendKey);
    }

    private static TestPayload ArrangeMockServer(MockHttpServer server, TestPayload.PayloadKey responseKey,
        int statusCode)
    {
        var responsePayload = TestPayload.GetPayloadByKey(responseKey);
        server.ResponseStatusCode = statusCode;
        server.ResponseObject = responsePayload;
        return responsePayload;
    }

    private static void CheckAssertionsIfStatusCodeIndicatesContent(TestPayload responsePayload, TestPayload result,
        int statusCode)
    {
        if (statusCode == 204) return;
        Assert.NotNull(result);
        Assert.AreEqual(result.Id, responsePayload.Id);
        Assert.AreEqual(result.Name, responsePayload.Name);
    }

    private static void ThrowIfStatusCodeIsSuccess(int statusCode)
    {
        if (statusCode is >= 200 and < 300)
            Assert.Fail("Status code indicates success, But ApiException was thrown!");
    }

    private static void ThrowIfStatusCodeIsSuccessAndIndicatesContent(int statusCode)
    {
        if (statusCode is >= 200 and < 300 && !IsBodyLessResponse(statusCode))
            Assert.Fail("Status code indicates success and indicates content, But ApiException was thrown!");
    }
}