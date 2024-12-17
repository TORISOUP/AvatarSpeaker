using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AvatarSpeaker.Http.Server
{
    /// <summary>
    /// HTTPサーバー
    /// </summary>
    public sealed class HttpServer : IDisposable
    {
        private delegate ValueTask Handler(HttpListenerRequest req, HttpListenerResponse res, CancellationToken ct);

        private readonly HttpListener _httpListener = new();

        private readonly Dictionary<(string, string), Handler> _routes = new();
        private CancellationTokenSource _cts = new();
        private bool _isDisposed;
        
        public void Dispose()
        {
            if (_isDisposed) return;

            Stop();
            ((IDisposable)_httpListener)?.Dispose();
            _isDisposed = true;
        }

        private void AddGet(string localPath, Handler handler) => Method("GET", localPath, handler);
        private void AddPost(string localPath, Handler handler) => Method("POST", localPath, handler);
        private void AddPut(string localPath, Handler handler) => Method("PUT", localPath, handler);
        private void AddDelete(string localPath, Handler handler) => Method("DELETE", localPath, handler);
        private void Method(string httpMethod, string localPath, Handler handler)
        {
            _routes.Add((httpMethod, localPath), handler);
        }


        public void Start(int port)
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(HttpServer));

            Stop();
            _cts = new CancellationTokenSource();

            _httpListener.Prefixes.Clear();
            _httpListener.Prefixes.Add($"http://127.0.0.1:{port}/");

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
                        if (_routes.TryGetValue((request.HttpMethod, request.Url.LocalPath), out var handler))
                        {
                            // メインスレッドに切り替えてから処理
                            await UniTask.SwitchToMainThread();
                            await handler(request, response, ct);
                        }
                        else
                        {
                            response.StatusCode = (int)HttpStatusCode.NotFound;
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

        /// <summary>
        /// Controllerを登録する
        /// Reflectionを使っているので、パフォーマンスに注意
        /// </summary>
        public void RegisterController(BaseController baseController)
        {
            var type = baseController.GetType();
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);

            foreach (var method in methods)
            {
                var h = method.GetCustomAttribute<RouterAttribute>();
                if (h == null) continue;

                var handler = new Handler((req, res, ct) =>
                    (ValueTask)method.Invoke(baseController, new object[] { req, res, ct }));

                switch (h)
                {
                    case Get:
                        AddGet(h.LocalPath, handler);
                        break;
                    case Post:
                        AddPost(h.LocalPath, handler);
                        break;
                    case Put:
                        AddPut(h.LocalPath, handler);
                        break;
                    case Delete:
                        AddDelete(h.LocalPath, handler);
                        break;
                }
            }
        }
    }
}