using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace AvatarSpeaker.UIs
{
    public class MainCanvasPresenter : MonoBehaviour
    {
        private UiController _uiController;

        [SerializeField] private Button _closeButton;

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

            _uiController.CloseMainUI();

            // Mキーを押すとメインUIを開く
            this.GetAsyncUpdateTrigger()
                .Where(_ => Input.GetKeyDown(KeyCode.M))
                .SubscribeAwait(async (_, ct) =>
                {
                    // UIを開く
                    _uiController.OpenMainUI();
                    // UIが閉じられるまで待機
                    await _uiController.IsUiUsing.FirstAsync(x => !x, ct);
                }, destroyCancellationToken);
        }
    }
}