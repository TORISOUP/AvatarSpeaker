using System;

namespace AvatarSpeaker.Core.Models
{
    public readonly struct Color : IEquatable<Color>
    {
        public static Color White => new(1, 1, 1);
        
        public float Red { get; }
        public float Green { get; }
        public float Blue { get; }

        public Color(float red, float green, float blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public bool Equals(Color other)
        {
            return Red.Equals(other.Red) && Green.Equals(other.Green) && Blue.Equals(other.Blue);
        }

        public override bool Equals(object obj)
        {
            return obj is Color other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Red, Green, Blue);
        }
    }
}