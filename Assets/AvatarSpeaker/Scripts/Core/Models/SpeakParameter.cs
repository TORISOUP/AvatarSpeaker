using System;

namespace AvatarSpeaker.Core.Models
{
    /// <summary>
    /// 発話パラメータ
    /// </summary>
    public readonly struct SpeakParameter : IEquatable<SpeakParameter>
    {
        /// <summary>
        /// 発話スタイル
        /// </summary>
        public SpeakStyle Style { get; }

        /// <summary>
        /// 読み上げ速度
        /// </summary>
        public float SpeedScale { get; }

        /// <summary>
        /// ピッチ
        /// </summary>
        public float PitchScale { get; }

        /// <summary>
        /// 音量
        /// </summary>
        public float VolumeScale { get; }

        public static SpeakParameter Default => new(new SpeakStyle(0, "Default"), 1.0f, 0.0f, 1.0f);

        public SpeakParameter(SpeakStyle style, float speedScale, float pitchScale, float volumeScale)
        {
            Style = style;
            SpeedScale = speedScale;
            PitchScale = pitchScale;
            VolumeScale = volumeScale;
        }

        public SpeakParameter Clone(
            SpeakStyle? style = null,
            float? speedScale = null,
            float? pitchScale = null,
            float? volumeScale = null)
        {
            return new SpeakParameter(style ?? Style, speedScale ?? SpeedScale, pitchScale ?? PitchScale,
                volumeScale ?? VolumeScale);
        }

        public bool Equals(SpeakParameter other)
        {
            return Style.Equals(other.Style) && SpeedScale.Equals(other.SpeedScale) &&
                   PitchScale.Equals(other.PitchScale) && VolumeScale.Equals(other.VolumeScale);
        }

        public override bool Equals(object obj)
        {
            return obj is SpeakParameter other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Style, SpeedScale, PitchScale, VolumeScale);
        }

        public bool Validate()
        {
            if (Style.Id < 0) return false;

            if (SpeedScale < 0.5f || SpeedScale > 2.0f) return false;

            if (PitchScale < -0.15f || PitchScale > 0.15f) return false;

            if (VolumeScale < 0.0f || VolumeScale > 2.0f) return false;

            return true;
        }
    }


    /// <summary>
    /// 発話スタイルを表す
    /// （事実上、VOICEVOXのStyleIdに紐づく）
    /// </summary>
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