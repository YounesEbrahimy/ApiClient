using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using System.Collections;
using NUnit.Framework;
using ApiClientLib;
using System.Text;
using System;

public class ApiClientBadJsonResponseTests : ApiClientTestBase
{
    [UnityTest]
    [TestCase(200, ExpectedResult = null)]
    [TestCase(201, ExpectedResult = null)]
    [TestCase(204, ExpectedResult = null)]
    [TestCase(256, ExpectedResult = null)]
    public IEnumerator AllMethodsNoBody_WhenBadJsonResponse_DoesNotThrow(int code) => UniTask.ToCoroutine(async () =>
    {
        // Arrange
        MockServer.ResponseStatusCode = code;
        MockServer.ResponseObject = Encoding.UTF8.GetBytes("{ invalid json...");

        try
        {
            // Act
            await Client.GetAsync("api/v1/test");
            await Client.PostAsync("api/v1/test", string.Empty);
            await Client.PutAsync("api/v1/test", string.Empty);
            await Client.PatchAsync("api/v1/test", string.Empty);
            await Client.DeleteAsync("api/v1/test");
        }
        catch (Exception e)
        {
            // Assert, No exception must be thrown
            Assert.Fail($"Unexpected exception: {e.Message}");
        }
    });

    [UnityTest]
    [TestCase(200, ExpectedResult = null)]
    [TestCase(201, ExpectedResult = null)]
    [TestCase(204, ExpectedResult = null)]
    [TestCase(256, ExpectedResult = null)]
    public IEnumerator GetAsyncWithBody_WhenBadJsonResponse_ThrowsJsonException(int code) =>
        UniTask.ToCoroutine(async () =>
        {
            // Arrange
            MockServer.ResponseStatusCode = code;
            MockServer.ResponseObject = Encoding.UTF8.GetBytes("{ invalid json...");

            try
            {
                // Act
                await Client.GetAsync<TestPayload>("api/v1/test");
                Assert.Fail("Expected JsonException, but no exception was thrown.");
            }
            catch (JsonException ex)
            {
                Assert.Pass(); // This is what we want
            }
            catch (ApiException ex)
            {
                // Throws if status code indicates content yet ApiException was thrown instead of JsonException
                ThrowIfStatusCodeIndicatesContent(ex.StatusCode);
            }
            catch (Exception e)
            {
                // Assert, Only exceptions of type JsonException must be thrown
                Assert.Fail($"Unexpected exception: {e.Message}");
            }
        });

    [UnityTest]
    [TestCase(200, ExpectedResult = null)]
    [TestCase(201, ExpectedResult = null)]
    [TestCase(204, ExpectedResult = null)]
    [TestCase(256, ExpectedResult = null)]
    public IEnumerator PostAsyncWithBody_WhenBadJsonResponse_ThrowsJsonException(int code) =>
        UniTask.ToCoroutine(async () =>
        {
            // Arrange
            MockServer.ResponseStatusCode = code;
            MockServer.ResponseObject = Encoding.UTF8.GetBytes("{ invalid json...");

            try
            {
                // Act
                await Client.PostAsync<TestPayload>("api/v1/test", string.Empty);
                Assert.Fail("Expected JsonException, but no exception was thrown.");
            }
            catch (JsonException ex)
            {
                Assert.Pass(); // This is what we want
            }
            catch (ApiException ex)
            {
                // Throws if status code indicates content yet ApiException was thrown instead of JsonException
                ThrowIfStatusCodeIndicatesContent(ex.StatusCode);
            }
            catch (Exception e)
            {
                // Assert, Only exceptions of type JsonException must be thrown
                Assert.Fail($"Unexpected exception: {e.Message}");
            }
        });

    [UnityTest]
    [TestCase(200, ExpectedResult = null)]
    [TestCase(201, ExpectedResult = null)]
    [TestCase(204, ExpectedResult = null)]
    [TestCase(256, ExpectedResult = null)]
    public IEnumerator PutAsyncWithBody_WhenBadJsonResponse_ThrowsJsonException(int code) =>
        UniTask.ToCoroutine(async () =>
        {
            // Arrange
            MockServer.ResponseStatusCode = code;
            MockServer.ResponseObject = Encoding.UTF8.GetBytes("{ invalid json...");

            try
            {
                // Act
                await Client.PutAsync<TestPayload>("api/v1/test", string.Empty);
                Assert.Fail("Expected JsonException, but no exception was thrown.");
            }
            catch (JsonException ex)
            {
                Assert.Pass(); // This is what we want
            }
            catch (ApiException ex)
            {
                // Throws if status code indicates content yet ApiException was thrown instead of JsonException
                ThrowIfStatusCodeIndicatesContent(ex.StatusCode);
            }
            catch (Exception e)
            {
                // Assert, Only exceptions of type JsonException must be thrown
                Assert.Fail($"Unexpected exception: {e.Message}");
            }
        });

    [UnityTest]
    [TestCase(200, ExpectedResult = null)]
    [TestCase(201, ExpectedResult = null)]
    [TestCase(204, ExpectedResult = null)]
    [TestCase(256, ExpectedResult = null)]
    public IEnumerator PatchAsyncWithBody_WhenBadJsonResponse_ThrowsJsonException(int code) =>
        UniTask.ToCoroutine(async () =>
        {
            // Arrange
            MockServer.ResponseStatusCode = code;
            MockServer.ResponseObject = Encoding.UTF8.GetBytes("{ invalid json...");

            try
            {
                // Act
                await Client.PatchAsync<TestPayload>("api/v1/test", string.Empty);
                Assert.Fail("Expected JsonException, but no exception was thrown.");
            }
            catch (JsonException ex)
            {
                Assert.Pass(); // This is what we want
            }
            catch (ApiException ex)
            {
                // Throws if status code indicates content yet ApiException was thrown instead of JsonException
                ThrowIfStatusCodeIndicatesContent(ex.StatusCode);
            }
            catch (Exception e)
            {
                // Assert, Only exceptions of type JsonException must be thrown
                Assert.Fail($"Unexpected exception: {e.Message}");
            }
        });

    [UnityTest]
    [TestCase(200, ExpectedResult = null)]
    [TestCase(201, ExpectedResult = null)]
    [TestCase(204, ExpectedResult = null)]
    [TestCase(256, ExpectedResult = null)]
    public IEnumerator DeleteAsyncWithBody_WhenBadJsonResponse_ThrowsJsonException(int code) =>
        UniTask.ToCoroutine(async () =>
        {
            // Arrange
            MockServer.ResponseStatusCode = code;
            MockServer.ResponseObject = Encoding.UTF8.GetBytes("{ invalid json...");

            try
            {
                // Act
                await Client.DeleteAsync<TestPayload>("api/v1/test");
                Assert.Fail("Expected JsonException, but no exception was thrown.");
            }
            catch (JsonException ex)
            {
                Assert.Pass(); // This is what we want
            }
            catch (ApiException ex)
            {
                // Throws if status code indicates content yet ApiException was thrown instead of JsonException
                ThrowIfStatusCodeIndicatesContent(ex.StatusCode);
            }
            catch (Exception e)
            {
                // Assert, Only exceptions of type JsonException must be thrown
                Assert.Fail($"Unexpected exception: {e.Message}");
            }
        });

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static void ThrowIfStatusCodeIndicatesContent(int statusCode)
    {
        Assert.AreEqual(IsBodyLessResponse(statusCode), true, "Expected status code to indicate no content");
    }
}