using System;
using System.Threading;
using System.Threading.Tasks;
using AvatarSpeaker.Core.Models;
using AvatarSpeaker.Core.UnityAdapter.VRM;
using AvatarSpeaker.Infrastructures.Voicevoxes;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UniVRM10;
using VoicevoxClientSharp;
using VoicevoxClientSharp.Unity;
using Object = UnityEngine.Object;

namespace AvatarSpeaker.Infrastructures.VoicevoxSpeakers
{
    /// <summary>
    /// SpeakerのVOICEVOXとVRM実装
    /// </summary>
    public class VoicevoxSpeaker : VrmSpeaker
    {
        /// <summary>
        /// このVoicevoxSpeakerが存在する限り使い続けるCancellationTokenSource
        /// </summary>
        private readonly CancellationTokenSource _allCts = new();

        private readonly ReactiveProperty<string> _currentSpeakingText = new("");

        private readonly UniTaskCompletionSource _onDisposeUniTaskCompletionSource = new();

        private readonly Subject<(ValueTask<SynthesisResult>, AutoResetUniTaskCompletionSource, CancellationToken)>
            _speechRegisterSubject =
                new();

        private readonly VoicevoxProvider _voicevoxProvider;
        private readonly GameObject _vrmGameObject;

        /// <summary>
        /// 現在の読み上げのコンテキストにおいて使用されるCancellationTokenSource
        /// </summary>
        private CancellationTokenSource _currentSpeakingCts;

        public VoicevoxSpeaker(Vrm10Instance vrm10Instance, VoicevoxProvider provider)
        {
            GameObject = vrm10Instance.gameObject;
            Vrm10Instance = vrm10Instance;

            // SpeakerのIDを設定
            Id = $"voicevox_vrm_{vrm10Instance.gameObject.GetInstanceID().ToString()}";

            _voicevoxProvider = provider;
            _vrmGameObject = vrm10Instance.gameObject;

            // AudioSourceを追加
            var audioSource = _vrmGameObject.AddComponent<AudioSource>();
            // VRMをリップシンクするためのコンポーネントを追加
            var lipSync = _vrmGameObject.AddComponent<VoicevoxVrmLipSyncPlayer>();

            // 音声合成の再生を行うコンポーネントを追加
            var voicevoxSpeakPlayer = _vrmGameObject.AddComponent<VoicevoxSpeakPlayer>();

            // 紐づける
            voicevoxSpeakPlayer.AudioSource = audioSource;
            voicevoxSpeakPlayer.AddOptionalVoicevoxPlayer(lipSync);

            // 音声合成の再生依頼が流れてくるので、ここで非同期的に逐次処理する
            // VOICEVOXに事前に音声合成リクエストだけ投げておき、終わったら音声再生を実行する
            // 音声再生が終わったら次の音声合成リクエストが完了するのを待つ、を繰り返す
            _speechRegisterSubject
                .SubscribeAwait(async (values, ct) =>
                {
                    if (voicevoxSpeakPlayer == null) return;

                    var (task, autoResetUniTaskCompletionSource, ctsToken) = values;
                    try
                    {
                        // 音声合成のタスクが完了するまで待機
                        var result = await task;
                        ct.ThrowIfCancellationRequested();

                        // 現在の発話中のテキストを更新
                        _currentSpeakingText.Value = result.Text;

                        await voicevoxSpeakPlayer.PlayAsync(result, ctsToken);
                        autoResetUniTaskCompletionSource.TrySetResult();
                    }
                    catch (OperationCanceledException)
                    {
                        autoResetUniTaskCompletionSource.TrySetCanceled();
                    }
                    catch (Exception e)
                    {
                        autoResetUniTaskCompletionSource.TrySetException(e);
                    }
                    finally
                    {
                        // 発話中のテキストをクリア
                        _currentSpeakingText.Value = "";
                    }
                })
                .RegisterTo(_allCts.Token);
        }

        // GameObjectのIDをSpeakerのIDとして利用
        public sealed override string Id { get; }

        public override GameObject GameObject { get; }
        public override Vrm10Instance Vrm10Instance { get; }

        public override ReadOnlyReactiveProperty<string> CurrentSpeakingText => _currentSpeakingText;

        /// <summary>
        /// 目の位置をSpeakerの顔の位置として利用する
        /// </summary>
        public override Vector3 FacePosition => Vrm10Instance.Runtime.LookAt.LookAtOriginTransform.position;

        /// <summary>
        /// 腰の位置をSpeakerの前方向として利用する
        /// </summary>
        public override Vector3 BodyForward => Vrm10Instance.Runtime.LookAt.LookAtOriginTransform.forward;

        public override UniTask OnDisposeAsync => _onDisposeUniTaskCompletionSource.Task;


        /// <summary>
        /// 発話する
        /// </summary>
        public override async UniTask SpeakAsync(string text, CancellationToken ct)
        {
            var speakParameter = CurrentSpeakParameter.CurrentValue;
            await SpeakAsync(text, speakParameter, ct);
        }

        /// <summary>
        /// 発話する
        /// </summary>
        public override async UniTask SpeakAsync(string text, SpeakParameter speakParameter, CancellationToken ct)
        {
            _currentSpeakingCts ??= new CancellationTokenSource();

            using var lcts =
                CancellationTokenSource.CreateLinkedTokenSource(ct, _allCts.Token, _currentSpeakingCts.Token);
            var autoResetUniTaskCompletionSource = AutoResetUniTaskCompletionSource.Create();

            var synthesiser = _voicevoxProvider.Synthesizer.CurrentValue;

            // Voicevoxの音声合成を開始
            var task = synthesiser.SynthesizeSpeechAsync(
                text: text,
                styleId: speakParameter.Style.Id,
                speedScale: (decimal)speakParameter.SpeedScale,
                pitchScale: (decimal)speakParameter.PitchScale,
                volumeScale: (decimal)speakParameter.VolumeScale,
                cancellationToken: lcts.Token);

            // Observableを非同期処理を行えるQueueとして利用
            _speechRegisterSubject.OnNext((task, autoResetUniTaskCompletionSource, lcts.Token));

            // 読み上げが完了するまで待機
            await autoResetUniTaskCompletionSource.Task;
        }

        /// <summary>
        /// 現在の読み上げをキャンセルする
        /// </summary>
        public override void CancelSpeakingAll()
        {
            _currentSpeakingCts?.Cancel();
            _currentSpeakingCts?.Dispose();
            _currentSpeakingCts = null;
        }

        protected override void OnDisposed()
        {
            _currentSpeakingCts?.Cancel();
            _currentSpeakingCts?.Dispose();
            _allCts.Cancel();
            _allCts.Dispose();
            _speechRegisterSubject.Dispose(true);
            _currentSpeakingText.Dispose();

            Object.Destroy(_vrmGameObject);
            _onDisposeUniTaskCompletionSource.TrySetResult();
        }
    }
}