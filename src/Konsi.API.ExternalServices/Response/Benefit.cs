using System.Text.Json.Serialization;

namespace Konsi.API.ExternalServices.Response;

public class Benefit
{
    [JsonPropertyName("numero_beneficio")]
    public string NumberBenefit { get; set; }

    [JsonPropertyName("codigo_tipo_beneficio")]
    public string CodeTypeBenefit { get; set; }
}
