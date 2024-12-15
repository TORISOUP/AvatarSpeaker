using System;
using AvatarSpeaker.Core;
using AvatarSpeaker.Core.Interfaces;
using R3;

namespace AvatarSpeaker.Infrastructures.Https
{
    /// <summary>
    /// Http経由での発話リクエストを提供する
    /// </summary>
    public sealed class HttpSpeakRequestProvider : ISpeakRequestProvider, IDisposable
    {
        private readonly Subject<SpeakRequest> _speakRequestSubject = new();

        public Observable<SpeakRequest> OnSpeakRequest => _speakRequestSubject;
        
        public HttpSpeakRequestProvider(MiniHttpServer<SpeakRequestDto> httpServer)
        {
            httpServer.OnRequestReceived += dto =>
            {
                var speakRequest = new SpeakRequest(dto.Text, new SpeakStyle(dto.Style, ""), dto.SpeedScale,
                    dto.PitchScale, dto.VolumeScale);
                _speakRequestSubject.OnNext(speakRequest);
            };

            httpServer.Start();
        }

        public void Dispose()
        {
            _speakRequestSubject.Dispose(true);
        }
    }
}