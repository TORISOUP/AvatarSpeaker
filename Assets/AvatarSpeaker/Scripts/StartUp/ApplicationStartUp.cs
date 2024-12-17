using AvatarSpeaker.Http.Server;
using AvatarSpeaker.UseCases;
using VContainer.Unity;

namespace AvatarSpeaker.StartUp
{
    /// <summary>
    /// アプリケーションの動作の起点
    /// </summary>
    public sealed class ApplicationStartUp : IStartable
    {
        private readonly HttpServerRunner _httpServerRunner;
        private readonly RoomSpaceUseCase _roomSpaceUseCase;

        public ApplicationStartUp(RoomSpaceUseCase roomSpaceUseCase, HttpServerRunner httpServerRunner)
        {
            _roomSpaceUseCase = roomSpaceUseCase;
            _httpServerRunner = httpServerRunner;
        }

        public void Start()
        {
            // HTTPサーバーを起動する
            _httpServerRunner.Start();

            // 新しいRoomSpaceを作成する
            _roomSpaceUseCase.CreateNewRoomSpace();
        }
    }
}