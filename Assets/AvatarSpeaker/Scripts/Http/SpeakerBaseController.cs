using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AvatarSpeaker.Http.Models;
using AvatarSpeaker.Http.Server;
using AvatarSpeaker.UseCases;
using Cysharp.Threading.Tasks;

namespace AvatarSpeaker.Http
{
    public sealed class SpeakerBaseController : BaseController
    {
        private readonly SpeakerUseCase _speakerUseCase;

        public SpeakerBaseController(SpeakerUseCase speakerUseCase)
        {
            _speakerUseCase = speakerUseCase;
        }


        [Post("/api/v1/speaker")]
        public async ValueTask SpeakAsync(
            HttpListenerRequest request,
            HttpListenerResponse response,
            CancellationToken ct)
        {
            try
            {
                var data = await ReadAsJson<SpeakRequestDto>(request, response);
                
                // HTTPレスポンスはすぐに返したいのでForget
                _speakerUseCase.SpeakByCurrentSpeakerAsync(data.Text, default).Forget();
                
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Close();
            }
            catch (Exception)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Close();
            }
        }
    }
}