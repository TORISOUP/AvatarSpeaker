using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace AvatarSpeaker.Http.Models
{
    [DataContract(Name = "SpeakRequest")]
    public class SpeakRequestDto
    {
        [JsonPropertyName("text")] public string Text { get; set; }
    }
}