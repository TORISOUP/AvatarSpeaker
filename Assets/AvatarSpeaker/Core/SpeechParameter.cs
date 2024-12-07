using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace AvatarSpeaker.Core
{
    public readonly struct SpeechParameter : IEquatable<SpeechParameter>
    {
        public string Text { get; }
        public SpeechStyle Style { get; }
        public float SpeedScale { get; }
        public float PitchScale { get; }
        public float VolumeScale { get; }

        public SpeechParameter(string text, SpeechStyle style, float speedScale, float pitchScale, float volumeScale)
        {
            Text = text;
            Style = style;
            SpeedScale = speedScale;
            PitchScale = pitchScale;
            VolumeScale = volumeScale;
        }


        public bool Equals(SpeechParameter other)
        {
            return Text == other.Text && Style.Equals(other.Style) && SpeedScale.Equals(other.SpeedScale) &&
                   PitchScale.Equals(other.PitchScale) && VolumeScale.Equals(other.VolumeScale);
        }

        public override bool Equals(object obj)
        {
            return obj is SpeechParameter other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Text, Style, SpeedScale, PitchScale, VolumeScale);
        }
    }

    public readonly struct SpeechStyle : IEquatable<SpeechStyle>
    {
        public int Id { get; }
        public string DisplayName { get; }

        public SpeechStyle(int id, string displayName)
        {
            Id = id;
            DisplayName = displayName;
        }

        public bool Equals(SpeechStyle other)
        {
            return Id == other.Id && DisplayName == other.DisplayName;
        }

        public override bool Equals(object obj)
        {
            return obj is SpeechStyle other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, DisplayName);
        }
    }

    public interface ISpeechStyleProvider
    {
        UniTask<SpeechStyle[]> GetSpeechStylesAsync(CancellationToken ct);
    }
}