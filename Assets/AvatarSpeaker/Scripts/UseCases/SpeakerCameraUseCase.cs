using AvatarSpeaker.Core;
using AvatarSpeaker.Core.Interfaces;

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

        /// <summary>
        /// RoomSpace内の現在のSpeakerの顔をカメラにフォーカスする
        /// </summary>
        public void FocusOnCurrentSpeakerFace()
        {
            var roomSpace = _roomSpaceProvider.CurrentRoomSpace.CurrentValue;
            if (roomSpace == null) return;

            var currentSpeaker = roomSpace.CurrentSpeaker.CurrentValue;
            if (currentSpeaker == null)
            {
                return;
            }

            var camera = roomSpace.SpeakerCamera;
            
            // カメラの位置を設定
            // 顔の位置から前方に少し移動したところ
            camera.Position.Value = currentSpeaker.FacePosition + currentSpeaker.BodyForward * 0.5f;
            
            // カメラの姿勢を設定
            camera.LookAt(currentSpeaker.FacePosition);
        }
    }
}