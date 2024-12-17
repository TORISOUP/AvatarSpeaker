using AvatarSpeaker.Http.Server;
using AvatarSpeaker.UseCases;
using VContainer.Unity;

namespace AvatarSpeaker.StartUp
{
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
            _httpServerRunner.Start();
            _roomSpaceUseCase.CreateNewRoomSpace();
        }
    }
}