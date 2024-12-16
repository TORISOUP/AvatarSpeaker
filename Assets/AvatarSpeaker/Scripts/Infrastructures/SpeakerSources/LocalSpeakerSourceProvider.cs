using System;
using System.Threading;
using AvatarSpeaker.Core;
using AvatarSpeaker.Core.Interfaces;
using Cysharp.Threading.Tasks;
using SimpleFileBrowser;

namespace AvatarSpeaker.Infrastructures.SpeakerSources
{
    public sealed class LocalSpeakerSourceProvider : ISpeakerSourceProvider, IDisposable
    {
        /// <summary>
        /// 使用可能なSpeakerSourceを一覧で取得する
        /// ただし実装として「ローカルから一つ選択する」なので、常に一つのみを返す
        /// </summary>
        public async UniTask<ISpeakerSource[]> GetSpeakerSourcesAsync(CancellationToken ct)
        {
            await FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files).ToUniTask(cancellationToken: ct);

            return FileBrowser.Success
                ? new ISpeakerSource[] { new LocalSpeakerSource(FileBrowser.Result[0]) }
                : Array.Empty<ISpeakerSource>();
        }

        public void Dispose()
        {
        }
    }
}