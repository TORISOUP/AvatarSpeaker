using System;

namespace AvatarSpeaker.Core.Configurations
{
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
}