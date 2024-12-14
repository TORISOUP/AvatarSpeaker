using AvatarSpeaker.UseCases;
using UnityEngine;
using VContainer.Unity;

namespace AvatarSpeaker.StartUp
{
    public sealed class ApplicationStartUp : IStartable
    {
        private readonly RoomSpaceUseCase _roomSpaceUseCase;

        public ApplicationStartUp(RoomSpaceUseCase roomSpaceUseCase)
        {
            _roomSpaceUseCase = roomSpaceUseCase;
        }

        public void Start()
        {
            _roomSpaceUseCase.CreateNewRoomSpace();
        }
    }
}