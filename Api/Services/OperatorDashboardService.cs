using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using AutogestionSena.MAUI.Api.Dtos;

namespace AutogestionSena.MAUI.Api.Services
{
    /// <summary>
    /// Servicio para obtener datos del dashboard del operador Sofia
    /// </summary>
    public class OperatorDashboardService
    {
        private readonly HttpClient _httpClient;

        public OperatorDashboardService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(Endpoints.API_BASE_URL)
            };
        }

        /// <summary>
        /// Obtiene los datos del dashboard del operador Sofia
        /// </summary>
        /// <param name="token">Token de autenticación (opcional)</param>
        /// <returns>Datos del dashboard</returns>
        public async Task<OperatorDashboardResponseDto?> GetOperatorDashboardAsync(string? token = null)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, Endpoints.SofiaOperator.Dashboard);

                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    return JsonSerializer.Deserialize<OperatorDashboardResponseDto>(jsonContent, options);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Error al obtener dashboard: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Excepción al obtener dashboard: {ex.Message}");
                return null;
            }
        }
    }
}
