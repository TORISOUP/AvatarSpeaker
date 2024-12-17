using System;
using R3;
using UnityEngine;

namespace AvatarSpeaker.Core
{
    /// <summary>
    /// RoomSpace内でSpeakerを表示するカメラ
    /// </summary>
    public sealed class SpeakerCamera : IDisposable
    {
        /// <summary>
        /// 位置
        /// </summary>
        public ReactiveProperty<Vector3> Position { get; } = new(Vector3.zero);

        /// <summary>
        /// 姿勢
        /// </summary>
        public ReactiveProperty<Quaternion> Rotation { get; } = new(Quaternion.identity);


        public Action OnDispose { get; set; }


        public void Dispose()
        {
            OnDispose?.Invoke();
        }

        /// <summary>
        /// 指定した位置を向く
        /// </summary>
        public void LookAt(Vector3 target)
        {
            var direction = target - Position.Value;
            Rotation.Value = Quaternion.LookRotation(direction);
        }
    }
}