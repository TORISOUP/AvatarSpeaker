using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace AvatarSpeaker.Infrastructures.Https
{
    [DataContract(Name = "SpeakRequest")]
    public class SpeakRequestDto
    {
        [JsonPropertyName("text")] public string Text { get; set; }
    }
}