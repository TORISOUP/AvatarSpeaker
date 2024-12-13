using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AvatarSpeaker.Core
{
    /// <summary>
    /// 発話者の概念を表すクラス
    /// </summary>
    public abstract class Speaker : IDisposable, IEquatable<Speaker>
    {
        /// <summary>
        /// SpeakerのID
        /// </summary>
        public abstract string Id { get; }

        /// <summary>
        /// Speakerの顔の位置
        /// </summary>
        public abstract Vector3 FacePosition { get; }

        /// <summary>
        /// Speakerに発話させる
        /// </summary>
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