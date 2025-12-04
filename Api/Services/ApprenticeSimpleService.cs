using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutogestionSena.MAUI.Api.Dtos;

namespace AutogestionSena.MAUI.Api.Services
{
    /// <summary>
    /// Servicio simple para obtener lista de aprendices
    /// </summary>
    public class ApprenticeSimpleService
    {
        private readonly ApiService _apiService;

        public ApprenticeSimpleService()
        {
            _apiService = new ApiService();
        }

        public ApprenticeSimpleService(ApiService apiService)
        {
            _apiService = apiService;
        }

        /// <summary>
        /// Obtiene todos los aprendices (lista simple)
        /// </summary>
        public async Task<List<ApprenticeSimpleDto>?> GetAllApprenticesAsync()
        {
            try
            {
                var response = await _apiService.GetAsync<List<ApprenticeSimpleDto>>(Endpoints.ApprenticeSimple.GetAllApprenticesSimple);
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Obtiene el conteo de aprendices activos
        /// </summary>
        public async Task<int> GetActiveApprenticesCountAsync()
        {
            try
            {
                var response = await GetAllApprenticesAsync();
                
                if (response == null)
                    return 0;

                return response.Count(x => x.Active);
            }
            catch (Exception)
            {
                return 0;
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
