using System;
using System.Threading;
using AvatarSpeaker.Core.Interfaces;
using Cysharp.Threading.Tasks;

namespace AvatarSpeaker.Infrastructures.Voicevoxes
{
    public sealed class VoicevoxVoiceController : IVoiceController
    {
        private readonly VoicevoxProvider _voicevoxProvider;

        public VoicevoxVoiceController(VoicevoxProvider voicevoxProvider)
        {
            _voicevoxProvider = voicevoxProvider;
        }

        public async UniTask<bool> IsReadyAsync(CancellationToken ct)
        {
            var apiClient = _voicevoxProvider.ApiClient.CurrentValue;
            if (apiClient == null) return false;

            try
            {
                // バージョンを取得して成功したら使用可能
                await apiClient.GetVersionAsync(ct);
                return true;
            }
            catch (Exception e) when (e is not OperationCanceledException)
            {
                return false;
            }
        }
    }
}