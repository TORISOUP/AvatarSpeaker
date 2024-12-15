using System;

namespace AvatarSpeaker.Core
{
    public readonly struct SpeakRequest : IEquatable<SpeakRequest>
    {
        public string Text { get; }
        public SpeakStyle Style { get; }
        public float SpeedScale { get; }
        public float PitchScale { get; }
        public float VolumeScale { get; }

        public SpeakRequest(string text, SpeakStyle style, float speedScale, float pitchScale, float volumeScale)
        {
            Text = text;
            Style = style;
            SpeedScale = speedScale;
            PitchScale = pitchScale;
            VolumeScale = volumeScale;
        }
        
        public SpeakRequest(string text)
        {
            Text = text;
            Style = new SpeakStyle(0, "Default");
            SpeedScale = 1.0f;
            PitchScale = 1.0f;
            VolumeScale = 1.0f;
        }



        public bool Equals(SpeakRequest other)
        {
            return Text == other.Text && Style.Equals(other.Style) && SpeedScale.Equals(other.SpeedScale) &&
                   PitchScale.Equals(other.PitchScale) && VolumeScale.Equals(other.VolumeScale);
        }

        public override bool Equals(object obj)
        {
            return obj is SpeakRequest other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Text, Style, SpeedScale, PitchScale, VolumeScale);
        }
    }

    public readonly struct SpeakStyle : IEquatable<SpeakStyle>
    {
        public int Id { get; }
        public string DisplayName { get; }

        public SpeakStyle(int id, string displayName)
        {
            Id = id;
            DisplayName = displayName;
        }

        public bool Equals(SpeakStyle other)
        {
            return Id == other.Id && DisplayName == other.DisplayName;
        }

        public override bool Equals(object obj)
        {
            return obj is SpeakStyle other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, DisplayName);
        }
    }
}