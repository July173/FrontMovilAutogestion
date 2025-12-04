using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AutogestionSena.MAUI.Api.Dtos
{
    public class ModuleFormDto
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("icon")]
        public string? Icon { get; set; }
        // Asegura que la propiedad coincida exactamente con el JSON: "form"
        [JsonPropertyName("form")]
        public List<FormDto>? Form { get; set; }
    }
}