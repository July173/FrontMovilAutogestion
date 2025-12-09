using System;
using System.Text.Json;
using System.Threading.Tasks;
using AutogestionSena.MAUI.Api.Dtos;
using AutogestionSena.MAUI.Api;

namespace AutogestionSena.MAUI.Api.Services
{
    /// <summary>
    /// Servicio para operaciones del instructor
    /// </summary>
    public class InstructorService
    {
        private readonly ApiService _apiService;

        public InstructorService()
        {
            _apiService = new ApiService();
        }

        public InstructorService(ApiService apiService)
        {
            _apiService = apiService;
        }

        /// <summary>
        /// Obtiene el dashboard del instructor con estadísticas y próximas visitas
        /// Endpoint: GET /api/general/instructors/{id}/dashboard/
        /// </summary>
        public async Task<InstructorDashboardResponseDto?> GetInstructorDashboardAsync(int instructorId)
        {
            try
            {
                var endpoint = Endpoints.Instructor.GetDashboard(instructorId);
                var fullUrl = $"{Endpoints.API_BASE_URL}{endpoint}";
                System.Diagnostics.Debug.WriteLine($"[InstructorService] Llamando endpoint: {fullUrl}");

                var response = await _apiService.GetAsync<InstructorDashboardResponseDto>(endpoint);
                
                if (response != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[InstructorService] Respuesta recibida - Success: {response.Success}");
                    if (response.Data != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"[InstructorService] Stats: Visitas={response.Data.Stats?.VisitasProgramadas}, Asignados={response.Data.Stats?.AprendicesAsignados}, Evaluados={response.Data.Stats?.AprendicesEvaluados}");
                        System.Diagnostics.Debug.WriteLine($"[InstructorService] Próximas visitas: {response.Data.ProximasVisitas?.Count ?? 0}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("[InstructorService] Data es null");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[InstructorService] Response es null");
                }
                
                return response;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[InstructorService] Error al obtener dashboard: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[InstructorService] Stack: {ex.StackTrace}");
                throw;
            }
        }

        /// <summary>
        /// Obtiene el dashboard con manejo de errores y valores por defecto
        /// </summary>
        public async Task<InstructorDashboardDataDto> GetDashboardDataSafeAsync(int instructorId)
        {
            try
            {
                var response = await GetInstructorDashboardAsync(instructorId);
                
                if (response?.Success == true && response.Data != null)
                {
                    return response.Data;
                }

                return CreateEmptyDashboardData();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[InstructorService] Error seguro: {ex.Message}");
                return CreateEmptyDashboardData();
            }
        }

        /// <summary>
        /// Crea un objeto de dashboard vacío para cuando no hay datos
        /// </summary>
        private static InstructorDashboardDataDto CreateEmptyDashboardData()
        {
            return new InstructorDashboardDataDto
            {
                Stats = new InstructorStatsDto
                {
                    VisitasProgramadas = 0,
                    AprendicesAsignados = 0,
                    AprendicesEvaluados = 0
                },
                ProximasVisitas = new List<ProximaVisitaDto>()
            };
        }

        /// <summary>
        /// Obtiene los detalles de un instructor por ID
        /// Endpoint: GET /api/general/instructors/{id}/
        /// </summary>
        public async Task<InstructorDetailDto?> GetInstructorDetailsAsync(int instructorId)
        {
            try
            {
                var endpoint = Endpoints.Instructor.GetInstructor(instructorId);
                return await _apiService.GetAsync<InstructorDetailDto>(endpoint);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[InstructorService] Error al obtener detalles: {ex.Message}");
                throw;
            }
        }
    }
}
