using System.Threading;
using AvatarSpeaker.Core;
using Cysharp.Threading.Tasks;

namespace AvatarSpeaker.UseCases
{
    /// <summary>
    /// RoomSpaceに関する操作を提供するUseCase
    /// </summary>
    public sealed class RoomSpaceUseCase
    {
        private readonly IRoomSpaceProvider _roomSpaceProvider;
        private readonly ISpeakerSourceProvider _speakerSourceProvider;
        private readonly ISpeakerProvider _speakerProvider;


        public RoomSpaceUseCase(IRoomSpaceProvider roomSpaceProvider,
            ISpeakerSourceProvider speakerSourceProvider,
            ISpeakerProvider speakerProvider)
        {
            _roomSpaceProvider = roomSpaceProvider;
            _speakerSourceProvider = speakerSourceProvider;
            _speakerProvider = speakerProvider;
        }

        /// <summary>
        /// Speakerを読み込みRoomSpaceに配置する
        /// すでにSpeakerが配置されている場合は、そのSpeakerを削除して新しいSpeakerを配置する
        /// </summary>
        public async UniTask LoadNewSpeakerAsync(ISpeakerSource speakerSource, CancellationToken ct)
        {
            // SpeakerをRoomSpaceに配置する
            // すでにSpeakerが配置されている場合は、そのSpeakerを削除して新しいSpeakerを配置する
            var roomSpace = _roomSpaceProvider.CurrentRoomSpace;

            // 現在のSpeakerがRoomSpaceに配置されている場合は削除する
            var currentSpeaker = roomSpace.CurrentSpeaker.CurrentValue;
            if (currentSpeaker != null)
            {
                roomSpace.RemoveSpeaker(currentSpeaker.Id);
            }
            
            // Speakerをロードする
            var speaker = await _speakerProvider.LoadSpeakerAsync(speakerSource, ct);
            
            // 新しいSpeakerをRoomSpaceに配置する
            roomSpace.RegisterSpeaker(speaker);
        }
    }
}