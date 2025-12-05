using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AutogestionSena.MAUI.Api.Dtos
{
    /// <summary>
    /// DTO para la respuesta del dashboard del operador Sofia
    /// </summary>
    public class OperatorDashboardResponseDto
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public OperatorDashboardDataDto? Data { get; set; }
    }

    /// <summary>
    /// Datos del dashboard del operador
    /// </summary>
    public class OperatorDashboardDataDto
    {
        [JsonPropertyName("year")]
        public int Year { get; set; }

        [JsonPropertyName("monthly_data")]
        public List<MonthlyDataDto> MonthlyData { get; set; } = new();

        [JsonPropertyName("totals")]
        public DashboardTotalsDto? Totals { get; set; }
    }

    /// <summary>
    /// Datos mensuales para el gráfico
    /// </summary>
    public class MonthlyDataDto
    {
        [JsonPropertyName("month")]
        public string Month { get; set; } = string.Empty;

        [JsonPropertyName("month_number")]
        public int MonthNumber { get; set; }

        [JsonPropertyName("registered")]
        public int Registered { get; set; }

        [JsonPropertyName("pending")]
        public int Pending { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }
    }

    /// <summary>
    /// Totales del dashboard
    /// </summary>
    public class DashboardTotalsDto
    {
        [JsonPropertyName("registered")]
        public int Registered { get; set; }

        [JsonPropertyName("pending")]
        public int Pending { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }
    }
}
