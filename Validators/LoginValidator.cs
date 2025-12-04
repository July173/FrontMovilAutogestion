using System;

namespace AutogestionSena.MAUI.Validators
{
    /// <summary>
    /// Clase que contiene métodos de validación para el formulario de login
    /// Esta clase es pura y no depende de MAUI, facilitando pruebas unitarias
    /// </summary>
    public static class LoginValidator
    {
        /// <summary>
        /// Valida si el username es válido (no vacío ni espacios en blanco)
        /// </summary>
        public static bool IsUsernameValid(string? username)
        {
            return !string.IsNullOrWhiteSpace(username);
        }

        /// <summary>
        /// Valida si el password es válido (no vacío ni espacios en blanco)
        /// </summary>
        public static bool IsPasswordValid(string? password)
        {
            return !string.IsNullOrWhiteSpace(password);
        }

        /// <summary>
        /// Valida si ambas credenciales son válidas
        /// </summary>
        public static bool AreCredentialsValid(string? username, string? password)
        {
            return IsUsernameValid(username) && IsPasswordValid(password);
        }

        /// <summary>
        /// Obtiene el mensaje de error para username inválido
        /// </summary>
        public static string GetUsernameErrorMessage()
        {
            return "Ingresa tu usuario";
        }

        /// <summary>
        /// Obtiene el mensaje de error para password inválido
        /// </summary>
        public static string GetPasswordErrorMessage()
        {
            return "Ingresa tu contraseña";
        }

        

        /// <summary>
        /// Valida si el código 2FA tiene el formato correcto (6 dígitos)
        /// </summary>
        public static bool Is2FACodeValid(string? code)
        {
            return !string.IsNullOrWhiteSpace(code) && code.Length == 6 && code.All(char.IsDigit);
        }

        /// <summary>
        /// Valida que el correo sea institucional:
        /// - @soy.sena.edu.co
        /// - @sena.edu.co
        /// </summary>
        /// <param name="email">Email a validar</param>
        /// <returns>True si el email es institucional, false en otro caso</returns>
        public static bool IsSenaEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            try
            {
                // Normaliza, valida formato de email y revisa dominios
                var normalized = email.Trim().ToLowerInvariant();
                var addr = new System.Net.Mail.MailAddress(normalized);
                normalized = addr.Address;
                return normalized.EndsWith("@soy.sena.edu.co") || normalized.EndsWith("@sena.edu.co");
            }
            catch
            {
                // Si no es un email válido, devolvemos false
                return false;
            }
        }
    }
}
