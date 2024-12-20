using System.Threading;
using Cysharp.Threading.Tasks;

namespace AvatarSpeaker.Core.Interfaces
{
    /// <summary>
    /// Speakerを提供する
    /// </summary>
    public interface ISpeakerProvider
    {
        /// <summary>
        /// SpeakerSourceを元にSpeakerを提供する
        /// </summary>
        UniTask<Speaker> LoadSpeakerAsync(ISpeakerSource source, CancellationToken ct);
    }
}