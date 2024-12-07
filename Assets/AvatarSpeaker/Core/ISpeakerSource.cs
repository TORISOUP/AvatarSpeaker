using System.Threading;
using Cysharp.Threading.Tasks;

namespace AvatarSpeaker.Core
{
    public interface ISpeakerSource
    {
        UniTask<T> Accept<T>(ISpeakerSourceVisitor<T> visitor, CancellationToken ct);
    }

    public interface ISpeakerSourceVisitor<T>
    {
        UniTask<T> Visit(LocalSpeakerSource source, CancellationToken ct);
    }
    
    public readonly struct LocalSpeakerSource : ISpeakerSource
    {
        public string Path { get; }

        public LocalSpeakerSource(string path)
        {
            Path = path;
        }

        public UniTask<T> Accept<T>(ISpeakerSourceVisitor<T> visitor, CancellationToken ct) => visitor.Visit(this, ct);
    }
}