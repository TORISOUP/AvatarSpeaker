using System;
using System.Threading;
using AvatarSpeaker.Core;
using AvatarSpeaker.Infrastructures.Voicevoxs;
using Cysharp.Threading.Tasks;
using Object = UnityEngine.Object;

namespace AvatarSpeaker.Infrastructures.VrmSpeakers
{
    /// <summary>
    /// VOICEVOXとVRMを利用したSpeaker実装
    /// </summary>
    public sealed class VoicevoxVrmSpeakerProvider : ISpeakerProvider, ISpeakerSourceVisitor<Speaker>, IDisposable
    {
        private readonly VoicevoxSynthesizerProvider _voicevoxSynthesizerProvider;

        public VoicevoxVrmSpeakerProvider(VoicevoxSynthesizerProvider voicevoxSynthesizerProvider)
        {
            _voicevoxSynthesizerProvider = voicevoxSynthesizerProvider;
        }

        public UniTask<Speaker> LoadSpeakerAsync(ISpeakerSource source, CancellationToken ct)
        {
            return source.Accept(this, ct);
        }

        public async UniTask<Speaker> Visit(LocalSpeakerSource source, CancellationToken ct)
        {
            var vrmInstance = await UniVRM10.Vrm10.LoadPathAsync(source.Path, ct: ct);
            if (ct.IsCancellationRequested)
            {
                Object.Destroy(vrmInstance.gameObject);
                ct.ThrowIfCancellationRequested();
            }

            return new VoicevoxVrmSpeaker(vrmInstance, _voicevoxSynthesizerProvider);
        }

        public void Dispose()
        {
            // do nothing
        }
    }
}