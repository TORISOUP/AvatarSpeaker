using System;
using System.Threading;
using AvatarSpeaker.Core;
using AvatarSpeaker.Infrastructures.Voicevoxes;
using AvatarSpeaker.Scripts.Externals;
using Cysharp.Threading.Tasks;
using Object = UnityEngine.Object;

namespace AvatarSpeaker.Infrastructures.VoicevoxSpeakers
{
    /// <summary>
    /// VOICEVOXを利用したSpeaker実装
    /// </summary>
    public sealed class VoicevoxSpeakerProvider : ISpeakerProvider, ISpeakerSourceVisitor<Speaker>, IDisposable
    {
        private readonly VoicevoxSynthesizerProvider _voicevoxSynthesizerProvider;
        private readonly GameObjectRepository _gameObjectRepository;

        public VoicevoxSpeakerProvider(VoicevoxSynthesizerProvider voicevoxSynthesizerProvider,
            GameObjectRepository gameObjectRepository)
        {
            _voicevoxSynthesizerProvider = voicevoxSynthesizerProvider;
            _gameObjectRepository = gameObjectRepository;
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

            // Speakerを生成
            var speaker = new VoicevoxSpeaker(vrmInstance, _voicevoxSynthesizerProvider);

            // Speakerに紐づいたGameObjectを登録しておく（Viewで使う用）
            _gameObjectRepository.Register(speaker.Id, vrmInstance.gameObject);
            // Speakerが削除されたらRepositoryから解除
            speaker.OnDisposed = () => _gameObjectRepository.Remove(speaker.Id);

            return speaker;
        }

        public void Dispose()
        {
            // do nothing
        }
    }
}