using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace AvatarSpeaker.Infrastructures.Https
{
    public sealed class MiniHttpServer<T> : IDisposable
    {
        private readonly HttpListener _httpListener;
        private CancellationTokenSource _cts = new();
        public Action<T> OnRequestReceived { get; set; }
        private bool _isDisposed;

        public MiniHttpServer(int port)
        {
            _httpListener = new HttpListener();

            _httpListener.Prefixes.Clear();
            _httpListener.Prefixes.Add($"http://127.0.0.1:{port}/");
        }

        public void Start()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(MiniHttpServer<T>));
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            _httpListener.Start();
            _ = HandleRequestsAsync(_cts.Token);
        }

        public void Stop()
        {
            if (_isDisposed) return;

            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            _httpListener.Stop();
        }

        private async ValueTask HandleRequestsAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    var context = await _httpListener.GetContextAsync().ConfigureAwait(false);
                    ct.ThrowIfCancellationRequested();
                    var request = context.Request;
                    var response = context.Response;
                    try
                    {
                        if (request.HttpMethod == "POST")
                        {
                            T v;
                            try
                            {
                                using var reader = new StreamReader(request.InputStream);
                                var json = await reader.ReadToEndAsync();
                                v = JsonSerializer.Deserialize<T>(json);
                            }
                            catch (Exception)
                            {
                                response.StatusCode = (int)HttpStatusCode.BadRequest;
                                response.Close();
                                continue;
                            }

                            OnRequestReceived?.Invoke(v);

                            response.StatusCode = (int)HttpStatusCode.OK;
                            response.Close();
                        }
                        else
                        {
                            response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                            response.Close();
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        response.Close();
                    }
                }
                catch (ObjectDisposedException)
                {
                    // ignore
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    Debug.LogException(ex);
                }
            }
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            Stop();
            ((IDisposable)_httpListener)?.Dispose();
            _isDisposed = true;
        }
    }
}