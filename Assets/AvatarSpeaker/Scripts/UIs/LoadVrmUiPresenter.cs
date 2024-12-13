using System.Linq;
using System.Threading;
using AvatarSpeaker.UseCases;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace AvatarSpeaker.UIs
{
    public class LoadVrmUiPresenter : MonoBehaviour
    {
        private Button _button;
        private RoomSpaceUseCase _roomSpaceUseCase;
        private SpeakerUseCase _speakerUseCase;
        private UiController _uiController;

        [Inject]
        private void Initialize(UiController uiController,
            RoomSpaceUseCase roomSpaceUseCase,
            SpeakerUseCase speakerUseCase)
        {
            _uiController = uiController;
            _roomSpaceUseCase = roomSpaceUseCase;
            _speakerUseCase = speakerUseCase;
        }

        private void Start()
        {
            _button = GetComponent<Button>();

            _button.OnClickAsAsyncEnumerable(destroyCancellationToken)
                .SubscribeAwait(async (_, ct) => await SelectVrmAsync(ct))
                .AddTo(destroyCancellationToken);
        }

        private async UniTask SelectVrmAsync(CancellationToken ct)
        {
            // メニューを無効化してSpeakerをロードする
            using (_uiController.DisableTemporaryMainUiCanvasScope())
            {
                // 現在有効なSpeakerSourceを取得する
                var speakerSources = await _speakerUseCase.GetAvailableSpeakerSourcesAsync(ct);
                if(speakerSources.Length == 0)
                {
                    // 空なら何もしない
                    return;
                }

                // MEMO: 本来はここでVRMを選択するUIを表示する
                //       今回は単一のSpeakerSourceしか返ってこない前提で進める
                var speakerSource = speakerSources.First();

                // SpeakerをロードしてRoomSpaceに配置する
                await _roomSpaceUseCase.LoadNewSpeakerAsync(speakerSource, ct);
            }
        }
    }
}