using System.Collections.Generic;
using System.Threading;
using AvatarSpeaker.Core;
using AvatarSpeaker.Core.Models;
using AvatarSpeaker.UseCases;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace AvatarSpeaker.UIs.Presenters
{
    /// <summary>
    /// Speak周りのPresenter
    /// </summary>
    public class SpeakPresenter : MonoBehaviour
    {
        [SerializeField] private GameObject _speakArea;
        [SerializeField] private GameObject _loading;

        [SerializeField] private TMP_InputField _textInputField;
        [SerializeField] private TMP_Dropdown _styleDropdown;
        [SerializeField] private TMP_InputField _speedInputField;
        [SerializeField] private TMP_InputField _pitchInputField;
        [SerializeField] private TMP_InputField _volumeInputField;

        [SerializeField] private Button _speakButton;
        [SerializeField] private Button _cancelButton;
        private readonly Dictionary<TMP_Dropdown.OptionData, SpeakStyle> _styleMap = new();

        private SpeakerUseCase _speakerUseCase;

        private void Awake()
        {
            _speakArea.SetActive(false);
            _loading.SetActive(false);
        }

        [Inject]
        public void Inject(SpeakerUseCase speakerUseCase)
        {
            _speakerUseCase = speakerUseCase;

            // Speakerが変更されたときにセットアップする
            _speakerUseCase.OnSpeakerChanged
                .SubscribeAwait(async (speaker, ct) =>
                {
                    // 引数で渡すctはSpeakerが変更されるときにキャンセルされる
                    await SetUpAsync(speaker, ct);
                }, AwaitOperation.Switch)
                .RegisterTo(destroyCancellationToken);

            _speakButton.OnClickAsAsyncEnumerable(destroyCancellationToken)
                .Where(_ => _textInputField.text.Length > 0)
                .SubscribeAwait(async (_, ct) =>
                {
                    try
                    {
                        _speakButton.interactable = false;
                        var text = _textInputField.text;
                        _textInputField.text = "";
                        await _speakerUseCase.SpeakByCurrentSpeakerAsync(text, ct);
                    }
                    finally
                    {
                        _speakButton.interactable = true;
                    }
                });

            _cancelButton.OnClickAsAsyncEnumerable(destroyCancellationToken)
                .Subscribe(_ =>
                {
                    _speakerUseCase.CancelSpeakingAll();
                });
        }

        /// <summary>
        /// Speakerに対して設定する
        /// </summary>
        private async UniTask SetUpAsync(Speaker speaker, CancellationToken ct)
        {
            _speakArea.SetActive(false);
            _loading.SetActive(true);

            _styleDropdown.onValueChanged.RemoveAllListeners();

            if (speaker == null) return;

            // View -> Speaker
            _speedInputField.OnEndEditAsAsyncEnumerable(ct)
                .Subscribe(x =>
                {
                    if (float.TryParse(x, out var v))
                    {
                        var current = speaker.CurrentSpeakParameter.CurrentValue;
                        v = Mathf.Clamp(v, 0.5f, 2.0f);
                        speaker.ChangeSpeakParameter(current.Clone(speedScale: v));
                        _speedInputField.text = v.ToString();
                    }
                });

            _pitchInputField.OnEndEditAsAsyncEnumerable(ct)
                .Subscribe(x =>
                {
                    if (float.TryParse(x, out var v))
                    {
                        var current = speaker.CurrentSpeakParameter.CurrentValue;
                        v = Mathf.Clamp(v, -0.15f, 0.15f);
                        speaker.ChangeSpeakParameter(current.Clone(pitchScale: v));
                        _pitchInputField.text = v.ToString();
                    }
                });

            _volumeInputField.OnEndEditAsAsyncEnumerable(ct)
                .Subscribe(x =>
                {
                    if (float.TryParse(x, out var v))
                    {
                        var current = speaker.CurrentSpeakParameter.CurrentValue;
                        v = Mathf.Clamp(v, 0f, 2f);
                        speaker.ChangeSpeakParameter(current.Clone(volumeScale: v));
                        _volumeInputField.text = v.ToString();
                    }
                });


            // Styleのセットアップ
            _styleDropdown.options.Clear();
            _styleMap.Clear();

            // VOICEVOX未起動時は失敗する可能性あり
            var styles = await _speakerUseCase.GetSpeakStylesAsync(ct);

            foreach (var style in styles)
            {
                var opt = new TMP_Dropdown.OptionData(style.DisplayName);
                _styleDropdown.options.Add(opt);
                _styleMap.Add(opt, style);
            }

            // TMP_DropdownのAsyncEnumerable実装が無いので手動で登録
            _styleDropdown.onValueChanged.AddListener(i =>
            {
                var style = _styleMap[_styleDropdown.options[i]];
                speaker.ChangeSpeakParameter(speaker.CurrentSpeakParameter.CurrentValue.Clone(style));
            });


            // Speaker -> View

            speaker.CurrentSpeakParameter.Subscribe(x =>
                {
                    _styleDropdown.value = _styleDropdown.options.FindIndex(y => _styleMap[y].Id.Equals(x.Style.Id));
                    _speedInputField.text = x.SpeedScale.ToString();
                    _pitchInputField.text = x.PitchScale.ToString();
                    _volumeInputField.text = x.VolumeScale.ToString();
                })
                .RegisterTo(ct);


            _speakArea.SetActive(true);
            _loading.SetActive(false);
        }
    }
}