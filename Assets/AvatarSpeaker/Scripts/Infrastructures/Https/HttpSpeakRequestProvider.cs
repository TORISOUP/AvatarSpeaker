using System;
using System.Threading;
using AvatarSpeaker.Core;
using AvatarSpeaker.Core.Interfaces;
using Cysharp.Threading.Tasks;
using R3;

namespace AvatarSpeaker.Infrastructures.Https
{
    /// <summary>
    /// Http経由での発話リクエストを提供する
    /// </summary>
    public sealed class HttpSpeakRequestProvider : ISpeakRequestProvider, IDisposable
    {
        private readonly Subject<SpeakRequest> _speakRequestSubject = new();
        private readonly CancellationTokenSource _cts = new();
        public Observable<SpeakRequest> OnSpeakRequest => _speakRequestSubject;
        
        
        public HttpSpeakRequestProvider(MiniHttpServer<SpeakRequestDto> httpServer)
        {
            httpServer.OnRequestReceived += dto =>
            {
                UniTask.Void(async () =>
                {
                    var speakRequest = new SpeakRequest(dto.Text, new SpeakStyle(dto.Style, ""), dto.SpeedScale,
                        dto.PitchScale, dto.VolumeScale);
                    await UniTask.SwitchToMainThread(_cts.Token);
                    _speakRequestSubject.OnNext(speakRequest);
                });
       
            };

            httpServer.Start();
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
            _speakRequestSubject.Dispose(true);
        }
    }
}