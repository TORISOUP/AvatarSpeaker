#nullable enable
using System;
using System.Threading;
using AvatarSpeaker.Core;
using AvatarSpeaker.Core.Interfaces;
using AvatarSpeaker.Core.Models;
using Cysharp.Threading.Tasks;
using R3;

namespace AvatarSpeaker.UseCases
{
    /// <summary>
    /// Speaker関係の操作を提供するUseCase
    /// </summary>
    public sealed class SpeakerUseCase : IDisposable
    {
        private readonly ISpeakerSourceProvider _speakerSourceProvider;
        private readonly IRoomSpaceProvider _roomSpaceProvider;
        private readonly ISpeakStyleProvider _speakStyleProvider;
        private readonly IDisposable _speakerChangedDisposable;


        /// <summary>
        /// RoomSpaceのSpeakerが変更されたときに発行されるObservable
        /// </summary>
        public Observable<Unit> OnSpeakerChanged { get; }


        public SpeakerUseCase(ISpeakerSourceProvider speakerSourceProvider,
            IRoomSpaceProvider roomSpaceProvider,
            ISpeakStyleProvider speakStyleProvider)
        {
            _speakerSourceProvider = speakerSourceProvider;
            _roomSpaceProvider = roomSpaceProvider;
            _speakStyleProvider = speakStyleProvider;

            var connectable =
                _roomSpaceProvider.CurrentRoomSpace.Select(x => x.CurrentSpeaker)
                    .Where(x => x != null)
                    .Select(x => x.Select(y => y != null))
                    .Switch()
                    .Select(_ => Unit.Default)
                    .Publish();
            _speakerChangedDisposable = connectable.Connect();
            OnSpeakerChanged = connectable;
        }

        /// <summary>
        /// 使用可能なSpeakerSourceを一覧で取得する
        /// </summary>
        public UniTask<ISpeakerSource[]> GetAvailableSpeakerSourcesAsync(CancellationToken ct)
        {
            return _speakerSourceProvider.GetSpeakerSourcesAsync(ct);
        }

        /// <summary>
        /// 使用可能なSpeakStyleを一覧で取得する
        /// </summary>
        public UniTask<SpeakStyle[]> GetSpeechStylesAsync(CancellationToken ct)
        {
            return _speakStyleProvider.GetSpeechStylesAsync(ct);
        }


        /// <summary>
        /// 現在のRoomSpaceのSpeakerを発話させる
        /// </summary>
        public async UniTask SpeakByCurrentSpeakerAsync(SpeakRequest speakRequest, CancellationToken ct)
        {
            // 現在のRoomSpaceのSpeakerを取得して発話させる
            var roomSpace = await _roomSpaceProvider.CurrentRoomSpace
                .FirstAsync(x => x != null, ct);

            var speaker = roomSpace.CurrentSpeaker.CurrentValue;
            if (speaker == null)
            {
                return;
            }

            await speaker.SpeakAsync(speakRequest, ct);
        }


        /// <summary>
        /// 現在のRoomSpaceのSpeakerを発話させる
        /// </summary>
        public async UniTask SpeakByCurrentSpeakerAsync(string text, SpeakStyle style, CancellationToken ct)
        {
            var speakParameter = new SpeakRequest(text, style, 1.0f, 1.0f, 1.0f);

            // 現在のRoomSpaceのSpeakerを取得して発話させる
            var roomSpace = await _roomSpaceProvider.CurrentRoomSpace
                .FirstAsync(x => x != null, ct);

            var speaker = roomSpace.CurrentSpeaker.CurrentValue;
            if (speaker == null)
            {
                return;
            }

            await speaker.SpeakAsync(speakParameter, ct);
        }
        
        public void ChangeIdlePoseToCurrentSpeaker(IdlePose idlePose)
        {
            var roomSpace = _roomSpaceProvider.CurrentRoomSpace.CurrentValue;
            if (roomSpace == null)
            {
                return;
            }

            var speaker = roomSpace.CurrentSpeaker.CurrentValue;
            if (speaker == null)
            {
                return;
            }

            speaker.ChangeIdlePose(idlePose);
        }
        
        public Speaker? GetCurrentSpeaker()
        {
            var roomSpace = _roomSpaceProvider.CurrentRoomSpace.CurrentValue;
            if (roomSpace == null)
            {
                return null;
            }

            return roomSpace.CurrentSpeaker.CurrentValue;
        }

        public void Dispose()
        {
            _speakerChangedDisposable.Dispose();
        }
    }
}