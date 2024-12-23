using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

namespace AvatarSpeaker.UIs.Presenters
{
    public class MainCanvasPresenter : MonoBehaviour
    {
        [SerializeField] private Button _closeButton;
        private UiController _uiController;

        [Inject]
        public void Inject(UiController uiController)
        {
            _uiController = uiController;
            SetUp();
        }

        private void SetUp()
        {
            _closeButton.OnClickAsAsyncEnumerable(destroyCancellationToken)
                .Subscribe(_ => _uiController.CloseMainUI());

            // Mキーを押すとメインUIを開く
            this.GetAsyncUpdateTrigger()
                .Where(_ => Input.GetKeyDown(KeyCode.M) && EventSystem.current.currentSelectedGameObject == null)
                .Subscribe(_ =>
                {
                    if (_uiController.IsUiUsing.Value)
                        _uiController.CloseMainUI();
                    else
                        _uiController.OpenMainUI();
                }, destroyCancellationToken);

            // 最初は開いておく
            _uiController.OpenMainUI();
        }
    }
}