using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VContainer;

namespace AvatarSpeaker.UIs
{
    public sealed class MenuButtonPresenter : MonoBehaviour
    {
        private UiController _uiController;

        [SerializeField] private Button _avatarButton;
        [SerializeField] private Button _roomSpaceButton;
        [SerializeField] private Button _cameraButton;

        [Inject]
        public void Inject(UiController uiController)
        {
            _uiController = uiController;
        }

        private void Start()
        {
            _cameraButton.OnClickAsAsyncEnumerable(destroyCancellationToken)
                .Subscribe(_ => _uiController.OpenCameraSubCanvas());

            _roomSpaceButton.OnClickAsAsyncEnumerable(destroyCancellationToken)
                .Subscribe(_ => _uiController.OpenRoomSpaceSubCanvas());

            _avatarButton.OnClickAsAsyncEnumerable(destroyCancellationToken)
                .Subscribe(_ => _uiController.OpenAvatarSubCanvas());
        }
    }
}