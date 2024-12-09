using System.Threading;
using Cysharp.Threading.Tasks;

namespace AvatarSpeaker.Core
{
    public interface ISpeakerSourceProvider
    {
        UniTask<ISpeakerSource> GetSpeakerSourcesAsync(CancellationToken ct);
    }
}