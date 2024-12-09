using System.Threading;
using AvatarSpeaker.Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;

namespace AvatarSpeaker.StartUp
{
    public sealed class EntryPoint : IAsyncStartable
    {
        private readonly ISpeakerSourceProvider _speakerSourceProvider;
        private readonly ISpeakerProvider _speakerProvider;

        public EntryPoint(ISpeakerSourceProvider speakerSourceProvider, ISpeakerProvider speakerProvider)
        {
            _speakerSourceProvider = speakerSourceProvider;
            _speakerProvider = speakerProvider;
        }

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            var vrm = await _speakerSourceProvider.GetSpeakerSourcesAsync(cancellation);
            // vrmを使って何かする

            var speaker = await _speakerProvider.LoadSpeakerAsync(vrm, cancellation);
            // speakerを使って何かする
            
            speaker.SpeechAsync(new SpeechParameter("こんにちは"), cancellation).Forget();
        }
    }
}