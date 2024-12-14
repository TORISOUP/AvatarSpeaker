using System;
using System.Threading;
using AvatarSpeaker.Core;
using AvatarSpeaker.Core.Interfaces;
using AvatarSpeaker.Cushion.VRM;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using VContainer.Unity;

namespace AvatarSpeaker.Scripts.Views.ViewBinder
{
    public sealed class RoomSpaceViewBinder : IInitializable, IDisposable
    {
        private readonly SpeakerCameraView _speakerCameraViewPrefab;
        private readonly IRoomSpaceProvider _roomSpaceProvider;
        private readonly CancellationTokenSource _cts = new();
        private readonly UguiRoomSpaceBackgroundView _uguiRoomSpaceBackgroundViewPrefab;

        private RoomSpaceView _currentRoomSpaceView;

        public RoomSpaceViewBinder(SpeakerCameraView speakerCameraViewPrefab,
            IRoomSpaceProvider roomSpaceProvider,
            UguiRoomSpaceBackgroundView backgroundViewPrefab)
        {
            _speakerCameraViewPrefab = speakerCameraViewPrefab;
            _roomSpaceProvider = roomSpaceProvider;
            _uguiRoomSpaceBackgroundViewPrefab = backgroundViewPrefab;
        }

        public void Initialize()
        {
            // RoomSpaceが生成されたらRoomSpaceViewとSpeakerCameraViewを生成
            _roomSpaceProvider.CurrentRoomSpace
                .Where(r => r != null)
                .SubscribeAwait(async (r, ct) => await CreateRoomSpaceViews(r, ct))
                .RegisterTo(_cts.Token);

            // RoomSpaceのSpeakerが変更されたらSpeakerViewを更新
            // ただし生成はVrmSpeakerに限る
            _roomSpaceProvider.CurrentRoomSpace
                .Where(r => r != null)
                .SelectMany(r => r.CurrentSpeaker)
                .OfType<Speaker, VrmSpeaker>()
                .Subscribe(speaker =>
                {
                    var speakerViewObject = new GameObject("VrmSpeakerView");
                    var speakerView = speakerViewObject.AddComponent<VrmSpeakerView>();
                    speakerView.SetVrmSpeaker(speaker);
                    // ヒエラルキー上の位置を調整
                    speakerViewObject.transform.SetParent(_currentRoomSpaceView.Root.transform);
                })
                .RegisterTo(_cts.Token);
        }

        private async UniTask CreateRoomSpaceViews(RoomSpace roomSpace, CancellationToken ct)
        {
            if (roomSpace == null) return;

            // RoomSpaceViewを生成
            var roomSpaceView = RoomSpaceView.Create();

            // SpeakerCameraViewを生成
            var speakerCameraView = UnityEngine.Object.Instantiate(_speakerCameraViewPrefab);
            speakerCameraView.Initialize(roomSpace.SpeakerCamera);

            // UguiRoomSpaceBackgroundViewを生成
            var backgroundView = UnityEngine.Object.Instantiate(_uguiRoomSpaceBackgroundViewPrefab);
            backgroundView.Initalize(roomSpace, speakerCameraView.Camera);

            // RoomSpaceViewに登録
            roomSpaceView.Initalize(roomSpace, backgroundView, speakerCameraView);

            // 現在のRoomSpaceViewを登録
            _currentRoomSpaceView = roomSpaceView;

            // RoomSpaceがDisposeされたらRoomSpaceViewを破棄
            await roomSpace.OnDisposeAsync.AttachExternalCancellation(ct);

            // 現在のRoomSpaceViewの参照を破棄
            _currentRoomSpaceView = null;

            // RoomSpaceViewを破棄
            UnityEngine.Object.Destroy(roomSpaceView.Root);
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}