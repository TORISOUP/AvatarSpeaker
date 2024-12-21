#nullable enable
using System.Threading;
using AvatarSpeaker.Core;
using AvatarSpeaker.Core.Interfaces;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace AvatarSpeaker.UseCases
{
    /// <summary>
    /// RoomSpaceに関する操作を提供するUseCase
    /// </summary>
    public sealed class RoomSpaceUseCase
    {
        private readonly IRoomSpaceProvider _roomSpaceProvider;
        private readonly IRoomSpaceRegister _roomSpaceRegister;
        private readonly SpeakerCameraUseCase _speakerCameraUseCase;
        private readonly ISpeakerProvider _speakerProvider;

        public RoomSpaceUseCase(IRoomSpaceProvider roomSpaceProvider,
            IRoomSpaceRegister roomSpaceRegister,
            ISpeakerProvider speakerProvider,
            SpeakerCameraUseCase speakerCameraUseCase)
        {
            _roomSpaceProvider = roomSpaceProvider;
            _roomSpaceRegister = roomSpaceRegister;
            _speakerProvider = speakerProvider;
            _speakerCameraUseCase = speakerCameraUseCase;
        }

        public RoomSpace? CurrentRoomSpace => _roomSpaceProvider.CurrentRoomSpace.CurrentValue;


        /// <summary>
        /// 新しいRoomSpaceを作成し、古いRoomSpaceを破棄する
        /// </summary>
        public void CreateNewRoomSpace()
        {
            // 現在のRoomSpaceを破棄して新しいRoomSpaceを作成する
            _roomSpaceProvider.CurrentRoomSpace.CurrentValue?.Dispose();
            var roomSpace = new RoomSpace();
            _roomSpaceRegister.RegisterRoomSpace(roomSpace);
        }

        /// <summary>
        /// Speakerを読み込みRoomSpaceに配置する
        /// すでにSpeakerが配置されている場合は、そのSpeakerを削除して新しいSpeakerを配置する
        /// </summary>
        public async UniTask LoadNewSpeakerAsync(ISpeakerSource speakerSource, CancellationToken ct)
        {
            // SpeakerをRoomSpaceに配置する
            // すでにSpeakerが配置されている場合は、そのSpeakerを削除して新しいSpeakerを配置する
            var roomSpace = await _roomSpaceProvider.CurrentRoomSpace
                .FirstAsync(x => x != null, ct);

            if (roomSpace == null) return;

            // 現在のSpeakerがRoomSpaceに配置されている場合は削除する
            var currentSpeaker = roomSpace.CurrentSpeaker.CurrentValue;
            if (currentSpeaker != null) roomSpace.RemoveSpeaker(currentSpeaker.Id);

            // Speakerをロードする
            var speaker = await _speakerProvider.LoadSpeakerAsync(speakerSource, ct);

            // 新しいSpeakerをRoomSpaceに配置する
            roomSpace.RegisterSpeaker(speaker);

            // カメラを現在のSpeakerの顔にフォーカスする
            _speakerCameraUseCase.FocusOnCurrentSpeakerFace();
        }

        public void ChangeBackgroundColor(Color color)
        {
            var roomSpace = CurrentRoomSpace;
            if (roomSpace == null) return;

            roomSpace.ChangeBackgroundColor(color);
        }
    }
}