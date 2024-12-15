using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace AvatarSpeaker.Infrastructures.Https
{
    [DataContract(Name = "SpeakRequest")]
    public class SpeakRequestDto
    {
        [JsonPropertyName("text")] public string Text { get; set; }
        [JsonPropertyName("style")] public int Style { get; set; }
        [JsonPropertyName("speedScale")] public float SpeedScale { get; set; }
        [JsonPropertyName("pitchScale")] public float PitchScale { get; set; }
        [JsonPropertyName("volumeScale")] public float VolumeScale { get; set; }
    }
}