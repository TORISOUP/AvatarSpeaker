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
            return source.Accept(this, ct);
        }

        public async UniTask<Speaker> Visit(LocalSpeakerSource source, CancellationToken ct)
        {
            var vrmInstance = await Vrm10.LoadPathAsync(source.Path, ct: ct);
            if (ct.IsCancellationRequested)
            {
                Object.Destroy(vrmInstance.gameObject);
                ct.ThrowIfCancellationRequested();
            }

            // Speakerを生成
            var speaker = new VoicevoxSpeaker(vrmInstance, _voicevoxProvider);
            return speaker;
        }
    }
}