using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AvatarSpeaker.UIs
{
    public sealed class UiController : MonoBehaviour
    {
        [SerializeField] private Canvas _mainCanvas;
        [SerializeField] private Canvas _speakerSubCanvas;
        [SerializeField] private Canvas _roomSpaceSubCanvas;
        [SerializeField] private Canvas _cameraSubCanvas;
        [SerializeField] private Canvas _settingsSubCanvas;

        /// <summary>
        /// UIを使用中かどうか
        /// </summary>
        public AsyncReactiveProperty<bool> IsUiUsing { get; } = new(false);

        private void Start()
        {
            _speakerSubCanvas.gameObject.SetActive(true);
            _roomSpaceSubCanvas.gameObject.SetActive(true);
            _cameraSubCanvas.gameObject.SetActive(true);
            _settingsSubCanvas.gameObject.SetActive(true);
        }

        public void OpenAvatarSubCanvas()
        {
            _speakerSubCanvas.enabled = true;
            _roomSpaceSubCanvas.enabled = false;
            _cameraSubCanvas.enabled = false;
            _settingsSubCanvas.enabled = false;
        }

        public void OpenRoomSpaceSubCanvas()
        {
            _speakerSubCanvas.enabled = false;
            _roomSpaceSubCanvas.enabled = true;
            _cameraSubCanvas.enabled = false;
            _settingsSubCanvas.enabled = false;
        }

        public void OpenCameraSubCanvas()
        {
            _speakerSubCanvas.enabled = false;
            _roomSpaceSubCanvas.enabled = false;
            _cameraSubCanvas.enabled = true;
            _settingsSubCanvas.enabled = false;
        }

        public void OpenSettingsSubCanvas()
        {
            _speakerSubCanvas.enabled = false;
            _roomSpaceSubCanvas.enabled = false;
            _cameraSubCanvas.enabled = false;
            _settingsSubCanvas.enabled = true;
        }

        public void OpenMainUI()
        {
            _mainCanvas.enabled = true;
            _cameraSubCanvas.enabled = false;
            _roomSpaceSubCanvas.enabled = false;
            _settingsSubCanvas.enabled = false;
            _speakerSubCanvas.enabled = true;
            IsUiUsing.Value = true;
        }

        public void CloseMainUI()
        {
            _mainCanvas.enabled = false;
            _cameraSubCanvas.enabled = false;
            _roomSpaceSubCanvas.enabled = false;
            _speakerSubCanvas.enabled = false;
            _settingsSubCanvas.enabled = false;
            IsUiUsing.Value = false;
        }


        /// <summary>
        /// UIを一時的に無効化する
        /// 一時無効化中はIsUiUsingはtrueのまま
        /// </summary>
        public DisableMenuScope DisableTemporaryMainUiCanvasScope()
        {
            return new DisableMenuScope(_mainCanvas);
        }

        /// <summary>
        /// メニューの無効化をDisposableで管理する
        /// </summary>
        public readonly struct DisableMenuScope : IDisposable
        {
            private readonly Canvas _canvas;

            public DisableMenuScope(Canvas canvas)
            {
                _canvas = canvas;
                _canvas.enabled = false;
            }

            public void Dispose()
            {
                if (_canvas != null) _canvas.enabled = true;
            }
        }
    }
}