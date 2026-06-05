using System.Text.RegularExpressions;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using System.Collections;
using NUnit.Framework;
using ApiClientLib;
using UnityEngine;
using System;

public class ApiClientSpriteConcurrentStressTests : ApiClientTestBase
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
            var tasks = new List<UniTask<Sprite>>();
            for (var i = 0; i < distinctUrlCount; i++)
            {
                tasks.Add(Client.GetCachedSpriteAsync($"{MockServer.ServerUrl}image_{i}.png"));
            }

            var sprites = await UniTask.WhenAll(tasks);

            // Assert
            Assert.AreEqual(distinctUrlCount, callCounter, "Server should be hit exactly once for every unique URL.");
            Assert.AreEqual(distinctUrlCount, sprites.Length,
                "Should return a unique sprite instance for each successful download.");
            for (var i = 0; i < sprites.Length; i++)
            {
                Assert.IsNotNull(sprites[i], $"Sprite at index {i} should be successfully instantiated.");
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
            var tasks = new List<UniTask<AudioClip>>();
            for (var i = 0; i < distinctUrlCount; i++)
            {
                tasks.Add(Client.GetCachedAudioClipAsync($"{MockServer.ServerUrl}image_{i}.mp3"));
            }

            var audioClips = await UniTask.WhenAll(tasks);

            // Assert
            Assert.AreEqual(distinctUrlCount, callCounter, "Server should be hit exactly once for every unique URL.");
            Assert.AreEqual(distinctUrlCount, audioClips.Length,
                "Should return a unique AudioClip instance for each successful download.");
            for (var i = 0; i < audioClips.Length; i++)
            {
                Assert.IsNotNull(audioClips[i], $"AudioClip at index {i} should be successfully instantiated.");
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
                await Client.GetSpriteAsync(MockServer.ServerUrl + "empty.png");
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
                await Client.GetAudioClipAsync(MockServer.ServerUrl + "empty.mp3");
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
                await Client.GetSpriteAsync(MockServer.ServerUrl + "nocontent.png");
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
                await Client.GetAudioClipAsync(MockServer.ServerUrl + "nocontent.mp3");
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