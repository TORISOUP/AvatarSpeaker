using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AvatarSpeaker.Http.Models;
using AvatarSpeaker.Http.Server;
using AvatarSpeaker.UseCases;

namespace AvatarSpeaker.Http
{
    public class MiscController : BaseController
    {
        private readonly SpeakerUseCase _speakerUseCase;

        public MiscController(SpeakerUseCase speakerUseCase)
        {
            _speakerUseCase = speakerUseCase;
        }

        /// <summary>
        /// 使用可能なスタイルを取得する
        /// </summary>
        [Get("/api/v1/misc/speak_styles")]
        public async ValueTask GetParameters(
            HttpListenerRequest request,
            HttpListenerResponse response,
            CancellationToken ct)
        {
            try
            {
                var styles = await _speakerUseCase.GetSpeakStylesAsync(ct);
                var dto = styles.Select(x => new SpeakStyleDto(x.Id, x.DisplayName)).ToArray();

                await SuccessAsJson(response, dto);
                response.Close();
            }
            catch (Exception)
            {
                // Voicevoxからの取得に失敗した場合は500を返す
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Close();
            }
        }
    }
}