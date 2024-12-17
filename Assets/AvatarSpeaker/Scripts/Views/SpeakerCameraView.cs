using AvatarSpeaker.Core;
using R3;
using UnityEngine;

namespace AvatarSpeaker.Views
{
    public sealed class SpeakerCameraView : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        private SpeakerCamera _speakerCamera;

        public Camera Camera => _camera;


        public void Initialize(SpeakerCamera speakerCamera)
        {
            _speakerCamera = speakerCamera;
            _speakerCamera.OnDispose = () => { Destroy(gameObject); };

            // SpeakerCameraの位置と姿勢を同期
            _speakerCamera.Position.Subscribe(position => { transform.position = position; }).AddTo(this);

            _speakerCamera.Rotation.Subscribe(rotation => { transform.rotation = rotation; }).AddTo(this);
        }
    }
}