using System.Threading;
using Cysharp.Threading.Tasks;

namespace AvatarSpeaker.Core.Interfaces
{
    /// <summary>
    /// 音声制御の機構
    /// （事実上のVoicevoxApiClientの抽象化）
    /// </summary>
    public interface IVoiceController
    {
        /// <summary>
        /// 音声制御機構が使用可能かどうか
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        UniTask<bool> IsReadyAsync(CancellationToken ct);
    }
}