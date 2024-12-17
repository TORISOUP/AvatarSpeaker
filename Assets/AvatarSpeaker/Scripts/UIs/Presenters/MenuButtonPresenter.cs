using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace AvatarSpeaker.UIs.Presenters
{
    public sealed class MenuButtonPresenter : MonoBehaviour
    {
        [SerializeField] private Button _avatarButton;
        [SerializeField] private Button _roomSpaceButton;
        [SerializeField] private Button _cameraButton;
        [SerializeField] private Button _settingsButton;
        private UiController _uiController;

        private void Start()
        {
            _cameraButton.OnClickAsAsyncEnumerable(destroyCancellationToken)
                .Subscribe(_ => _uiController.OpenCameraSubCanvas());

            _roomSpaceButton.OnClickAsAsyncEnumerable(destroyCancellationToken)
                .Subscribe(_ => _uiController.OpenRoomSpaceSubCanvas());

            _avatarButton.OnClickAsAsyncEnumerable(destroyCancellationToken)
                .Subscribe(_ => _uiController.OpenAvatarSubCanvas());

            _settingsButton.OnClickAsAsyncEnumerable(destroyCancellationToken)
                .Subscribe(_ => _uiController.OpenSettingsSubCanvas());
        }

        [Inject]
        public void Inject(UiController uiController)
        {
            _uiController = uiController;
        }
    }
}