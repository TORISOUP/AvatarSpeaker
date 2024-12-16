using System;
using R3;

namespace AvatarSpeaker.Core.Configurations
{
    /// <summary>
    /// 現在のアプリケーション内での設定値を表す
    /// </summary>
    public interface IConfigurationRepository
    {
        /// <summary>
        /// 「音声の制御」を行うための接続設定
        /// </summary>
        ReactiveProperty<VoiceControlConnectionSettings> VoiceControlConnectionSettings { get; }

        /// <summary>
        /// Httpサーバーの設定
        /// </summary>
        ReactiveProperty<HttpServerSettings> HttpServerSettings { get; }
    }

    public readonly struct HttpServerSettings : IEquatable<HttpServerSettings>
    {
        public int Port { get; }
        public bool IsEnabled { get; }

        public HttpServerSettings(int port, bool isEnabled)
        {
            Port = port;
            IsEnabled = isEnabled;
        }

        public bool Equals(HttpServerSettings other)
        {
            return Port == other.Port && IsEnabled == other.IsEnabled;
        }

        public override bool Equals(object obj)
        {
            return obj is HttpServerSettings other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Port, IsEnabled);
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