using System;
using System.Threading;
using System.Threading.Tasks;
using AvatarSpeaker.Core.Models;
using AvatarSpeaker.Cushion.VRM;
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
        private readonly Animator _animator;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly ReactiveProperty<string> _currentSpeakingText = new("");

        private readonly UniTaskCompletionSource _onDisposeUniTaskCompletionSource = new();

        private readonly Subject<(ValueTask<SynthesisResult>, AutoResetUniTaskCompletionSource, CancellationToken)>
            _speechRegisterSubject =
                new();

        private readonly VoicevoxProvider _voicevoxProvider;

        private readonly GameObject _vrmGameObject;

        public VoicevoxSpeaker(Vrm10Instance vrm10Instance, VoicevoxProvider provider)
        {
            GameObject = vrm10Instance.gameObject;
            Vrm10Instance = vrm10Instance;

            // SpeakerのIDを設定
            Id = $"voicevox_vrm_{vrm10Instance.gameObject.GetInstanceID().ToString()}";

            _voicevoxProvider = provider;
            _vrmGameObject = vrm10Instance.gameObject;
            _animator = _vrmGameObject.GetComponent<Animator>();

            var audioSource = _vrmGameObject.AddComponent<AudioSource>();
            var lipSync = _vrmGameObject.AddComponent<VoicevoxVrmLipSyncPlayer>();

            var voicevoxSpeakPlayer = _vrmGameObject.AddComponent<VoicevoxSpeakPlayer>();
            voicevoxSpeakPlayer.AudioSource = audioSource;
            voicevoxSpeakPlayer.AddOptionalVoicevoxPlayer(lipSync);

            // 音声合成のタスクが流れてくる
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
                .RegisterTo(_cancellationTokenSource.Token);
        }

        // GameObjectのIDをSpeakerのIDとして利用
        public sealed override string Id { get; }

        public override GameObject GameObject { get; }
        public override Vrm10Instance Vrm10Instance { get; }

        public override ReadOnlyReactiveProperty<string> CurrentSpeakingText => _currentSpeakingText;

        /// <summary>
        /// 両目の中心の位置を顔の位置として利用する
        /// </summary>
        public override Vector3 FacePosition
        {
            get
            {
                var leftEye = _animator.GetBoneTransform(HumanBodyBones.LeftEye).position;
                var rightEye = _animator.GetBoneTransform(HumanBodyBones.RightEye).position;
                return (leftEye + rightEye) / 2;
            }
        }

        /// <summary>
        /// 腰の位置をSpeakerの前方向として利用する
        /// </summary>
        public override Vector3 BodyForward => _animator.GetBoneTransform(HumanBodyBones.Chest).forward;

        public override UniTask OnDisposeAsync => _onDisposeUniTaskCompletionSource.Task;


        public override async UniTask SpeakAsync(string text, CancellationToken ct)
        {
            var speakParameter = CurrentSpeakParameter.CurrentValue;
            await SpeakAsync(text, speakParameter, ct);
        }

        public override async UniTask SpeakAsync(string text, SpeakParameter speakParameter, CancellationToken ct)
        {
            var lcts = CancellationTokenSource.CreateLinkedTokenSource(ct, _cancellationTokenSource.Token);
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

        protected override void OnDisposed()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _speechRegisterSubject.Dispose(true);
            _currentSpeakingText.Dispose();

            Object.Destroy(_vrmGameObject);
            _onDisposeUniTaskCompletionSource.TrySetResult();
        }
    }
}