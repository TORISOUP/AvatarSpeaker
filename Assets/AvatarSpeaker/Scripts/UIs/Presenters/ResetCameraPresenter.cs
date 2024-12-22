using AvatarSpeaker.UseCases;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace AvatarSpeaker.UIs.Presenters
{
    public class ResetCameraPresenter : MonoBehaviour
    {
        [SerializeField] private Button _resetCameraButton;

        private SpeakerCameraUseCase _speakerCameraUseCase;

        [Inject]
        public void Inject(SpeakerCameraUseCase speakerCameraUseCase)
        {
            _speakerCameraUseCase = speakerCameraUseCase;

            _resetCameraButton
                .OnClickAsAsyncEnumerable(destroyCancellationToken)
                .Subscribe(_ => _speakerCameraUseCase.FocusOnCurrentSpeakerFace());
        }
    }
}