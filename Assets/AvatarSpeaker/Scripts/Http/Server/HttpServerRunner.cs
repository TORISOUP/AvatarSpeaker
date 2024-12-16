using System;
using System.Collections.Generic;
using System.Threading;
using AvatarSpeaker.Core.Configurations;
using R3;

namespace AvatarSpeaker.Http.Server
{
    public sealed class HttpServerRunner : IDisposable
    {
        private readonly HttpServer _currentHttpServer;
        private readonly CancellationTokenSource _cts = new();

        private readonly IEnumerable<BaseController> _controllers;
        private readonly IConfigurationRepository _configurationRepository;


        public HttpServerRunner(IEnumerable<BaseController> controllers, IConfigurationRepository configurationRepository)
        {
            _currentHttpServer = new HttpServer();
            _controllers = controllers;
            _configurationRepository = configurationRepository;
        }

        public void Start()
        {
            foreach (var controller in _controllers)
            {
                _currentHttpServer.RegisterController(controller);
            }

            _configurationRepository
                .HttpServerSettings
                .Subscribe(x =>
                {
                    if (x.IsEnabled)
                    {
                        _currentHttpServer.Start(x.Port);
                    }
                    else
                    {
                        _currentHttpServer.Stop();
                    }
                })
                .RegisterTo(_cts.Token);
        }


        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
            _currentHttpServer?.Dispose();
        }
    }
}