using System;
using AvatarSpeaker.Core.Configurations;
using AvatarSpeaker.Infrastructures.Configurations;
using R3;
using VoicevoxClientSharp;
using VoicevoxClientSharp.ApiClient;

namespace AvatarSpeaker.Infrastructures.Voicevoxes
{
    /// <summary>
    /// 現在のアプリケーション上で有効なVoicevoxの制御クライアントを提供する
    /// </summary>
    public sealed class VoicevoxProvider : IDisposable
    {
        public ReadOnlyReactiveProperty<VoicevoxSynthesizer> Synthesizer => _synthesizer;
        private readonly ReactiveProperty<VoicevoxSynthesizer> _synthesizer;

        public ReadOnlyReactiveProperty<IVoicevoxApiClient> ApiClient => _apiClient;
        private readonly ReactiveProperty<IVoicevoxApiClient> _apiClient;

        private readonly IDisposable _disposable;

        public VoicevoxProvider(IConfigurationRepository configurationRepository)
        {
            var apiClient = VoicevoxApiClient.Create();
            var synthesizer = new VoicevoxSynthesizer(apiClient);

            _synthesizer = new ReactiveProperty<VoicevoxSynthesizer>(synthesizer);
            _apiClient = new ReactiveProperty<IVoicevoxApiClient>(apiClient);

            _disposable = configurationRepository.VoiceControlConnectionSettings.Subscribe(ChangeConnectionSettings);
        }

        private void ChangeConnectionSettings(VoiceControlConnectionSettings settings)
        {
            if (string.IsNullOrEmpty(settings.Address)) return;

            _apiClient.Value?.Dispose();
            _synthesizer.Value?.Dispose();

            _apiClient.Value = VoicevoxApiClient.Create(settings.Address);
            _synthesizer.Value = new VoicevoxSynthesizer(_apiClient.Value);
        }


        public void Dispose()
        {
            _synthesizer.Value?.Dispose();
            _synthesizer.Dispose(true);
            _disposable.Dispose();
        }
    }
}