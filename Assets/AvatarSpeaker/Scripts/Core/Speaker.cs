using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace AvatarSpeaker.Core
{
    public abstract class Speaker : IDisposable, IEquatable<Speaker>
    {
        public abstract string Id { protected set; get; }
        public abstract UniTask SpeechAsync(SpeechParameter speechParameter, CancellationToken ct);

        public abstract void Dispose();

        public bool Equals(Speaker other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Id == other.Id;
        }
    }
}