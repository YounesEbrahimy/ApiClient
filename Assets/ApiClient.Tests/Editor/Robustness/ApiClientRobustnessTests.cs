using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using System.Collections;
using Newtonsoft.Json;
using NUnit.Framework;
using ApiClientLib;
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
                _ = await Client.GetSpriteAsync(MockServer.ServerUrl + "bad_data.png");
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
    public IEnumerator Cache_WhenIndexFileIsCorrupted_RecoversGracefully() => UniTask.ToCoroutine(async () =>
    {
        // Arrange: Manually corrupt the index file
        Directory.CreateDirectory(Client._cacheDir);
        await File.WriteAllTextAsync(Client._cacheIndexPath, "{ invalid json... ");

        // Arrange
        MockServer.ResponseStatusCode = 200;
        MockServer.ResponseBytes = RealPngBytes;

        // Loading cache shouldn't throw an exception, it should silently handle the corrupted index,
        // and delete the corrupt cache index file
        try
        {
            _ = await Client.GetCachedSpriteAsync(MockServer.ServerUrl + "test.png");
            var newJson = await File.ReadAllTextAsync(Client._cacheIndexPath);
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

    [Test]
    public void Basics_IdentifiesNoContentStatusCodesCorrectly()
    {
        // Iterates for all status codes between 0 and 1000 and checks if no-content
        // status codes are identified correctly
        for (var statusCode = 0; statusCode < 1000; statusCode++)
        {
            Assert.AreEqual(ApiClient.IsBodyLessResponse(statusCode), IsBodyLessResponse(statusCode)
                , "Identified no content status code incorrectly");
        }
    }
}