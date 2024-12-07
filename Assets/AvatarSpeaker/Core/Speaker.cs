using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace AvatarSpeaker.Core
{
    public abstract class Speaker : IDisposable
    {
        public abstract UniTask SpeechAsync(SpeechParameter speechParameter, CancellationToken ct);

        public abstract void Dispose();
    }
}