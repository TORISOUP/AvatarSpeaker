using System;
using System.Collections.Generic;
using System.Threading;
using AvatarSpeaker.Core.Configurations;
using R3;

namespace AvatarSpeaker.Http.Server
{
    /// <summary>
    /// HTTPServerの実行を管理する
    /// </summary>
    public sealed class HttpServerRunner : IDisposable
    {
        private readonly IConfigurationRepository _configurationRepository;

        private readonly IEnumerable<BaseController> _controllers;
        private readonly CancellationTokenSource _cts = new();
        private readonly HttpServer _currentHttpServer;
        
        public HttpServerRunner(IEnumerable<BaseController> controllers,
            IConfigurationRepository configurationRepository)
        {
            _currentHttpServer = new HttpServer();
            _controllers = controllers;
            _configurationRepository = configurationRepository;
        }


        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
            _currentHttpServer?.Dispose();
        }

        public void Start()
        {
            // Controllerを登録
            foreach (var controller in _controllers)
            {
                _currentHttpServer.RegisterController(controller);
            }

            // 設定を監視してサーバーを起動/停止
            _configurationRepository
                .HttpServerSettings
                .Subscribe(x =>
                {
                    if (x.IsEnabled)
                        _currentHttpServer.Start(x.Port);
                    else
                        _currentHttpServer.Stop();
                })
                .RegisterTo(_cts.Token);
        }
    }
}