using AvatarSpeaker.UseCases;
using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace AvatarSpeaker.UIs
{
    public class KeyboardMouseInput : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 1.0f;
        [SerializeField] private float _rotateSpeed = 1.0f;
        [SerializeField] private float _zoomSpeed = 5.0f;

        private SpeakerCameraUseCase _speakerCameraUseCase;

        [Inject]
        public void Inject(SpeakerCameraUseCase speakerCameraUseCase)
        {
            _speakerCameraUseCase = speakerCameraUseCase;
            SetUp();
        }

        private void SetUp()
        {
            this.UpdateAsObservable()
                .Where(_ => EventSystem.current.currentSelectedGameObject == null)
                .Subscribe(_ =>
                {
                    Rotate();
                    Move();
                })
                .AddTo(this);
        }

        private void Move()
        {
            // W, A, S, D, Q, Eでカメラを移動

            var v = Vector3.zero;

            if (Input.GetKey(KeyCode.A)) v += Vector3.left;

            if (Input.GetKey(KeyCode.D)) v += Vector3.right;

            if (Input.GetKey(KeyCode.S)) v += Vector3.down;

            if (Input.GetKey(KeyCode.W)) v += Vector3.up;

            var mouseScroll = Input.mouseScrollDelta.y;
            v += Vector3.forward * mouseScroll * _zoomSpeed;

            _speakerCameraUseCase.MoveShift(v * _moveSpeed * Time.deltaTime);
        }

        private void Rotate()
        {
            // 右クリック中のみ
            if (!Input.GetMouseButton(1)) return;

            // マウスでカメラを回転
            var delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));


            if (Input.GetMouseButton(1)) _speakerCameraUseCase.RotateEuler(delta * _rotateSpeed);
        }
    }
}