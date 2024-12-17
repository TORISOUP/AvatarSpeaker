using System;
using System.Threading;
using AvatarSpeaker.Core;
using AvatarSpeaker.Core.Configurations;
using AvatarSpeaker.Core.Interfaces;
using AvatarSpeaker.Cushion.VRM;
using AvatarSpeaker.Views.RoomSpaces;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace AvatarSpeaker.Views.ViewBinder
{
    /// <summary>
    /// RoomSpace内で生成されたオブジェクトをViewにバインドする
    /// </summary>
    public sealed class RoomSpaceViewBinder : IInitializable, IDisposable
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly CancellationTokenSource _cts = new();
        private readonly IRoomSpaceProvider _roomSpaceProvider;
        private readonly RuntimeAnimatorController _speakerAnimatorController;
        private readonly SpeakerCameraView _speakerCameraViewPrefab;
        private readonly SubtitleView _subtitleViewPrefab;
        private readonly UguiRoomSpaceBackgroundView _uguiRoomSpaceBackgroundViewPrefab;

        private RoomSpaceView _currentRoomSpaceView;
        private SubtitleView _subtitleView;

        public RoomSpaceViewBinder(SpeakerCameraView speakerCameraViewPrefab,
            IRoomSpaceProvider roomSpaceProvider,
            UguiRoomSpaceBackgroundView uguiRoomSpaceBackgroundViewPrefab,
            RuntimeAnimatorController speakerAnimatorController,
            SubtitleView subtitleViewPrefab,
            IConfigurationRepository configurationRepository)
        {
            _speakerCameraViewPrefab = speakerCameraViewPrefab;
            _roomSpaceProvider = roomSpaceProvider;
            _uguiRoomSpaceBackgroundViewPrefab = uguiRoomSpaceBackgroundViewPrefab;
            _speakerAnimatorController = speakerAnimatorController;
            _subtitleViewPrefab = subtitleViewPrefab;
            _configurationRepository = configurationRepository;
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }

        public void Initialize()
        {
            // SubtitleViewは常に1つ存在していてよいのでここで作る
            _subtitleView = Object.Instantiate(_subtitleViewPrefab);

            // RoomSpaceが生成されたらRoomSpaceViewとSpeakerCameraViewを生成
            _roomSpaceProvider.CurrentRoomSpace
                .Where(r => r != null)
                .SubscribeAwait(async (r, ct) => await CreateRoomSpaceViews(r, ct))
                .RegisterTo(_cts.Token);

            // RoomSpaceのSpeakerが変更されたらSpeakerViewを更新
            // ただしここで生成できるものはVrmSpeakerに限る
            _roomSpaceProvider.CurrentRoomSpace
                .Where(r => r != null)
                .SelectMany(r => r.CurrentSpeaker)
                .OfType<Speaker, VrmSpeaker>()
                .Subscribe(speaker =>
                {
                    // Speakerが更新されたとき
                    
                    // SpeakerViewを生成
                    // 実体はInfrastructures.VoicevoxSpeakersで生成されたGameObjectなので、
                    // SpeakerViewはそれをネストして保持するだけの空オブジェクト
                    var speakerViewObject = new GameObject("VrmSpeakerView");
                    var speakerView = speakerViewObject.AddComponent<VrmSpeakerView>();
                    speakerView.SetVrmSpeaker(speaker, _speakerAnimatorController);

                    // ヒエラルキー上の位置を調整
                    speakerViewObject.transform.SetParent(_currentRoomSpaceView.Root.transform);

                    // SubtitleViewにSpeakerをセット
                    _subtitleView.SetUp(speaker, _configurationRepository.IsSubtitleEnabled);
                })
                .RegisterTo(_cts.Token);
        }

        /// <summary>
        /// RoomSpaceが生成されたときの処理
        /// </summary>
        private async UniTask CreateRoomSpaceViews(RoomSpace roomSpace, CancellationToken ct)
        {
            if (roomSpace == null) return;

            // RoomSpaceViewを生成
            var roomSpaceView = RoomSpaceView.Create();

            // SpeakerCameraViewを生成
            var speakerCameraView = Object.Instantiate(_speakerCameraViewPrefab);
            speakerCameraView.Initialize(roomSpace.SpeakerCamera);

            // UguiRoomSpaceBackgroundViewを生成
            // 背景の実装はいくつか考えられるが、今回は「uGUI実装版」を使うことにする
            var backgroundView = Object.Instantiate(_uguiRoomSpaceBackgroundViewPrefab);
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
            // 同時にRoomSpaceViewに紐づいているSpeakerCameraViewなどもまとめて破棄される
            Object.Destroy(roomSpaceView.Root);
        }
    }
}