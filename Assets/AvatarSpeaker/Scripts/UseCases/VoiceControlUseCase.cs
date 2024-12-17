using System.Threading;
using AvatarSpeaker.Core.Interfaces;
using Cysharp.Threading.Tasks;

namespace AvatarSpeaker.UseCases
{
    public sealed class VoiceControlUseCase
    {
        private readonly IVoiceController _voiceController;

        public VoiceControlUseCase(IVoiceController voiceController)
        {
            _voiceController = voiceController;
        }

        /// <summary>
        /// VoiceControllerが使用可能かどうか
        /// </summary>
        public UniTask<bool> IsReadyAsync(CancellationToken ct)
        {
            return _voiceController.IsReadyAsync(ct);
        }
    }
}