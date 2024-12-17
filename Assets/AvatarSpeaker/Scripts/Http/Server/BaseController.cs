using System.IO;
using System.Net;
using System.Text;
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

        internal async ValueTask SuccessAsJson<T>(
            HttpListenerResponse res,
            T data)
        {
            res.ContentType = "application/json";
            res.StatusCode = (int)HttpStatusCode.OK;
            res.ContentEncoding = Encoding.UTF8;
            await using var writer = new StreamWriter(res.OutputStream);
            await writer.WriteAsync(JsonSerializer.Serialize(data));
        }
    }
}