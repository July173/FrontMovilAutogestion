using System.Text.Json.Serialization;

namespace AutogestionSena.MAUI.Api.Dtos
{
    /// <summary>
    /// DTO para tipos de documento
    /// Estructura del API: {"id":1,"name":"Cédula de Ciudadanía","acronyms":"CC","active":true}
    /// </summary>
    public class DocumentTypeDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("acronyms")]
        public string? Acronyms { get; set; }

        // Alias para compatibilidad con código existente
        public string? Abbreviation => Acronyms;

        [JsonPropertyName("active")]
        public bool Active { get; set; }
    }
}