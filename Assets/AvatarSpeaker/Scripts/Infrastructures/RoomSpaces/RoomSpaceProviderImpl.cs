#nullable enable
using AvatarSpeaker.Core;
using AvatarSpeaker.Core.Interfaces;
using R3;
using UnityEngine;

namespace AvatarSpeaker.Infrastructures.RoomSpaces
{
    public sealed class RoomSpaceProviderImpl : IRoomSpaceProvider, IRoomSpaceRegister
    {
        public ReadOnlyReactiveProperty<RoomSpace?> CurrentRoomSpace => _currentRoomSpace;
        private readonly ReactiveProperty<RoomSpace?> _currentRoomSpace = new(null);

        public void RegisterRoomSpace(RoomSpace roomSpace)
        {
            _currentRoomSpace.Value = roomSpace;
        }


        public void Dispose()
        {
            _currentRoomSpace.Dispose();
        }
    }
}