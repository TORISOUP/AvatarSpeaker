using System;
using System.Threading;
using System.Threading.Tasks;
using AvatarSpeaker.Core;
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
    public class VoicevoxSpeaker : Speaker
    {
        // GameObjectのIDをSpeakerのIDとして利用
        public sealed override string Id { protected set; get; }

        private readonly GameObject _vrmGameObject;
        private readonly VoicevoxSynthesizerProvider _voicevoxSynthesizerProvider;
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        private readonly Subject<(ValueTask<SynthesisResult>, AutoResetUniTaskCompletionSource, CancellationToken)>
            _speechRegisterSubject =
                new();

        /// <summary>
        /// Dispose時に発火する
        /// </summary>
        public Action OnDisposed;

        public VoicevoxSpeaker(Vrm10Instance vrm10Instance, VoicevoxSynthesizerProvider synthesizerProvider)
        {
            // SpeakerのIDを設定
            Id = $"voicevox_vrm_{vrm10Instance.gameObject.GetInstanceID().ToString()}";

            _voicevoxSynthesizerProvider = synthesizerProvider;
            _vrmGameObject = vrm10Instance.gameObject;
            var audioSource = _vrmGameObject.AddComponent<AudioSource>();
            var lipSync = _vrmGameObject.AddComponent<VoicevoxVrmLipSyncPlayer>();

            var voicevoxSpeakPlayer = _vrmGameObject.AddComponent<VoicevoxSpeakPlayer>();
            voicevoxSpeakPlayer.AudioSource = audioSource;
            voicevoxSpeakPlayer.AddOptionalVoicevoxPlayer(lipSync);

            // 音声合成のタスクが流れてくる
            _speechRegisterSubject
                .SubscribeAwait(async (values, ct) =>
                {
                    var (task, autoResetUniTaskCompletionSource, ctsToken) = values;
                    try
                    {
                        // 音声合成のタスクが完了するまで待機
                        var result = await task;
                        ct.ThrowIfCancellationRequested();
                        await voicevoxSpeakPlayer.PlayAsync(result, ctsToken);
                        autoResetUniTaskCompletionSource.TrySetResult();
                    }
                    catch (OperationCanceledException e)
                    {
                        autoResetUniTaskCompletionSource.TrySetCanceled();
                    }
                    catch (Exception e)
                    {
                        autoResetUniTaskCompletionSource.TrySetException(e);
                    }
                })
                .RegisterTo(_cancellationTokenSource.Token);
        }


        public override async UniTask SpeechAsync(SpeechParameter speechParameter, CancellationToken ct)
        {
            var lcts = CancellationTokenSource.CreateLinkedTokenSource(ct, _cancellationTokenSource.Token);
            var autoResetUniTaskCompletionSource = AutoResetUniTaskCompletionSource.Create();

            var synthesiser = _voicevoxSynthesizerProvider.Current.CurrentValue;

            // Voicevoxの音声合成を開始
            var task = synthesiser.SynthesizeSpeechAsync(
                text: speechParameter.Text,
                styleId: speechParameter.Style.Id,
                speedScale: (decimal)speechParameter.SpeedScale,
                pitchScale: (decimal)speechParameter.PitchScale,
                volumeScale: (decimal)speechParameter.VolumeScale,
                cancellationToken: lcts.Token);

            // Observableを非同期処理を行えるQueueとして利用
            _speechRegisterSubject.OnNext((task, autoResetUniTaskCompletionSource, lcts.Token));

            // 読み上げが完了するまで待機
            await autoResetUniTaskCompletionSource.Task;
        }

        public override void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _speechRegisterSubject.Dispose(true);
            Object.Destroy(_vrmGameObject);
            OnDisposed?.Invoke();
        }
    }
}