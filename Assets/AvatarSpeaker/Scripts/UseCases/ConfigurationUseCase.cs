using AvatarSpeaker.Core.Configurations;
using R3;

namespace AvatarSpeaker.UseCases
{
    public sealed class ConfigurationUseCase
    {
        private readonly IConfigurationRepository _configurationRepository;

        public ConfigurationUseCase(IConfigurationRepository configurationRepository)
        {
            _configurationRepository = configurationRepository;
        }

        public ReadOnlyReactiveProperty<VoiceControlConnectionSettings> VoiceControlConnectionSettings =>
            _configurationRepository.VoiceControlConnectionSettings;

        public void SetVoiceControlConnectionSettings(string path)
        {
            _configurationRepository.VoiceControlConnectionSettings.Value = new VoiceControlConnectionSettings(path);
        }

        public ReadOnlyReactiveProperty<HttpServerSettings> HttpServerSettings =>
            _configurationRepository.HttpServerSettings;

        public void SetHttpServerSettings(int port, bool isEnabled)
        {
            _configurationRepository.HttpServerSettings.Value = new HttpServerSettings(port, isEnabled);
        }
        
        public ReadOnlyReactiveProperty<bool> IsSubtitleEnabled =>
            _configurationRepository.IsSubtitleEnabled;

        public void SetIsSubtitleEnabled(bool isEnabled)
        {
            _configurationRepository.IsSubtitleEnabled.Value = isEnabled;
        }
    }
}