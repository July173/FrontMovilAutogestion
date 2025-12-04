using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutogestionSena.MAUI.Api.Dtos;
using AutogestionSena.MAUI.Api;

namespace AutogestionSena.MAUI.Api.Services
{
    public class UserService
    {
        private readonly ApiService _apiService;

        public UserService()
        {
            _apiService = new ApiService();
        }

        public UserService(ApiService apiService)
        {
            _apiService = apiService;
        }

        /// <summary>
        /// Obtiene todos los tipos de documento disponibles
        /// </summary>
        public async Task<List<DocumentTypeDto>?> GetDocumentTypesAsync()
        {
            return await _apiService.GetAsync<List<DocumentTypeDto>>(Endpoints.DocumentType.GetAll);
        }

        /// <summary>
        /// Obtiene un usuario por su ID
        /// </summary>
        public async Task<UserDetailDto?> GetUserByIdAsync(int id)
        {
            try
            {
                var result = await _apiService.GetAsync<UserDetailDto>(Endpoints.User.GetUserId(id));
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Registra un nuevo usuario (aprendiz)
        /// </summary>
        public async Task<RegisterResponse?> RegisterUserAsync(User user)
        {
            return await _apiService.PostAsync<User, RegisterResponse>(Endpoints.Person.RegisterApprentice, user);
        }

        /// <summary>
        /// Registra un nuevo aprendiz con el payload correcto
        /// </summary>
        public async Task<RegisterResponse?> RegisterApprenticeAsync(RegisterPayloadDto payload)
        {
            return await _apiService.PostAsync<RegisterPayloadDto, RegisterResponse>(Endpoints.Person.RegisterApprentice, payload);
        }

        /// <summary>
        /// Valida el login institucional del usuario
        /// </summary>
        public async Task<ValidateLoginResponse?> ValidateLoginAsync(string email, string password)
        {
            var payload = new { email = email.Trim(), password = password.Trim() };
            return await _apiService.PostAsync<object, ValidateLoginResponse>(Endpoints.User.ValidateLogin, payload);
        }

        /// <summary>
        /// Solicita el restablecimiento de contraseña (envía código por email)
        /// </summary>
        public async Task<PasswordResetRequestResponse?> RequestPasswordResetAsync(string email)
        {
            var payload = new { email };
            return await _apiService.PostAsync<object, PasswordResetRequestResponse>(Endpoints.User.RequestPasswordReset, payload);
        }

        /// <summary>
        /// Restablece la contraseña usando email, código de verificación y nueva contraseña
        /// </summary>
        public async Task<PasswordResetResponse?> ResetPasswordAsync(string email, string newPassword, string code)
        {
            var payload = new { email, new_password = newPassword, code };
            return await _apiService.PostAsync<object, PasswordResetResponse>(Endpoints.User.ResetPassword, payload);
        }

        /// <summary>
        /// Restablece la contraseña usando email y nueva contraseña
        /// </summary>
        public async Task<PasswordResetResponse?> ResetPasswordAsync(string email, string newPassword)
        {
            var payload = new { email, new_password = newPassword };
            return await _apiService.PostAsync<object, PasswordResetResponse>(Endpoints.User.ResetPassword, payload);
        }

        /// <summary>
        /// Envía la petición de restablecimiento y devuelve el HttpResponseMessage crudo para escenarios donde
        /// la respuesta no puede deserializarse al DTO esperado. Esto ayuda en UIs que deben reaccionar al status code.
        /// </summary>
        public async Task<System.Net.Http.HttpResponseMessage?> ResetPasswordRawAsync(string email, string newPassword, string code)
        {
            var payload = new { email, new_password = newPassword, code };
            return await _apiService.PostAsync<object>(Endpoints.User.ResetPassword, payload);
        }

        /// <summary>
        /// Envía la petición de restablecimiento y devuelve el HttpResponseMessage crudo (método original para compatibilidad)
        /// </summary>
        public async Task<System.Net.Http.HttpResponseMessage?> ResetPasswordRawAsync(string email, string newPassword)
        {
            var payload = new { email, new_password = newPassword };
            return await _apiService.PostAsync<object>(Endpoints.User.ResetPassword, payload);
        }

        /// <summary>
        /// Envía la petición de restablecimiento y devuelve un objeto parseado con éxito / detalle / status code.
        /// </summary>
        public async Task<AutogestionSena.MAUI.Api.Dtos.ApiResponseParsedDto?> ResetPasswordParsedAsync(string email, string newPassword, string code)
        {
            var response = await ResetPasswordRawAsync(email, newPassword, code);
            if (response == null) return null;
            return await _apiService.ParseApiResponseAsync(response);
        }

        /// <summary>
        /// Envía la petición de restablecimiento y devuelve un objeto parseado (método original para compatibilidad)
        /// </summary>
        public async Task<AutogestionSena.MAUI.Api.Dtos.ApiResponseParsedDto?> ResetPasswordParsedAsync(string email, string newPassword)
        {
            var response = await ResetPasswordRawAsync(email, newPassword);
            if (response == null) return null;
            return await _apiService.ParseApiResponseAsync(response);
        }

        /// <summary>
        /// Valida el código de segundo factor de autenticación y retorna los tokens
        /// </summary>
        public async Task<ValidateLoginResponse?> ValidateSecondFactorAsync(SecondFactorRequest request)
        {
            return await _apiService.PostAsync<SecondFactorRequest, ValidateLoginResponse>(Endpoints.User.ValidateSecondFactor, request);
        }

        /// <summary>
        /// Configura el token de autenticación
        /// </summary>
        public void SetAuthToken(string token)
        {
            _apiService.SetAuthToken(token);
        }

        /// <summary>
        /// Limpia el token de autenticación
        /// </summary>
        public void ClearAuthToken()
        {
            _apiService.ClearAuthToken();
        }
    }
}