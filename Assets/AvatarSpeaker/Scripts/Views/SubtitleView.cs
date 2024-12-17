using System;
using AvatarSpeaker.Core;
using R3;
using TMPro;
using UnityEngine;

namespace AvatarSpeaker.Views
{
    /// <summary>
    /// 字幕を出すView
    /// </summary>
    public class SubtitleView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        private IDisposable _disposable;

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }

        public void SetUp(Speaker speaker, ReadOnlyReactiveProperty<bool> isSubtitleEnabled)
        {
            _disposable?.Dispose();
            _disposable = speaker
                .CurrentSpeakingText
                .CombineLatest(isSubtitleEnabled, (text, isEnabled) => (isEnabled, text))
                .Subscribe(tp =>
                {
                    var (isEnabled, text) = tp;
                    _text.gameObject.SetActive(isEnabled);
                    _text.text = text;
                });
        }
    }
}