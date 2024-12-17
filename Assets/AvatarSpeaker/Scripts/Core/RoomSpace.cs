#nullable enable
using System;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace AvatarSpeaker.Core
{
    /// <summary>
    /// 「空間の状態」を管理する
    /// </summary>
    public sealed class RoomSpace : IDisposable
    {
        private readonly ReactiveProperty<Color> _backgroundColor;
        private readonly ReactiveProperty<Speaker?> _currentSpeaker;
        private readonly UniTaskCompletionSource _disposeTaskCompletionSource = new();

        public readonly SpeakerCamera SpeakerCamera;

        public RoomSpace()
        {
            _backgroundColor = new ReactiveProperty<Color>(Color.green);
            _currentSpeaker = new ReactiveProperty<Speaker?>(null);
            SpeakerCamera = new SpeakerCamera();
        }

        public ReadOnlyReactiveProperty<Color> BackgroundColor => _backgroundColor;

        /// <summary>
        /// Speakerは1つだけ配置できる
        /// </summary>
        public ReadOnlyReactiveProperty<Speaker?> CurrentSpeaker => _currentSpeaker;

        /// <summary>
        /// Dispose時に完了するUniTask
        /// </summary>
        public UniTask OnDisposeAsync => _disposeTaskCompletionSource.Task;

        public void Dispose()
        {
            // Speakerを破棄する
            _currentSpeaker.Value?.Dispose();

            // SpeakerCameraを破棄する
            SpeakerCamera.Dispose();

            _backgroundColor.Dispose();
            _currentSpeaker.Dispose();

            _disposeTaskCompletionSource.TrySetResult();
        }

        /// <summary>
        /// SpeakerをRoomSpaceから削除する
        /// </summary>
        public void RemoveSpeaker(string speakerId)
        {
            if (_currentSpeaker.Value == null) return;

            if (_currentSpeaker.Value.Id != speakerId) return;

            _currentSpeaker.Value.Dispose();
            _currentSpeaker.Value = null;
        }

        /// <summary>
        /// SpeakerをRoomSpaceに登録する
        /// </summary>
        public void RegisterSpeaker(Speaker speaker)
        {
            if (_currentSpeaker.Value != null)
            {
                if (_currentSpeaker.Value.Equals(speaker))
                {
                    // すでに登録されているSpeakerと同じSpeakerを登録しようとした場合は何もしない
                    return;
                }

                throw new InvalidOperationException("Speaker is already registered.");
            }

            // Speakerを登録する
            _currentSpeaker.Value = speaker;
        }

        public void ChangeBackgroundColor(Color color)
        {
            _backgroundColor.Value = color;
        }
    }
}