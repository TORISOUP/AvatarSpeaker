using AvatarSpeaker.Core;

namespace AvatarSpeaker.UseCases
{
    /// <summary>
    /// カメラに関する操作を提供するUseCase
    /// </summary>
    public sealed class SpeakerCameraUseCase
    {
        private readonly IRoomSpaceProvider _roomSpaceProvider;

        public SpeakerCameraUseCase(IRoomSpaceProvider roomSpaceProvider)
        {
            _roomSpaceProvider = roomSpaceProvider;
        }


    }
}