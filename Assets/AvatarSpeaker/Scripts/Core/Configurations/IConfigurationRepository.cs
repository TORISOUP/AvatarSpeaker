using R3;

namespace AvatarSpeaker.Core.Configurations
{
    /// <summary>
    /// 現在のアプリケーション内での設定値を表す
    /// </summary>
    public interface IConfigurationRepository
    {
        /// <summary>
        /// 「音声の制御」を行うための接続設定
        /// </summary>
        ReactiveProperty<VoiceControlConnectionSettings> VoiceControlConnectionSettings { get; }

        /// <summary>
        /// Httpサーバーの設定
        /// </summary>
        ReactiveProperty<HttpServerSettings> HttpServerSettings { get; }

        /// <summary>
        /// 字幕の有効/無効
        /// </summary>
        ReactiveProperty<bool> IsSubtitleEnabled { get; }
    }
}