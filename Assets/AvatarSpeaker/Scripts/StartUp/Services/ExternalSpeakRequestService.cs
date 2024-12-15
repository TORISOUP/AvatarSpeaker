using System;
using System.Threading;
using AvatarSpeaker.Core.Interfaces;
using AvatarSpeaker.UseCases;
using Cysharp.Threading.Tasks;
using R3;
using VContainer.Unity;

namespace AvatarSpeaker.StartUp.Services
{
    /// <summary>
    /// 外部からの発話リクエストを実行する
    /// </summary>
    public sealed class ExternalSpeakRequestService : IInitializable,  IDisposable
    {
        private readonly CancellationTokenSource _cts = new();

        private readonly SpeakerUseCase _speakerUseCase;
        private readonly ISpeakRequestProvider _speakRequestProvider;

        public ExternalSpeakRequestService(SpeakerUseCase speakerUseCase, ISpeakRequestProvider speakRequestProvider)
        {
            _speakerUseCase = speakerUseCase;
            _speakRequestProvider = speakRequestProvider;
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }

        public void Initialize()
        {
            //　外部からの発話リクエストをSpeakerUseCaseに渡す
            _speakRequestProvider.OnSpeakRequest.Subscribe(request =>
            {
                _speakerUseCase.SpeakByCurrentSpeakerAsync(request.Text, _cts.Token).Forget();
            });
        }
    }
}