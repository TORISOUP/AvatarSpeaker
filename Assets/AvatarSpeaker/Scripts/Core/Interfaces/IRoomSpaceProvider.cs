using System;

namespace AvatarSpeaker.Core
{
    /// <summary>
    /// 現在の「空間の状態」を提供する
    /// </summary>
    public interface IRoomSpaceProvider : IDisposable
    {
        RoomSpace CurrentRoomSpace { get; }
    }
}