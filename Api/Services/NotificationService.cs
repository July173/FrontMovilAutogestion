using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using AutogestionSena.MAUI.Api.Dtos;

namespace AutogestionSena.MAUI.Api.Services
{
    public class NotificationService
    {
        private readonly ApiService _apiService;

        public NotificationService()
        {
            _apiService = new ApiService();
        }

        public NotificationService(ApiService apiService)
        {
            _apiService = apiService;
        }

        /// <summary>
        /// Obtiene las notificaciones seg�n rol y id de usuario
        /// roleQueryName debe ser uno de: apprentice_id, instructor_id, coordinator_id, sofia_operator_id, admin_id
        /// </summary>
        public async Task<List<NotificationDto>?> GetNotificationsAsync(string roleQueryName, int userId)
        {
            try
            {
                var endpoint = $"{Endpoints.Notification.GetNotifications}?{roleQueryName}={userId}";
                return await _apiService.GetAsync<List<NotificationDto>>(endpoint);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Elimina (desactiva) una notificaci�n por id
        /// </summary>
        public async Task<bool> DeleteNotificationByIdAsync(int notificationId)
        {
            try
            {
                var endpoint = $"{Endpoints.Notification.DeleteById}?id={notificationId}";
                var resp = await _apiService.DeleteAsync(endpoint);
                return resp.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Desactiva (elimina) todas las notificaciones de un usuario seg�n rol
        /// roleQueryName debe ser admin_id, apprentice_id, etc.
        /// </summary>
        public async Task<bool> DeleteNotificationsByUserAsync(string roleQueryName, int userId)
        {
            try
            {
                var endpoint = $"{Endpoints.Notification.DeleteByUser}?{roleQueryName}={userId}";
                var resp = await _apiService.DeleteAsync(endpoint);
                return resp.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Marca como le�da una notificaci�n solicitando GET al recurso /general/notifications/{id}/
        /// </summary>
        public async Task<NotificationDto?> MarkAsReadAsync(int notificationId)
        {
            try
            {
                var endpoint = Endpoints.Notification.GetById(notificationId);
                return await _apiService.GetAsync<NotificationDto>(endpoint);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
