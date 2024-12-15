using System;
using System.Linq;
using AvatarSpeaker.Core.Models;
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
    /// Poseの切り替えを行うPresenter
    /// </summary>
    public sealed class PosePresenter : MonoBehaviour
    {
        private SpeakerUseCase _speakerUseCase;

        [SerializeField] private Dropdown _dropdown;

        [Inject]
        public void Inject(SpeakerUseCase speakerUseCase)
        {
            _speakerUseCase = speakerUseCase;
            SetUp();
        }

        private void SetUp()
        {
            _dropdown.options.Clear();

            var poses = Enum.GetValues(typeof(IdlePose)).Cast<IdlePose>().ToArray();
            foreach (var pose in poses)
            {
                _dropdown.options.Add(new Dropdown.OptionData(pose.ToString()));
            }

            // View -> UseCase
            _dropdown.OnValueChangedAsAsyncEnumerable(destroyCancellationToken)
                .Subscribe(v =>
                {
                    var pose = (IdlePose)v; // enumの定義 = indexという前提を使う
                    _speakerUseCase.ChangeIdlePoseToCurrentSpeaker(pose);
                });
            
            // UseCase -> View
            _speakerUseCase.OnSpeakerChanged
                .Subscribe(_ =>
                {
                    var speaker = _speakerUseCase.GetCurrentSpeaker();
                    if(speaker != null)
                    {
                        _dropdown.value = (int)speaker.CurrentIdlePose.CurrentValue;
                    }
                })
                .AddTo(this);
        }
    }
}