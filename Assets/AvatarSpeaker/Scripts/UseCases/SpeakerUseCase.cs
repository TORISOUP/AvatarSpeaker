using System.Threading;
using AvatarSpeaker.Core;
using Cysharp.Threading.Tasks;

namespace AvatarSpeaker.UseCases
{
    /// <summary>
    /// Speaker関係の操作を提供するUseCase
    /// </summary>
    public sealed class SpeakerUseCase
    {
        private readonly ISpeakerSourceProvider _speakerSourceProvider;

        public SpeakerUseCase(ISpeakerSourceProvider speakerSourceProvider)
        {
            _speakerSourceProvider = speakerSourceProvider;
        }

        /// <summary>
        /// 使用可能なSpeakerSourceを一覧で取得する
        /// </summary>
        public UniTask<ISpeakerSource[]> GetAvailableSpeakerSourcesAsync(CancellationToken ct)
        {
            return _speakerSourceProvider.GetSpeakerSourcesAsync(ct);
        }
    }
}