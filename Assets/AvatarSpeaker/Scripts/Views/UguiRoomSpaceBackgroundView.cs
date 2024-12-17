using AvatarSpeaker.Core;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarSpeaker.Views
{
    /// <summary>
    /// BackgroundViewのuGUI実装
    /// </summary>
    public sealed class UguiRoomSpaceBackgroundView : MonoBehaviour, IBackgroundView
    {
        [SerializeField] private Image _backGroundImage;
        [SerializeField] private Canvas _canvas;

        public GameObject Root => gameObject;

        public void Dispose()
        {
            if (gameObject != null) Destroy(gameObject);
        }

        public void Initalize(RoomSpace roomSpace, Camera worldCamera)
        {
            _canvas.worldCamera = worldCamera;
            roomSpace
                .BackgroundColor.Subscribe(ChangeBackgroundColor)
                .AddTo(this);
        }

        private void ChangeBackgroundColor(Color color)
        {
            _backGroundImage.color = color;
        }
    }
}