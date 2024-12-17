using System.Threading;
using Cysharp.Threading.Tasks;

namespace AvatarSpeaker.Core.Interfaces
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

        public UniTask<T> Accept<T>(ISpeakerSourceVisitor<T> visitor, CancellationToken ct)
        {
            return visitor.Visit(this, ct);
        }

        public override string ToString()
        {
            return $"{nameof(Path)}: {Path}";
        }
    }
}