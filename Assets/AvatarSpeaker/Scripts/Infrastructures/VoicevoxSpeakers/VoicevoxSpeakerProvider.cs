using System;
using System.Threading;
using AvatarSpeaker.Core;
using AvatarSpeaker.Core.Interfaces;
using AvatarSpeaker.Infrastructures.Voicevoxes;
using Cysharp.Threading.Tasks;
using UniVRM10;
using Object = UnityEngine.Object;

namespace AvatarSpeaker.Infrastructures.VoicevoxSpeakers
{
    /// <summary>
    /// VOICEVOXを利用したSpeaker実装を提供する
    /// </summary>
    public sealed class VoicevoxSpeakerProvider : ISpeakerProvider, ISpeakerSourceVisitor<Speaker>, IDisposable
    {
        private readonly VoicevoxProvider _voicevoxProvider;

        public VoicevoxSpeakerProvider(VoicevoxProvider voicevoxProvider)
        {
            _voicevoxProvider = voicevoxProvider;
        }

        public void Dispose()
        {
            // do nothing
        }

        public UniTask<Speaker> LoadSpeakerAsync(ISpeakerSource source, CancellationToken ct)
        {
            // VisitorパターンでISpeakerSourceを処理する
            return source.Accept(this, ct);
        }

        /// <summary>
        /// LocalSpeakerSourceに対する実装
        /// </summary>
        public async UniTask<Speaker> Visit(LocalSpeakerSource source, CancellationToken ct)
        {
            // ローカルパスからVRMをロードしてInstantiate
            var vrmInstance = await Vrm10.LoadPathAsync(source.Path, ct: ct);
            
            // LoadPathAsyncでのctの扱い方が不明なので
            // ここでキャンセルされていたら破棄して例外を投げる
            if (ct.IsCancellationRequested)
            {
                Object.Destroy(vrmInstance.gameObject);
                ct.ThrowIfCancellationRequested();
            }
            
            // VoicevoxSpeakerを生成
            var speaker = new VoicevoxSpeaker(vrmInstance, _voicevoxProvider);
            return speaker;
        }
    }
}