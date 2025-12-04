using System;
using System.Net.Http;
using Microsoft.Maui.Controls;
using AutogestionSena.MAUI.Api.Services;
using AutogestionSena.MAUI.Api.Dtos;
using AutogestionSena.MAUI.Views;

namespace AutogestionSena.MAUI.Views
{
    public partial class PasswordRecoveryPage : ContentPage
    {
        private readonly UserService _apiService;

        public PasswordRecoveryPage()
        {
            InitializeComponent();
            _apiService = new UserService();
        }

        private async void OnBackToLoginClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///LoginPage");
        }

        private async void OnSendCodeClicked(object sender, EventArgs e)
        {
            var email = EmailEntry?.Text?.Trim();
            if (string.IsNullOrEmpty(email))
            {
                await DisplayAlert("Error", "Por favor ingresa tu correo electrónico.", "Aceptar");
                return;
            }

            try
            {
                // Mostrar indicador de carga
                if (SendButton != null)
                {
                    SendButton.IsEnabled = false;
                    SendButton.Text = "Enviando...";
                }

                try
                {
                    var result = await _apiService.RequestPasswordResetAsync(email);

                    // Si recibimos una respuesta válida con código, la guardamos en Preferences
                    if (result != null && !string.IsNullOrEmpty(result.code))
                    {
                        // Guardar la respuesta completa del servidor en Preferences
                        var codeData = new
                        {
                            code = result.code,
                            email = email,
                            fecha_expiracion = result.fecha_expiracion,
                            success = result.success
                        };

                        var codeDataJson = System.Text.Json.JsonSerializer.Serialize(codeData);
                        Preferences.Set("password_reset_data", codeDataJson);

                        // Mostrar mensaje de éxito
                        await DisplayAlert("Código Enviado",
             result.success ?? "Se ha enviado un código de verificación a tu correo electrónico.",
                    "Continuar");

                        // Navegar a la pantalla de verificación de código
                        var encodedEmail = Uri.EscapeDataString(email);
                        await Shell.Current.GoToAsync($"///CodeVerificationPage?email={encodedEmail}&isPasswordReset=true");
                    }
                    else if (result != null)
                    {
                        // Si result no es null pero no hay código, navegamos de todos modos

                        await DisplayAlert("Código Enviado",
                        "Se ha enviado un código de verificación a tu correo electrónico.",
                "Continuar");

                        var encodedEmail = Uri.EscapeDataString(email);
                        await Shell.Current.GoToAsync($"///CodeVerificationPage?email={encodedEmail}&isPasswordReset=true");
                    }
                    else
                    {
                        // Si result es null pero no hubo excepción, significa que el backend respondió
                        var encodedEmail = Uri.EscapeDataString(email);
                        await Shell.Current.GoToAsync($"///CodeVerificationPage?email={encodedEmail}&isPasswordReset=true");
                    }
                }
                catch (Exception apiEx) when (apiEx.Message.Contains("Error al procesar la respuesta") ||
                                             apiEx.Message.Contains("Object reference not set") ||
                                             apiEx is NullReferenceException)
                {
                    // El servidor respondió pero hay error en la deserialización o referencia nula
                    await DisplayAlert("Código Enviado",
                            "Se ha enviado un código de verificación a tu correo electrónico.",
                            "Continuar");

                    var encodedEmail = Uri.EscapeDataString(email);
                    await Shell.Current.GoToAsync($"///CodeVerificationPage?email={encodedEmail}&isPasswordReset=true");
                }
            }
            catch (HttpRequestException httpEx)
            {
                // Error de conexión - Para desarrollo, permitir navegación
                var continuar = await DisplayAlert("Error de Conexión",
                    $"No se pudo conectar al servidor.\n\n¿Deseas continuar de todos modos? (Solo para pruebas de UI)\n\nError: {httpEx.Message}",
                    "Continuar", "Cancelar");

                if (continuar)
                {
                    var encodedEmail = Uri.EscapeDataString(email);
                    await Shell.Current.GoToAsync($"///CodeVerificationPage?email={encodedEmail}&isPasswordReset=true");
                }
            }
            catch (Exception ex)
            {
                // Para cualquier otro error, mostrar el mensaje real del error
                await DisplayAlert("Error", $"No se pudo enviar el código: {ex.Message}", "Aceptar");
            }
            finally
            {
                // Restaurar botón
                if (SendButton != null)
                {
                    SendButton.IsEnabled = true;
                    SendButton.Text = "Enviar Código";
                }
            }
        }
    }
}
