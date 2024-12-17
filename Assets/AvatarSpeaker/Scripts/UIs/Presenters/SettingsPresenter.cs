using AvatarSpeaker.UseCases;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using R3;

namespace AvatarSpeaker.UIs.Presenters
{
    public class SettingsPresenter : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _voicevoxInputField;
        [SerializeField] private Button _voicevoxSetButton;
        [SerializeField] private Button _voicevoxTestButton;
        [SerializeField] private TMP_Text _voicevoxTestResultText;

        [SerializeField] private TMP_InputField _restApiInputField;
        [SerializeField] private Toggle _restApiToggle;

        [SerializeField] private Toggle _subtitlesToggle;

        private ConfigurationUseCase _configurationUseCase;
        private VoiceControlUseCase _voiceControlUseCase;
        private SpeakerUseCase _speakerUseCase;

        [Inject]
        public void Inject(ConfigurationUseCase configurationUseCase,
            VoiceControlUseCase voiceControlUseCase,
            SpeakerUseCase speakerUseCase)
        {
            _configurationUseCase = configurationUseCase;
            _voiceControlUseCase = voiceControlUseCase;
            _speakerUseCase = speakerUseCase;
            SetUp();
        }

        private void SetUp()
        {
            // --VOICEVOX--

            _voicevoxInputField.text = _configurationUseCase.VoiceControlConnectionSettings.CurrentValue.Address;
            _voicevoxSetButton.interactable = false;
            _voicevoxTestButton.interactable = true;

            // UseCase -> View
            _configurationUseCase.HttpServerSettings
                .Subscribe(x => _restApiInputField.interactable = !x.IsEnabled)
                .AddTo(this);

            // View -> UseCase
            _voicevoxSetButton.OnClickAsAsyncEnumerable(destroyCancellationToken)
                .Subscribe(_ =>
                {
                    _configurationUseCase.SetVoiceControlConnectionSettings(_voicevoxInputField.text);
                    _voicevoxTestResultText.text = "";
                    _voicevoxSetButton.interactable = false;
                    // SetButtonを押したらTestButtonを押せるようにする
                    _voicevoxTestButton.interactable = true;
                });

            // 接続先を変更したらTestボタンはSetするまで押せない
            _voicevoxInputField.OnValueChangedAsAsyncEnumerable(destroyCancellationToken)
                .Subscribe(_ =>
                {
                    _voicevoxTestResultText.text = "";
                    _voicevoxSetButton.interactable = true;
                    _voicevoxTestButton.interactable = false;
                });

            _voicevoxTestButton.OnClickAsAsyncEnumerable(destroyCancellationToken)
                .SubscribeAwait(async (_, ct) =>
                {
                    try
                    {
                        _voicevoxInputField.interactable = false;
                        _voicevoxSetButton.interactable = false;
                        _voicevoxTestButton.interactable = false;

                        _voicevoxTestResultText.text = $"Connecting:{_voicevoxInputField.text}";
                        var isReady = await _voiceControlUseCase.IsReadyAsync(ct);
                        _voicevoxTestResultText.text =
                            isReady ? $"{_voicevoxInputField.text} = OK" : $"{_voicevoxInputField.text} = NG";
                    }
                    finally
                    {
                        destroyCancellationToken.ThrowIfCancellationRequested();
                        _voicevoxInputField.interactable = true;
                        _voicevoxSetButton.interactable = false;
                        _voicevoxTestButton.interactable = true;
                    }
                }, destroyCancellationToken);

            // --REST API--
            _restApiInputField.text = _configurationUseCase.HttpServerSettings.CurrentValue.Port.ToString();
            _restApiToggle.isOn = _configurationUseCase.HttpServerSettings.CurrentValue.IsEnabled;
            _restApiToggle
                .OnValueChangedAsAsyncEnumerable(destroyCancellationToken)
                .Subscribe(_ =>
                {
                    _configurationUseCase.SetHttpServerSettings(
                        int.Parse(_restApiInputField.text),
                        _restApiToggle.isOn
                    );
                });

            // --SUBTITLES--
            _subtitlesToggle.isOn = _configurationUseCase.IsSubtitleEnabled.CurrentValue;
            _subtitlesToggle
                .OnValueChangedAsAsyncEnumerable(destroyCancellationToken)
                .Subscribe(_ => _configurationUseCase.SetIsSubtitleEnabled(_subtitlesToggle.isOn));
        }
    }
}