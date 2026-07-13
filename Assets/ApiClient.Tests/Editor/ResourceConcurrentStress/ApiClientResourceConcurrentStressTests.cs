using System.Text.RegularExpressions;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using System.Collections;
using NUnit.Framework;
using ApiClientLib;
using UnityEngine;
using System;

public class ApiClientResourceConcurrentStressTests : ApiClientTestBase
{
    [UnityTest]
    public IEnumerator GetCachedSpriteAsync_MultipleDifferentUrlsConcurrently_DownloadsAllUniqueSprites() =>
        UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var distinctUrlCount = 5;
            var callCounter = 0;
            MockServer.ResponseStatusCode = 200;
            MockServer.ResponseBytes = RealPngBytes;
            MockServer.OnRequestReceived = _ => callCounter++;

            // Act: Fire requests for 5 entirely distinct image resources simultaneously
            var tasks = new List<UniTask<SpriteResponse>>();
            for (var i = 0; i < distinctUrlCount; i++)
            {
                tasks.Add(Client.GetCachedSpriteAsync($"image_{i}.png"));
            }

            var spriteResponses = await UniTask.WhenAll(tasks);

            // Assert
            Assert.AreEqual(distinctUrlCount, callCounter, "Server should be hit exactly once for every unique URL.");
            Assert.AreEqual(distinctUrlCount, spriteResponses.Length,
                "Should return a unique sprite instance for each successful download.");
            for (var i = 0; i < spriteResponses.Length; i++)
            {
                Assert.IsNotNull(spriteResponses[i].Sprite,
                    $"Sprite at index {i} should be successfully instantiated.");
            }
        });

    [UnityTest]
    public IEnumerator GetCachedAudioClipAsync_MultipleDifferentUrlsConcurrently_DownloadsAllUniqueAudioClips() =>
        UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var distinctUrlCount = 5;
            var callCounter = 0;
            MockServer.ResponseStatusCode = 200;
            MockServer.ResponseBytes = RealMp3Bytes;
            MockServer.OnRequestReceived = _ => callCounter++;

            // Act: Fire requests for 5 entirely distinct audio clip resources simultaneously
            var tasks = new List<UniTask<AudioClipResponse>>();
            for (var i = 0; i < distinctUrlCount; i++)
            {
                tasks.Add(Client.GetCachedAudioClipAsync($"image_{i}.mp3"));
            }

            var audioClipResponses = await UniTask.WhenAll(tasks);

            // Assert
            Assert.AreEqual(distinctUrlCount, callCounter, "Server should be hit exactly once for every unique URL.");
            Assert.AreEqual(distinctUrlCount, audioClipResponses.Length,
                "Should return a unique AudioClip instance for each successful download.");
            for (var i = 0; i < audioClipResponses.Length; i++)
            {
                Assert.IsNotNull(audioClipResponses[i].AudioClip,
                    $"AudioClip at index {i} should be successfully instantiated.");
            }
        });

    [UnityTest]
    public IEnumerator GetSpriteAsync_WhenServerReturnsEmptyZeroBytes_ThrowsBadSpriteException() =>
        UniTask.ToCoroutine(async () =>
        {
            // Arrange
            MockServer.ResponseStatusCode = 200;
            MockServer.ResponseBytes = Array.Empty<byte>(); // 0-byte payload response

            // Act & Assert
            try
            {
                await Client.GetSpriteAsync("empty.png");
                Assert.Fail("Expected an empty texture payload to throw a BadSpriteException.");
            }
            catch (BadSpriteException)
            {
                Assert.Pass();
            }
        });

    [UnityTest]
    public IEnumerator GetAudioClipAsync_WhenServerReturnsEmptyZeroBytes_ThrowsBadAudioClipException() =>
        UniTask.ToCoroutine(async () =>
        {
            // Arrange
            MockServer.ResponseStatusCode = 200;
            MockServer.ResponseBytes = Array.Empty<byte>(); // 0-byte payload response

            // Since the native audio processing layer will log an error, But it's expected
            LogAssert.Expect(LogType.Error, new Regex("FMOD"));

            // Act & Assert
            try
            {
                await Client.GetAudioClipAsync("empty.mp3");
                Assert.Fail("Expected an empty audio clip payload to throw a BadAudioClipException.");
            }
            catch (BadAudioClipException)
            {
                Assert.Pass();
            }
        });

    [UnityTest]
    public IEnumerator GetSpriteAsync_WhenServerReturns204NoContent_ThrowsApiException() =>
        UniTask.ToCoroutine(async () =>
        {
            // Arrange
            MockServer.ResponseStatusCode = 204; // No Content
            MockServer.ResponseBytes = Array.Empty<byte>();

            // Act & Assert
            try
            {
                await Client.GetSpriteAsync("nocontent.png");
                Assert.Fail("Expected 204 No Content response to throw an exception.");
            }
            catch (ApiException e)
            {
                Assert.AreEqual(204, e.StatusCode, "ApiException should carry the 204 status code.");
            }
            catch (Exception e)
            {
                Assert.Fail("Expected an ApiException with the 204 status code.");
            }
        });

    [UnityTest]
    public IEnumerator GetAudioClipAsync_WhenServerReturns204NoContent_ThrowsApiException() =>
        UniTask.ToCoroutine(async () =>
        {
            // Arrange
            MockServer.ResponseStatusCode = 204; // No Content
            MockServer.ResponseBytes = Array.Empty<byte>();

            // Act & Assert
            try
            {
                await Client.GetAudioClipAsync("nocontent.mp3");
                Assert.Fail("Expected 204 No Content response to throw an exception.");
            }
            catch (ApiException e)
            {
                Assert.AreEqual(204, e.StatusCode, "ApiException should carry the 204 status code.");
            }
            catch (Exception e)
            {
                Assert.Fail("Expected an ApiException with the 204 status code.");
            }
        });
}