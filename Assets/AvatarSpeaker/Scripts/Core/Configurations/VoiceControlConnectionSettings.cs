using System;
using R3;

namespace AvatarSpeaker.Core.Configurations
{
    /// <summary>
    /// 現在のアプリケーション内での設定値を表す
    /// </summary>
    public sealed class CurrentConfigurationRepository : IDisposable
    {
        public ReadOnlyReactiveProperty<VoiceControlConnectionSettings> VoiceControlConnectionSettings =>
            _voiceControlConnectionSettings;

        private readonly ReactiveProperty<VoiceControlConnectionSettings> _voiceControlConnectionSettings;

        public CurrentConfigurationRepository(VoiceControlConnectionSettings voiceControlConnectionSettings)
        {
            _voiceControlConnectionSettings =
                new ReactiveProperty<VoiceControlConnectionSettings>(voiceControlConnectionSettings);
        }

        public void Dispose()
        {
            _voiceControlConnectionSettings.Dispose();
        }
    }


    /// <summary>
    /// 「音声の制御」を行うための接続設定を表す
    /// この設定値の解釈はInfrastructure層が行う
    /// </summary>
    public readonly struct VoiceControlConnectionSettings : IEquatable<VoiceControlConnectionSettings>
    {
        public string Address { get; }

        public VoiceControlConnectionSettings(string address)
        {
            Address = address;
        }

        public bool Equals(VoiceControlConnectionSettings other)
        {
            return Address == other.Address;
        }

        public override bool Equals(object obj)
        {
            return obj is VoiceControlConnectionSettings other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (Address != null ? Address.GetHashCode() : 0);
        }
    }
}