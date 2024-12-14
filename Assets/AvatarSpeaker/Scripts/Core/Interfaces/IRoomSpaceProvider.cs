#nullable enable
using System;
using R3;

namespace AvatarSpeaker.Core.Interfaces
{
    /// <summary>
    /// 現在の「空間の状態」を提供する
    /// </summary>
    public interface IRoomSpaceProvider : IDisposable
    {
        ReadOnlyReactiveProperty<RoomSpace?> CurrentRoomSpace { get; }
    }

    public interface IRoomSpaceRegister : IDisposable
    {
        void RegisterRoomSpace(RoomSpace roomSpace);
    }
}