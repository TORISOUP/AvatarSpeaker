using System.Threading;
using Cysharp.Threading.Tasks;

namespace AvatarSpeaker.Core.Interfaces
{
    public interface ISpeakerSourceProvider
    {
        /// <summary>
        /// 使用可能なSpeakerSourceを一覧で取得する
        /// </summary>
        UniTask<ISpeakerSource[]> GetSpeakerSourcesAsync(CancellationToken ct);
    }
}