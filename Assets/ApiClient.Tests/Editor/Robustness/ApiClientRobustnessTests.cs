using System.Text.RegularExpressions;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using System.Collections;
using Newtonsoft.Json;
using NUnit.Framework;
using ApiClientLib;
using UnityEngine;
using System.IO;
using System;

public class ApiClientRobustnessTests : ApiClientTestBase
{
    [UnityTest]
    public IEnumerator GetSpriteAsync_WhenServerReturnsGarbage_ThrowsBadSpriteException() =>
        UniTask.ToCoroutine(async () =>
        {
            // Arrange: Server returns non-image garbage
            MockServer.ResponseStatusCode = 200;
            MockServer.ResponseBytes = FakePngBytes;

            // Act & Assert
            try
            {
                _ = await Client.GetSpriteAsync("bad_data.png");
                Assert.Fail(); // Fail, Since it must throw BadSpriteException
            }
            catch (BadSpriteException e)
            {
                Assert.Pass(); // This is what we want
            }
            catch (Exception e)
            {
                Assert.Fail(); // Fail, Since it must throw BadSpriteException
            }
        });

    [UnityTest]
    public IEnumerator GetCachedSpriteAsync_WhenServerReturnsGarbage_ThrowsBadSpriteException() =>
        UniTask.ToCoroutine(async () =>
        {
            // Arrange: Server returns non-image garbage
            MockServer.ResponseStatusCode = 200;
            MockServer.ResponseBytes = FakePngBytes;

            // Act & Assert
            try
            {
                _ = await Client.GetCachedSpriteAsync("bad_data.png");
                Assert.Fail(); // Fail, Since it must throw BadSpriteException
            }
            catch (BadSpriteException e)
            {
                Assert.Pass(); // This is what we want
            }
            catch (Exception e)
            {
                Assert.Fail(); // Fail, Since it must throw BadSpriteException
            }
        });

    [UnityTest]
    public IEnumerator GetAudioClipAsync_WhenServerReturnsGarbage_ThrowsBadAudioClipException() =>
        UniTask.ToCoroutine(async () =>
        {
            // Arrange: Server returns non-audioClip garbage
            MockServer.ResponseStatusCode = 200;
            MockServer.ResponseBytes = FakeMp3Bytes;

            // Since the native audio processing layer will log an error, But it's expected
            LogAssert.Expect(LogType.Error, new Regex("FMOD"));

            // Act & Assert
            try
            {
                _ = await Client.GetAudioClipAsync("bad_data.mp3");
                Assert.Fail(); // Fail, Since it must throw BadAudioClipException
            }
            catch (BadAudioClipException e)
            {
                Assert.Pass(); // This is what we want
            }
            catch (Exception e)
            {
                Assert.Fail(); // Fail, Since it must throw BadAudioClipException
            }
        });

    [UnityTest]
    public IEnumerator GetCachedAudioClipAsync_WhenServerReturnsGarbage_ThrowsBadAudioClipException() =>
        UniTask.ToCoroutine(async () =>
        {
            // Arrange: Server returns non-audioClip garbage
            MockServer.ResponseStatusCode = 200;
            MockServer.ResponseBytes = FakeMp3Bytes;

            // Since the native audio processing layer will log an error, But it's expected
            LogAssert.Expect(LogType.Error, new Regex("FMOD"));

            // Act & Assert
            try
            {
                _ = await Client.GetCachedAudioClipAsync("bad_data.mp3");
                Assert.Fail(); // Fail, Since it must throw BadAudioClipException
            }
            catch (BadAudioClipException e)
            {
                Assert.Pass(); // This is what we want
            }
            catch (Exception e)
            {
                Assert.Fail(); // Fail, Since it must throw BadAudioClipException
            }
        });

    [UnityTest]
    public IEnumerator Cache_WhenIndexFileIsCorrupted_RecoversGracefully() => UniTask.ToCoroutine(async () =>
    {
        // Arrange: Manually corrupt the index file
        Directory.CreateDirectory(Client.CacheDirectoryPath());
        await File.WriteAllTextAsync(Client.CacheIndexPath(), "{ invalid json... ");

        // Arrange
        MockServer.ResponseStatusCode = 200;
        MockServer.ResponseBytes = RealPngBytes;

        // Loading cache shouldn't throw an exception, it should silently handle the corrupted index,
        // and delete the corrupt cache index file
        try
        {
            _ = await Client.GetCachedSpriteAsync("test.png");
            var newJson = await File.ReadAllTextAsync(Client.CacheIndexPath());
            _ = JsonConvert.DeserializeObject<Dictionary<string, CacheEntry>>(newJson);
            // If code reaches here without throw, then the corrupted cache file has been deleted,
            // recreated and saved successfully and all systems have worked accordingly
        }
        catch (Exception e)
        {
            // Assert, since cache index must have been gracefully rebuilt
            Assert.Fail();
        }
    });
}