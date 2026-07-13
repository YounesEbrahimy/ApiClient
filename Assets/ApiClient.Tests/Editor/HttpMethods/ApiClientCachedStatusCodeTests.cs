using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using System.Collections;
using NUnit.Framework;
using System;

[TestFixture]
public class ApiClientCachedStatusCodeTests : ApiClientTestBase
{
    [UnityTest]
    public IEnumerator GetCachedSpriteAsync_ReturnsCorrectStatusCode() => UniTask.ToCoroutine(async () =>
    {
        // Arrange
        const int ServerStatusCode = 200;
        const string RelativeUrl = "api/test.png";
        MockServer.ResponseStatusCode = ServerStatusCode;
        MockServer.ResponseBytes = RealPngBytes;

        try
        {
            // Act
            var result1 = await Client.GetCachedSpriteAsync(RelativeUrl);
            Assert.IsNotNull(result1.Sprite, "Expected to get a Sprite.");
            Assert.AreEqual(ServerStatusCode, result1.StatusCode, $"First request should have code {ServerStatusCode}");

            var result2 = await Client.GetCachedSpriteAsync(RelativeUrl);
            Assert.IsNotNull(result2.Sprite, "Expected to get a Sprite.");
            Assert.AreEqual(-1, result2.StatusCode, $"Second request should have code {-1}");

            await Client.InvalidateCacheAsync();

            var result3 = await Client.GetCachedSpriteAsync(RelativeUrl);
            Assert.IsNotNull(result3.Sprite, "Expected to get a Sprite.");
            Assert.AreEqual(ServerStatusCode, result3.StatusCode, $"Third request should have code {ServerStatusCode}");
        }
        catch (Exception e)
        {
            // Assert, No Exceptions should be thrown
            Assert.Fail();
        }
    });

    [UnityTest]
    public IEnumerator GetCachedAudioClipAsync_ReturnsCorrectStatusCode() => UniTask.ToCoroutine(async () =>
    {
        // Arrange
        const int ServerStatusCode = 200;
        const string RelativeUrl = "api/test.mp3";
        MockServer.ResponseStatusCode = ServerStatusCode;
        MockServer.ResponseBytes = RealMp3Bytes;

        try
        {
            // Act
            var result1 = await Client.GetCachedAudioClipAsync(RelativeUrl);
            Assert.IsNotNull(result1.AudioClip, "Expected to get an AudioClip.");
            Assert.AreEqual(ServerStatusCode, result1.StatusCode, $"First request should have code {ServerStatusCode}");

            var result2 = await Client.GetCachedAudioClipAsync(RelativeUrl);
            Assert.IsNotNull(result2.AudioClip, "Expected to get an AudioClip.");
            Assert.AreEqual(-1, result2.StatusCode, $"Second request should have code {-1}");

            await Client.InvalidateCacheAsync();

            var result3 = await Client.GetCachedAudioClipAsync(RelativeUrl);
            Assert.IsNotNull(result3.AudioClip, "Expected to get an AudioClip.");
            Assert.AreEqual(ServerStatusCode, result3.StatusCode, $"Third request should have code {ServerStatusCode}");
        }
        catch (Exception e)
        {
            // Assert, No Exceptions should be thrown
            Assert.Fail();
        }
    });
}