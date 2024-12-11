#nullable enable
using System;
using AvatarSpeaker.Core.Models;
using R3;


namespace AvatarSpeaker.Core
{
    /// <summary>
    /// 「空間の状態」を管理する
    /// </summary>
    public sealed class RoomSpace : IDisposable
    {
        public ReadOnlyReactiveProperty<Color> BackgroundColor => _backgroundColor;
        private readonly ReactiveProperty<Color> _backgroundColor = new(Color.White);

        /// <summary>
        /// Speakerは1つだけ配置できる
        /// </summary>
        public ReadOnlyReactiveProperty<Speaker?> CurrentSpeaker => _currentSpeaker;

        private readonly ReactiveProperty<Speaker?> _currentSpeaker = new();

        /// <summary>
        /// SpeakerをRoomSpaceから削除する
        /// </summary>
        public void RemoveSpeaker(string speakerId)
        {
            if (_currentSpeaker.Value == null)
            {
                return;
            }

            if (_currentSpeaker.Value.Id != speakerId)
            {
                return;
            }

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

        public void Dispose()
        {
            _backgroundColor.Dispose();
            _currentSpeaker.Dispose();
        }
    }
}