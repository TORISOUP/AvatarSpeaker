using System;

namespace AvatarSpeaker.Core.Models
{
    public readonly struct SpeakRequest : IEquatable<SpeakRequest>
    {
        public string Text { get; }

        public SpeakRequest(string text)
        {
            Text = text;
        }

        public bool Equals(SpeakRequest other)
        {
            return Text == other.Text;
        }

        public override bool Equals(object obj)
        {
            return obj is SpeakRequest other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (Text != null ? Text.GetHashCode() : 0);
        }
    }
}