using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using System;

public class MockHttpServer : IDisposable
{
    private readonly HttpListener _listener;
    private bool _isRunning;

    public string ServerUrl { get; }

    // Configurable behaviors for our tests to manipulate
    public int ResponseStatusCode { get; set; } = 200;
    public object ResponseObject { get; set; } = null;
    public byte[] ResponseBytes { get; set; } = null;
    public int DelayMilliseconds { get; set; } = 0;
    public Action<HttpListenerRequest> OnRequestReceived { get; set; }

    public MockHttpServer(int port)
    {
        ServerUrl = $"http://127.0.0.1:{port}/";
        _listener = new HttpListener();
        _listener.Prefixes.Add(ServerUrl);
    }

    public void Start()
    {
        _listener.Start();
        _isRunning = true;
        Task.Run(ListenLoop);
    }

    private async Task ListenLoop()
    {
        while (_isRunning)
        {
            try
            {
                var context = await _listener.GetContextAsync();
                _ = ProcessRequestAsync(context);
            }
            catch (HttpListenerException)
            {
                // Normal when listener is stopped
            }
        }
    }

    private async Task ProcessRequestAsync(HttpListenerContext context)
    {
        // 1. Trigger the spy hook so our tests can inspect headers/URLs
        OnRequestReceived?.Invoke(context.Request);

        // 2. Simulate network latency if the test requested it
        if (DelayMilliseconds > 0)
        {
            await Task.Delay(DelayMilliseconds);
        }

        // 3. Formulate the response
        context.Response.StatusCode = ResponseStatusCode;

        var buffer = ResponseBytes ?? Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(ResponseObject));
        context.Response.ContentLength64 = buffer.Length;

        // 4. Send the response
        try
        {
            await using var output = context.Response.OutputStream;
            await output.WriteAsync(buffer, 0, buffer.Length);
        }
        catch
        {
            // Client might have cancelled the request early
        }
        finally
        {
            context.Response.Close();
        }
    }

    public void Dispose()
    {
        _isRunning = false;
        if (_listener.IsListening)
        {
            _listener.Stop();
        }

        _listener.Close();
    }
}