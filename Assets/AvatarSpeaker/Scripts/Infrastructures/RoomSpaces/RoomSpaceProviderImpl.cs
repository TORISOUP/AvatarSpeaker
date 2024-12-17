#nullable enable
using AvatarSpeaker.Core;
using AvatarSpeaker.Core.Interfaces;
using R3;

namespace AvatarSpeaker.Infrastructures.RoomSpaces
{
    public sealed class RoomSpaceProviderImpl : IRoomSpaceProvider, IRoomSpaceRegister
    {
        private readonly ReactiveProperty<RoomSpace?> _currentRoomSpace = new(null);
        public ReadOnlyReactiveProperty<RoomSpace?> CurrentRoomSpace => _currentRoomSpace;


        public void Dispose()
        {
            _currentRoomSpace.Dispose();
        }

        public void RegisterRoomSpace(RoomSpace roomSpace)
        {
            _currentRoomSpace.Value = roomSpace;
        }
    }
}