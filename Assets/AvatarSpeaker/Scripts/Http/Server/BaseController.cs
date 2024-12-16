using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace AvatarSpeaker.Http.Server
{
    public class BaseController
    {
        internal async ValueTask<T> ReadAsJson<T>(
            HttpListenerRequest req,
            HttpListenerResponse res)
        {
            using var reader = new StreamReader(req.InputStream);
            var json = await reader.ReadToEndAsync();
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}