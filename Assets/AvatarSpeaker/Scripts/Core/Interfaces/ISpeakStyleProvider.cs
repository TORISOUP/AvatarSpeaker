using System.Threading;
using AvatarSpeaker.Core.Models;
using Cysharp.Threading.Tasks;

namespace AvatarSpeaker.Core.Interfaces
{
    public interface ISpeakStyleProvider
    {
        UniTask<SpeakStyle[]> GetSpeechStylesAsync(CancellationToken ct);
    }
}