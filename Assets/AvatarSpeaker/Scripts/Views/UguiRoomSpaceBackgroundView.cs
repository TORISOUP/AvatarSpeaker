using UnityEngine;
using UnityEngine.UI;

namespace AvatarSpeaker.Scripts.Views
{
    /// <summary>
    /// BackgroundViewのuGUI実装
    /// </summary>
    public sealed class UguiRoomSpaceBackgroundView : MonoBehaviour, IBackgroundView
    {
        [SerializeField] private Image _backGroundImage;
        [SerializeField] private Canvas _canvas;
        
        public void SetWorldCamera(Camera camera)
        {
            _canvas.worldCamera = camera;
        }

        public GameObject Root => gameObject;

        public void ChangeBackgroundColor(Color color)
        {
            _backGroundImage.color = color;
        }

        public void Dispose()
        {
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }
    }
}