using AvatarSpeaker.Core;

namespace AvatarSpeaker.Infrastructures.RoomSpaces
{
    /// <summary>
    /// 単一のRoomSpaceのみが存在するとして扱うProvider
    /// </summary>
    public sealed class SingletonRoomSpaceProvider : IRoomSpaceProvider
    {
        public RoomSpace CurrentRoomSpace { get; } = new();

        public void Dispose()
        {
            CurrentRoomSpace?.Dispose();
        }
    }
}