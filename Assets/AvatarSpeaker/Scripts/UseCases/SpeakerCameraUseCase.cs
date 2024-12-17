#nullable enable
using AvatarSpeaker.Core;
using AvatarSpeaker.Core.Interfaces;
using UnityEngine;

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

        private SpeakerCamera? CurrentSpeakerCamera => _roomSpaceProvider.CurrentRoomSpace.CurrentValue?.SpeakerCamera;


        /// <summary>
        /// RoomSpace内の現在のSpeakerの顔をカメラにフォーカスする
        /// </summary>
        public void FocusOnCurrentSpeakerFace()
        {
            var roomSpace = _roomSpaceProvider.CurrentRoomSpace.CurrentValue;
            if (roomSpace == null) return;

            var currentSpeaker = roomSpace.CurrentSpeaker.CurrentValue;
            if (currentSpeaker == null) return;

            // RoomSpaceが存在するならCameraは必ず存在する
            var camera = CurrentSpeakerCamera!;

            // カメラの位置を設定
            // 顔の位置から前方に少し移動したところ
            camera.Position.Value = currentSpeaker.FacePosition + currentSpeaker.BodyForward * 0.5f;

            // カメラの姿勢を設定
            camera.LookAt(currentSpeaker.FacePosition);
        }

        /// <summary>
        /// 指定ベクトル分平行移動する
        /// </summary>
        public void MoveShift(Vector3 move)
        {
            var camera = CurrentSpeakerCamera;
            if (camera == null) return;

            camera.Position.Value += camera.Rotation.Value * move;
        }

        /// <summary>
        /// オイラー角で回転する
        /// </summary>
        public void RotateEuler(Vector3 euler)
        {
            var camera = CurrentSpeakerCamera;
            if (camera == null) return;

            var currentRotation = camera.Rotation.CurrentValue;
            var forward = currentRotation * Vector3.forward;
            var up = Vector3.up;
            var horizontalRotation = Quaternion.AngleAxis(euler.x, up); // 水平方向 (Y軸) の回転
            var verticalRotation = Quaternion.AngleAxis(euler.y, Vector3.right); // 垂直方向 (X軸) の回転

            var rotatedForward = horizontalRotation * forward;

            var finalForward = verticalRotation * rotatedForward;

            var newRotation = Quaternion.LookRotation(finalForward, up);

            camera.Rotation.Value = newRotation;
        }
    }
}