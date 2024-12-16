#nullable enable
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
        /// 現在のポーズ
        /// </summary>
        public ReadOnlyReactiveProperty<IdlePose> CurrentIdlePose => _currentIdlePose;

        private readonly ReactiveProperty<IdlePose> _currentIdlePose = new(Models.IdlePose.Pose1);

        /// <summary>
        /// 現在の発話パラメータ
        /// </summary>
        public ReadOnlyReactiveProperty<SpeakParameter> CurrentSpeakParameter => _currentSpeakParameter;

        private readonly ReactiveProperty<SpeakParameter> _currentSpeakParameter = new(SpeakParameter.Default);

        /// <summary>
        /// 現在発話中のテキスト
        /// </summary>
        public abstract ReadOnlyReactiveProperty<string> CurrentSpeakingText { get; }

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
        /// 現在Speakerが保持する設定値で発話させる
        /// </summary>
        public abstract UniTask SpeakAsync(string text, CancellationToken ct);

        /// <summary>
        /// Speakerに発話させる
        /// </summary>
        public abstract UniTask SpeakAsync(string text, SpeakParameter speakParameter, CancellationToken ct);


        /// <summary>
        /// Dispose時に発火するUniTask
        /// </summary>
        public abstract UniTask OnDisposeAsync { get; }


        /// <summary>
        /// 現在のポーズを変更する
        /// </summary>
        public void ChangeIdlePose(IdlePose idlePose)
        {
            _currentIdlePose.Value = idlePose;
        }

        /// <summary>
        /// 発話パラメータを変更する
        /// </summary>
        public void ChangeSpeakParameter(SpeakParameter speakParameter)
        {
            // 無効な値の場合は変更しない
            if (!speakParameter.Validate()) return;
            
            _currentSpeakParameter.Value = speakParameter;
        }


        public void Dispose()
        {
            _currentIdlePose.Dispose();
            _currentSpeakParameter.Dispose();
            OnDisposed();
        }

        protected virtual void OnDisposed()
        {
            //
        }

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