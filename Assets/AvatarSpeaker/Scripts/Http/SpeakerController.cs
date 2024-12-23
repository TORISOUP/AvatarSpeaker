using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AvatarSpeaker.Http.Models;
using AvatarSpeaker.Http.Server;
using AvatarSpeaker.UseCases;
using Cysharp.Threading.Tasks;

namespace AvatarSpeaker.Http
{
    /// <summary>
    /// /api/v1/speakers/
    /// </summary>
    public sealed class SpeakerController : BaseController
    {
        private readonly SpeakerUseCase _speakerUseCase;

        public SpeakerController(SpeakerUseCase speakerUseCase)
        {
            _speakerUseCase = speakerUseCase;
        }

        /// <summary>
        /// 現在のSpeakerの設定で発話する
        /// </summary>
        [Post("/api/v1/speakers/current/speak_current_parameters")]
        public async ValueTask SpeakCurrentParamsAsync(
            HttpListenerRequest request,
            HttpListenerResponse response,
            CancellationToken ct)
        {
            try
            {
                var data = await ReadAsJson<SpeakRequestCurrentParamsDto>(request, response);

                if (data.Text == null)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return;
                }

                // HTTPレスポンスはすぐに返したいのでForget
                _speakerUseCase.SpeakByCurrentSpeakerAsync(data.Text, default).Forget();

                response.StatusCode = (int)HttpStatusCode.OK;
            }
            catch (Exception)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        }


        /// <summary>
        /// 発話する
        /// </summary>
        [Post("/api/v1/speakers/current/speak")]
        public async ValueTask SpeakAsync(
            HttpListenerRequest request,
            HttpListenerResponse response,
            CancellationToken ct)
        {
            try
            {
                var data = await ReadAsJson<SpeakRequestDto>(request, response);

                if (data.Text == null)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return;
                }

                // SpeakerUseCaseを用いて発話命令を送る
                // HTTPレスポンスはすぐに返したいのでForget
                _speakerUseCase
                    .SpeakByCurrentSpeakerAsync(data.Text, data.Parameters.ToCore(), default)
                    .Forget();

                response.StatusCode = (int)HttpStatusCode.OK;
            }
            catch (Exception)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        }

        /// <summary>
        /// 現在の設定値を取得する
        /// </summary>
        [Get("/api/v1/speakers/current/parameters")]
        public async ValueTask GetParameters(
            HttpListenerRequest request,
            HttpListenerResponse response,
            CancellationToken ct)
        {
            try
            {
                var speaker = _speakerUseCase.GetCurrentSpeaker();
                if (speaker == null)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    return;
                }

                var parameters = speaker.CurrentSpeakParameter.CurrentValue;
                var dto = new SpeakParametersDto
                {
                    Style = parameters.Style.Id,
                    SpeedScale = parameters.SpeedScale,
                    PitchScale = parameters.PitchScale,
                    VolumeScale = parameters.VolumeScale
                };

                await SuccessAsJson(response, dto);
            }
            catch (Exception)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        }

        /// <summary>
        /// 現在の設定値を上書きする
        /// </summary>
        [Put("/api/v1/speakers/current/parameters")]
        public async ValueTask PutParameters(
            HttpListenerRequest request,
            HttpListenerResponse response,
            CancellationToken ct)
        {
            try
            {
                var speaker = _speakerUseCase.GetCurrentSpeaker();
                if (speaker == null)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    return;
                }

                var dto = await ReadAsJson<SpeakParametersDto>(request, response);
                var parameters = dto.ToCore();

                if (!parameters.Validate())
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return;
                }

                speaker.ChangeSpeakParameter(parameters);

                response.StatusCode = (int)HttpStatusCode.OK;
            }
            catch (Exception)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        }
    }
}