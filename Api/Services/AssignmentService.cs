using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutogestionSena.MAUI.Api.Dtos;

namespace AutogestionSena.MAUI.Api.Services
{
    /// <summary>
    /// Servicio para manejar las solicitudes de asignación
    /// </summary>
    public class AssignmentService
    {
        private readonly ApiService _apiService;

        public AssignmentService()
        {
            _apiService = new ApiService();
        }

        public AssignmentService(ApiService apiService)
        {
            _apiService = apiService;
        }

        /// <summary>
        /// Obtiene todas las solicitudes de asignación
        /// </summary>
        public async Task<AssignmentRequestListResponse?> GetFormRequestListAsync()
        {
            try
            {
                var response = await _apiService.GetAsync<AssignmentRequestListResponse>(Endpoints.Assignment.GetFormRequestList);
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Obtiene el conteo de solicitudes sin asignar
        /// </summary>
        public async Task<int> GetUnassignedCountAsync()
        {
            try
            {
                var response = await GetFormRequestListAsync();
                
                if (response?.Data == null)
                    return 0;

                return response.Data.Count(x => x.RequestState == "SIN_ASIGNAR");
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Obtiene todas las solicitudes que están sin asignar
        /// </summary>
        public async Task<List<AssignmentRequestDto>?> GetUnassignedRequestsAsync()
        {
            try
            {
                var response = await GetFormRequestListAsync();
                
                if (response?.Data == null)
                    return new List<AssignmentRequestDto>();

                return response.Data.Where(x => x.RequestState == "SIN_ASIGNAR").ToList();
            }
            catch (Exception)
            {
                return new List<AssignmentRequestDto>();
            }
        }

        /// <summary>
        /// Configura el token de autenticación
        /// </summary>
        public void SetAuthToken(string token)
        {
            _apiService.SetAuthToken(token);
        }
    }
}
