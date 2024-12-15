using AvatarSpeaker.Core.Models;
using R3;

namespace AvatarSpeaker.Core.Interfaces
{
    /// <summary>
    /// 外部からの発話リクエストを提供する
    /// </summary>
    public interface ISpeakRequestProvider
    {
        Observable<SpeakRequest> OnSpeakRequest { get; }
    }
}