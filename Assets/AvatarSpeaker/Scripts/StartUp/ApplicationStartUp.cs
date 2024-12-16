using AvatarSpeaker.Http.Server;
using AvatarSpeaker.UseCases;
using UnityEngine;
using VContainer.Unity;

namespace AvatarSpeaker.StartUp
{
    public sealed class ApplicationStartUp : IStartable
    {
        private readonly RoomSpaceUseCase _roomSpaceUseCase;
        private readonly HttpServerRunner _httpServerRunner;

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