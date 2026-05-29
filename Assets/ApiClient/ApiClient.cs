using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class ApiClient : IApiClient
{
    private string _baseUrl = "";

    // ── Sprite cache ──────────────────────────────────────────────────────────
    private readonly string _cacheDir;
    private readonly string _cacheIndexPath;
    private Dictionary<string, string> _cacheIndex;
    private readonly object _cacheIndexLock = new object();

    // ── Concurrency guards ────────────────────────────────────────────────────
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _urlLocks = new();

    // ── Constructors ──────────────────────────────────────────────────────────
    public ApiClient()
    {
        _cacheDir       = Path.Combine(Application.persistentDataPath, "SpriteCache");
        _cacheIndexPath = Path.Combine(_cacheDir, "index.json");
        Directory.CreateDirectory(_cacheDir);}

    public ApiClient(string baseUrl) : this()
    {
        SetBaseUrl(baseUrl);
    }

    // ── BaseUrl ───────────────────────────────────────────────────────────────
    public string BaseUrl => _baseUrl;

    public void SetBaseUrl(string baseUrl)
    {
        if (baseUrl == null) throw new ArgumentNullException(nameof(baseUrl));
        _baseUrl = baseUrl.TrimEnd('/') + "/";
    }

    private string ResolveUrl(string url) => _baseUrl + url;

    // ── HTTP helpers ──────────────────────────────────────────────────────────
    private static UnityWebRequest BuildRequest(string method, string url, string jsonBody = null)
    {
        var req = new UnityWebRequest(url, method);
        req.downloadHandler = new DownloadHandlerBuffer();
        if (jsonBody != null)
        {
            byte[] body = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            req.uploadHandler = new UploadHandlerRaw(body);
            req.SetRequestHeader("Content-Type", "application/json");
        }
        return req;
    }

    private static async Task<string> SendAsync(UnityWebRequest req, int timeout)
    {
        req.timeout = timeout;
        var op = req.SendWebRequest();
        while (!op.isDone) await Task.Yield();

        if (req.result != UnityWebRequest.Result.Success)
            throw new ApiException((System.Net.HttpStatusCode)req.responseCode, req.error);

        return req.downloadHandler.text;
    }

    // ── Public HTTP methods ───────────────────────────────────────────────────
    public Task<string> GetAsync(string url, int timeout = 30)
    {
        var req = BuildRequest(UnityWebRequest.kHttpVerbGET, ResolveUrl(url));
        return SendAsync(req, timeout);
    }

    public Task<string> PostAsync(string url, string jsonBody, int timeout = 30)
    {
        var req = BuildRequest(UnityWebRequest.kHttpVerbPOST, ResolveUrl(url), jsonBody);
        return SendAsync(req, timeout);
    }

    public Task<string> PutAsync(string url, string jsonBody, int timeout = 30)
    {
        var req = BuildRequest(UnityWebRequest.kHttpVerbPUT, ResolveUrl(url), jsonBody);
        return SendAsync(req, timeout);
    }

    public Task<string> PatchAsync(string url, string jsonBody, int timeout = 30)
    {
        var req = BuildRequest("PATCH", ResolveUrl(url), jsonBody);
        return SendAsync(req, timeout);
    }

    public Task<string> DeleteAsync(string url, int timeout = 30)
    {
        var req = BuildRequest(UnityWebRequest.kHttpVerbDELETE, ResolveUrl(url));
        return SendAsync(req, timeout);
    }

    public Task<string> DeleteAsync(string url, string jsonBody, int timeout = 30)
    {
        var req = BuildRequest(UnityWebRequest.kHttpVerbDELETE, ResolveUrl(url), jsonBody);
        return SendAsync(req, timeout);
    }

    // ── Sprite (BaseUrl intentionally NOT applied) ────────────────────────────
    public async Task<Sprite> GetSpriteAsync(string url, int timeout = 30)
    {
        using var req = UnityWebRequestTexture.GetTexture(url);
        req.timeout = timeout;
        var op = req.SendWebRequest();
        while (!op.isDone) await Task.Yield();

        if (req.result != UnityWebRequest.Result.Success)
            throw new ApiException((System.Net.HttpStatusCode)req.responseCode, req.error);

        var tex = DownloadHandlerTexture.GetContent(req);
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }

    public async Task<Sprite> GetCachedSpriteAsync(string url, int timeout = 30)
    {
        await EnsureCacheIndexLoaded();

        string fileName;
        lock (_cacheIndexLock)
        {
            if (_cacheIndex.TryGetValue(url, out fileName))
            {
                string cached = Path.Combine(_cacheDir, fileName);
                if (File.Exists(cached))
                    return LoadSpriteFromDisk(cached);
            }
        }

        var sem = _urlLocks.GetOrAdd(url, _ => new SemaphoreSlim(1, 1));
        await sem.WaitAsync();
        try
        {
            // Double-check after acquiring lock
            lock (_cacheIndexLock)
            {
                if (_cacheIndex.TryGetValue(url, out fileName))
                {
                    string cached = Path.Combine(_cacheDir, fileName);
                    if (File.Exists(cached))
                        return LoadSpriteFromDisk(cached);
                }
            }

            return await SaveSpriteToDiskAsync(url, timeout);
        }
        finally
        {
            sem.Release();
        }
    }

    // ── Cache internals ───────────────────────────────────────────────────────
    private async Task EnsureCacheIndexLoaded()
    {
        if (_cacheIndex != null) return;
        if (File.Exists(_cacheIndexPath))
        {
            string json = await Task.Run(() => File.ReadAllText(_cacheIndexPath));
            _cacheIndex = JsonConvert.DeserializeObject<Dictionary<string, string>>(json)?? new Dictionary<string, string>();
        }
        else
        {
            _cacheIndex = new Dictionary<string, string>();
        }
    }

    private async Task<Sprite> SaveSpriteToDiskAsync(string url, int timeout)
    {
        using var req = UnityWebRequestTexture.GetTexture(url);
        req.timeout = timeout;
        var op = req.SendWebRequest();
        while (!op.isDone) await Task.Yield();

        if (req.result != UnityWebRequest.Result.Success)
            throw new ApiException((System.Net.HttpStatusCode)req.responseCode, req.error);

        var tex = DownloadHandlerTexture.GetContent(req);
        byte[] png = tex.EncodeToPNG();

        string fileName = Guid.NewGuid() + ".png";
        string filePath = Path.Combine(_cacheDir, fileName);
        await Task.Run(() => File.WriteAllBytes(filePath, png));

        lock (_cacheIndexLock)
        {
            _cacheIndex[url] = fileName;
            File.WriteAllText(_cacheIndexPath, JsonConvert.SerializeObject(_cacheIndex));
        }

        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }

    private static Sprite LoadSpriteFromDisk(string path)
    {
        byte[] data = File.ReadAllBytes(path);
        var tex = new Texture2D(2, 2);
        tex.LoadImage(data);
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }
}