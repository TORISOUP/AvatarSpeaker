using System;
using AvatarSpeaker.Core.Configurations;
using R3;
using VoicevoxClientSharp;
using VoicevoxClientSharp.ApiClient;

namespace AvatarSpeaker.Infrastructures.Voicevoxs
{
    /// <summary>
    /// 現在のアプリケーション上で有効なVoicevoxSynthesizerを提供する
    /// </summary>
    public sealed class VoicevoxSynthesizerProvider : IDisposable
    {
        public ReadOnlyReactiveProperty<VoicevoxSynthesizer> Current => _current;
        private readonly ReactiveProperty<VoicevoxSynthesizer> _current = new(new VoicevoxSynthesizer());
        private IVoicevoxApiClient _currentApiClient;
        private readonly IDisposable _disposable;

        public VoicevoxSynthesizerProvider(CurrentConfigurationRepository configurationRepository)
        {
            _disposable = configurationRepository.VoiceControlConnectionSettings.Subscribe(ChangeConnectionSettings);
        }

        private void ChangeConnectionSettings(VoiceControlConnectionSettings settings)
        {
            if (string.IsNullOrEmpty(settings.Address)) return;

            _currentApiClient?.Dispose();
            _current.Value?.Dispose();

            _currentApiClient = VoicevoxApiClient.Create(settings.Address);
            _current.Value = new VoicevoxSynthesizer(_currentApiClient);
        }


        public void Dispose()
        {
            _current.Value?.Dispose();
            _current.Dispose(true);
            _disposable.Dispose();
        }
    }
}