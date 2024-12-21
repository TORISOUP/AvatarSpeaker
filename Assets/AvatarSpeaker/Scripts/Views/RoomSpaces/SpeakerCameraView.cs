using AvatarSpeaker.Core;
using R3;
using UnityEngine;

namespace AvatarSpeaker.Views.RoomSpaces
{
    /// <summary>
    /// SpeakerCameraの表示を管理するView
    /// </summary>
    public sealed class SpeakerCameraView : MonoBehaviour
    {
        // カメラコンポーネント
        [SerializeField] private Camera _camera;

        private SpeakerCamera _speakerCamera;

        public Camera Camera => _camera;

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize(SpeakerCamera speakerCamera)
        {
            _speakerCamera = speakerCamera;
            _speakerCamera.OnDispose = () => { Destroy(gameObject); };


            // SpeakerCameraの位置と姿勢を実際のカメラに反映する
            _speakerCamera.Position
                .Subscribe(position => transform.position = position).AddTo(this);
            
            _speakerCamera.Rotation
                .Subscribe(rotation => transform.rotation = rotation).AddTo(this);
        }
    }
}