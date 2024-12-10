using System;
using UnityEngine;

namespace AvatarSpeaker.UIs
{
    public sealed class UiController : MonoBehaviour
    {
        [SerializeField] private Canvas _menuCanvas;

        /// <summary>
        /// Menuを無効化する
        /// </summary>
        public DisableMenuScope DisableMenuCanvasScope()
        {
            return new DisableMenuScope(_menuCanvas);
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
                if (_canvas != null)
                {
                    _canvas.enabled = true;
                }
            }
        }
    }
}