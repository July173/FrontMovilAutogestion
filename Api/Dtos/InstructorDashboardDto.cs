using System.Text.Json.Serialization;

namespace AutogestionSena.MAUI.Api.Dtos
{
    /// <summary>
    /// DTO para la respuesta del dashboard del instructor
    /// Endpoint: GET /api/general/instructors/{id}/dashboard/
    /// </summary>
    public class InstructorDashboardResponseDto
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("data")]
        public InstructorDashboardDataDto? Data { get; set; }
    }

    public class InstructorDashboardDataDto
    {
        [JsonPropertyName("stats")]
        public InstructorStatsDto? Stats { get; set; }

        [JsonPropertyName("proximas_visitas")]
        public List<ProximaVisitaDto>? ProximasVisitas { get; set; }
    }

    public class InstructorStatsDto
    {
        [JsonPropertyName("visitas_programadas")]
        public int VisitasProgramadas { get; set; }

        [JsonPropertyName("aprendices_asignados")]
        public int AprendicesAsignados { get; set; }

        [JsonPropertyName("aprendices_evaluados")]
        public int AprendicesEvaluados { get; set; }
    }

    public class ProximaVisitaDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("aprendiz_nombre")]
        public string? AprendizNombre { get; set; }

        [JsonPropertyName("aprendiz_identificacion")]
        public long AprendizIdentificacion { get; set; }

        [JsonPropertyName("programa")]
        public string? Programa { get; set; }

        [JsonPropertyName("tipo_visita")]
        public string? TipoVisita { get; set; }

        [JsonPropertyName("fecha_programada")]
        public string? FechaProgramada { get; set; }

        [JsonPropertyName("fecha_texto")]
        public string? FechaTexto { get; set; }

        [JsonPropertyName("asignacion_id")]
        public int AsignacionId { get; set; }

        [JsonPropertyName("visita_id")]
        public int VisitaId { get; set; }

        [JsonPropertyName("numero_ficha")]
        public long NumeroFicha { get; set; }
    }
}
