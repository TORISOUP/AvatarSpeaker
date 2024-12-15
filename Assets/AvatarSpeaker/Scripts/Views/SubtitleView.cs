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

        public void SetCurrentSpeaker(Speaker speaker)
        {
            _disposable?.Dispose();
            _disposable = speaker
                .CurrentSpeakingText
                .Subscribe(text => _text.text = text);
        }
        
        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}