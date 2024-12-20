using System.Threading;
using Cysharp.Threading.Tasks;

namespace AvatarSpeaker.Core.Interfaces
{
    /// <summary>
    /// Speakerのロード元を表す
    /// </summary>
    public interface ISpeakerSource
    {
        UniTask<T> Accept<T>(ISpeakerSourceVisitor<T> visitor, CancellationToken ct);
    }

    /// <summary>
    /// VisitorパターンでISpeakerSourceを処理するためのインタフェース
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISpeakerSourceVisitor<T>
    {
        UniTask<T> Visit(LocalSpeakerSource source, CancellationToken ct);
    }

    /// <summary>
    /// ローカルファイルからSpeakerをロードするための実装
    /// </summary>
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