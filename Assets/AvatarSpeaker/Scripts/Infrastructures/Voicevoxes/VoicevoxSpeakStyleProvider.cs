using System.Linq;
using System.Threading;
using AvatarSpeaker.Core;
using AvatarSpeaker.Core.Interfaces;
using AvatarSpeaker.Core.Models;
using Cysharp.Threading.Tasks;
using R3;

namespace AvatarSpeaker.Infrastructures.Voicevoxes
{
    public sealed class VoicevoxSpeakStyleProvider : ISpeakStyleProvider
    {
        private readonly VoicevoxProvider _voicevoxProvider;

        public VoicevoxSpeakStyleProvider(VoicevoxProvider voicevoxProvider)
        {
            _voicevoxProvider = voicevoxProvider;
        }

        /// <summary>
        /// Voicevoxから使用可能なSpeakStyleを取得する
        /// </summary>
        public async UniTask<SpeakStyle[]> GetSpeakStylesAsync(CancellationToken ct)
        {
            var synthesizer =
                await _voicevoxProvider.Synthesizer.FirstAsync(x => x != null, cancellationToken: ct);

            // ここの「Speaker」は「VoicevoxClientSharp.Speaker」
            var voicevoxSpeakers = await synthesizer.GetSpeakersAsync(ct);

            return voicevoxSpeakers
                .SelectMany(sp => sp.Styles.Select(x => new SpeakStyle(x.Id, $"{sp.Name}-{x.Name}")))
                .ToArray();
        }
    }
}