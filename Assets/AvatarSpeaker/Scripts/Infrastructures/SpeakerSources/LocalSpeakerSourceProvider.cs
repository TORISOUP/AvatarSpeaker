using System;
using System.Threading;
using AvatarSpeaker.Core;
using Cysharp.Threading.Tasks;
using SimpleFileBrowser;

namespace AvatarSpeaker.Infrastructures.SpeakerSources
{
    public sealed class LocalSpeakerSourceProvider : ISpeakerSourceProvider, IDisposable
    {
        public async UniTask<ISpeakerSource> GetSpeakerSourcesAsync(CancellationToken ct)
        {
            await FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files).ToUniTask(cancellationToken: ct);
            return FileBrowser.Success
                ? new LocalSpeakerSource(FileBrowser.Result[0])
                : new LocalSpeakerSource(string.Empty);
        }

        public void Dispose()
        {
        }
    }
}