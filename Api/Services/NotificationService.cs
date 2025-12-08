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
        /// Obtiene las notificaciones según rol y id de usuario
        /// roleQueryName debe ser uno de: apprentice_id, instructor_id, coordinator_id, sofia_operator_id, admin_id
        /// Retorna lista vacía si no hay notificaciones (404) o si hay error
        /// </summary>
        public async Task<List<NotificationDto>> GetNotificationsAsync(string roleQueryName, int userId)
        {
            try
            {
                var endpoint = $"{Endpoints.Notification.GetNotifications}?{roleQueryName}={userId}";
                System.Diagnostics.Debug.WriteLine($"[NotificationService] Llamando endpoint: {endpoint}");
                
                var result = await _apiService.GetAsync<List<NotificationDto>>(endpoint);
                
                if (result == null)
                {
                    System.Diagnostics.Debug.WriteLine("[NotificationService] Resultado null, retornando lista vacía");
                    return new List<NotificationDto>();
                }
                
                System.Diagnostics.Debug.WriteLine($"[NotificationService] Obtenidas {result.Count} notificaciones");
                return result;
            }
            catch (Exception ex)
            {
                // Si el error es 404 "No hay notificaciones", retornar lista vacía
                if (ex.Message.Contains("404") || ex.Message.Contains("No hay notificaciones") || ex.Message.Contains("Not Found"))
                {
                    System.Diagnostics.Debug.WriteLine("[NotificationService] 404 - No hay notificaciones para este usuario");
                    return new List<NotificationDto>();
                }
                
                System.Diagnostics.Debug.WriteLine($"[NotificationService] Error: {ex.Message}");
                // Para otros errores, retornar lista vacía también para no bloquear la UI
                return new List<NotificationDto>();
            }
        }

        /// <summary>
        /// Elimina (desactiva) una notificación por id
        /// </summary>
        public async Task<bool> DeleteNotificationByIdAsync(int notificationId)
        {
            try
            {
                var endpoint = $"{Endpoints.Notification.DeleteById}?id={notificationId}";
                var resp = await _apiService.DeleteAsync(endpoint);
                return resp.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NotificationService] Error al eliminar: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Desactiva (elimina) todas las notificaciones de un usuario según rol
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NotificationService] Error al eliminar todas: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Marca como leída una notificación solicitando GET al recurso /general/notifications/{id}/
        /// </summary>
        public async Task<NotificationDto?> MarkAsReadAsync(int notificationId)
        {
            try
            {
                var endpoint = Endpoints.Notification.GetById(notificationId);
                return await _apiService.GetAsync<NotificationDto>(endpoint);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NotificationService] Error al marcar como leída: {ex.Message}");
                return null;
            }
        }
    }
}
