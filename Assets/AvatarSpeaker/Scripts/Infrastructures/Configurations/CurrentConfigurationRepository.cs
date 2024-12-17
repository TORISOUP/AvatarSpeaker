using System;
using AvatarSpeaker.Core.Configurations;
using R3;

namespace AvatarSpeaker.Infrastructures.Configurations
{
    public sealed class CurrentConfigurationRepository : IConfigurationRepository, IDisposable
    {
        public ReactiveProperty<VoiceControlConnectionSettings> VoiceControlConnectionSettings { get; }
        public ReactiveProperty<HttpServerSettings> HttpServerSettings { get; }

        public ReactiveProperty<bool> IsSubtitleEnabled { get; }　= new(true);

        public CurrentConfigurationRepository(VoiceControlConnectionSettings voiceControlConnectionSettings,
            HttpServerSettings httpServerSettings)
        {
            VoiceControlConnectionSettings =
                new ReactiveProperty<VoiceControlConnectionSettings>(voiceControlConnectionSettings);
            HttpServerSettings = new ReactiveProperty<HttpServerSettings>(httpServerSettings);
        }


        public void Dispose()
        {
            VoiceControlConnectionSettings.Dispose(true);
            HttpServerSettings.Dispose(true);
            IsSubtitleEnabled.Dispose(true);
        }
    }
}