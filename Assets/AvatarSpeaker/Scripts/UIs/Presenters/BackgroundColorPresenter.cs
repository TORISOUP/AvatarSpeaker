using AvatarSpeaker.UIs.Components;
using AvatarSpeaker.UseCases;
using UnityEngine;
using R3;
using VContainer;

namespace AvatarSpeaker.UIs.Presenters
{
    public sealed class BackgroundColorPresenter : MonoBehaviour
    {
        [SerializeField] private ColorSliders _colorSliders;

        private RoomSpaceUseCase _roomSpaceUseCase;
        
        [Inject]
        private void Initialize(RoomSpaceUseCase roomSpaceUseCase)
        {
            _roomSpaceUseCase = roomSpaceUseCase;
        }
        
        private void Start()
        {
            _colorSliders.Current.Value = _roomSpaceUseCase.CurrentRoomSpace?.BackgroundColor.CurrentValue ?? Color.green;
            _colorSliders.Current.Subscribe(c =>
            {
                _roomSpaceUseCase.ChangeBackgroundColor(c);
            }).AddTo(this);
        }
    }
}