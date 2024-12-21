using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AvatarSpeaker.Core.Models;

namespace AvatarSpeaker.Http.Models
{
    [DataContract(Name = "SpeakRequest")]
    public class SpeakRequestDto
    {
        [JsonPropertyName("text")] public string Text { get; set; }
        [JsonPropertyName("Parameters")] public SpeakParametersDto Parameters { get; set; }
    }

    [DataContract(Name = "SpeakRequestCurrentParams")]
    public class SpeakRequestCurrentParamsDto
    {
        [JsonPropertyName("text")] public string Text { get; set; }
    }

    [DataContract(Name = "SpeakParameters")]
    public class SpeakParametersDto
    {
        [JsonPropertyName("style")] public int Style { get; set; }
        [JsonPropertyName("speedScale")] public float SpeedScale { get; set; }
        [JsonPropertyName("pitchScale")] public float PitchScale { get; set; }
        [JsonPropertyName("volumeScale")] public float VolumeScale { get; set; }

        public SpeakParameter ToCore()
        {
            return new SpeakParameter(new SpeakStyle(Style, ""), SpeedScale, PitchScale, VolumeScale);
        }
    }

    [DataContract(Name = "SpeakStyle")]
    public class SpeakStyleDto
    {
        public SpeakStyleDto(int id, string displayName)
        {
            Id = id;
            DisplayName = displayName;
        }

        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("displayName")] public string DisplayName { get; set; }
    }
}