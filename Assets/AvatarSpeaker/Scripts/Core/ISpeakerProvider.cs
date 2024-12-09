using System.Threading;
using Cysharp.Threading.Tasks;

namespace AvatarSpeaker.Core
{
    public interface ISpeakerProvider
    {
        UniTask<Speaker> LoadSpeakerAsync(ISpeakerSource source, CancellationToken ct);
    }
}