using System.Collections.Generic;
using System.Threading;
using AvatarSpeaker.Core;
using AvatarSpeaker.UseCases;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace AvatarSpeaker.UIs.Presenters
{
    /// <summary>
    /// 発話依頼を行うUIのPresenter
    /// </summary>
    public class SpeakPresenter : MonoBehaviour
    {
        [SerializeField] private GameObject _speakArea;

        [SerializeField] private InputField _textInputField;

        [SerializeField] private Dropdown _styleDropdown;

        [SerializeField] private Button _speakButton;

        private SpeakerUseCase _speakerUseCase;
        private readonly Dictionary<Dropdown.OptionData, SpeakStyle> _styleMap = new();

        [Inject]
        public void Inject(SpeakerUseCase speakerUseCase)
        {
            _speakerUseCase = speakerUseCase;

            // Speakerが変更されたときにセットアップする
            _speakerUseCase.OnSpeakerChanged
                .SubscribeAwait(async (_, ct) => await SetUpAsync(ct), AwaitOperation.Switch)
                .RegisterTo(destroyCancellationToken);

            _speakButton.OnClickAsAsyncEnumerable(destroyCancellationToken)
                .Where(_ => _textInputField.text.Length > 0)
                .SubscribeAwait(async (_, ct) =>
                {
                    try
                    {
                        _speakButton.interactable = false;
                        var text = _textInputField.text;
                        var style = _styleMap[_styleDropdown.options[_styleDropdown.value]];
                        await _speakerUseCase.SpeakByCurrentSpeakerAsync(text, style, ct);
                        _textInputField.text = "";
                    }
                    finally
                    {
                        _speakButton.interactable = true;
                    }
                });
        }

        private void Awake()
        {
            _speakArea.SetActive(false);
        }

        private async UniTask SetUpAsync(CancellationToken ct)
        {
            _speakArea.SetActive(false);

            _styleDropdown.options.Clear();
            _styleMap.Clear();

            // VOICEVOX未起動時は失敗する可能性あり
            var styles = await _speakerUseCase.GetSpeechStylesAsync(ct);

            foreach (var style in styles)
            {
                var opt = new Dropdown.OptionData(style.DisplayName);
                _styleDropdown.options.Add(opt);
                _styleMap.Add(opt, style);
            }

            _speakArea.SetActive(true);
        }
    }
}