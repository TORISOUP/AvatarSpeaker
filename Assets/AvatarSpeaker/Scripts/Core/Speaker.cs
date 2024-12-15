using System;
using System.Threading;
using AvatarSpeaker.Core.Models;
using Cysharp.Threading.Tasks;
using R3;
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
        /// 体の前方を表すベクトル
        /// </summary>
        public abstract Vector3 BodyForward { get; }

        /// <summary>
        /// Speakerに発話させる
        /// </summary>
        public abstract UniTask SpeakAsync(SpeakRequest speakRequest, CancellationToken ct);

        /// <summary>
        /// Dispose時に発火するUniTask
        /// </summary>
        public abstract UniTask OnDisposeAsync { get; }

        /// <summary>
        /// 現在のポーズ
        /// </summary>
        public abstract ReadOnlyReactiveProperty<IdlePose> IdlePose { get; }

        /// <summary>
        /// 現在のポーズを変更する
        /// </summary>
        public abstract void ChangeIdlePose(IdlePose idlePose);

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