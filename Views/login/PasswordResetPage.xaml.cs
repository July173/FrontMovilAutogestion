using System;
using Microsoft.Maui.Controls;
using AutogestionSena.MAUI.Api.Services;

namespace AutogestionSena.MAUI.Views
{
    [QueryProperty(nameof(Email), "email")]
    [QueryProperty(nameof(Code), "code")]
    public partial class PasswordResetPage : ContentPage
    {
        private readonly UserService _apiService;
        private string _email = string.Empty;
        private string _code = string.Empty;

        public string Email
        {
            get => _email;
            set => _email = value;
        }

        public string Code
        {
            get => _code;
            set => _code = value;
        }

        public PasswordResetPage() : this(string.Empty, string.Empty)
        {
        }

        public PasswordResetPage(string email) : this(email, string.Empty)
        {
        }

        public PasswordResetPage(string email, string code)
        {
            InitializeComponent();
            _apiService = new UserService();
            _email = email;
            _code = code;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            try
            {
                if (width <= 360)
                {
                    ResetTitleLabel.FontSize = 20;
                    ResetSubtitleLabel.FontSize = 18;
                    ResetLogoImage.HeightRequest = 60;
                    ResetLogoImage.WidthRequest = 60;
                    ResetButton.FontSize = 16;
                }
                else if (width <= 420)
                {
                    ResetTitleLabel.FontSize = 24;
                    ResetSubtitleLabel.FontSize = 20;
                    ResetLogoImage.HeightRequest = 80;
                    ResetLogoImage.WidthRequest = 80;
                    ResetButton.FontSize = 18;
                }
                else
                {
                    ResetTitleLabel.FontSize = 26;
                    ResetSubtitleLabel.FontSize = 22;
                    ResetLogoImage.HeightRequest = 90;
                    ResetLogoImage.WidthRequest = 90;
                    ResetButton.FontSize = 18;
                }
            }
            catch (Exception)
            {
                // Responsive sizing error handled silently
            }
        }

        private async void OnResetPasswordClicked(object sender, EventArgs e)
        {
            var newPassword = NewPasswordEntry.Text?.Trim();
            var confirmPassword = ConfirmPasswordEntry.Text?.Trim();

            // Validaciones
            if (string.IsNullOrEmpty(newPassword))
            {
                await DisplayAlert("Error", "Por favor ingresa la nueva contraseña.", "Aceptar");
                return;
            }

            if (newPassword.Length < 8)
            {
                await DisplayAlert("Error", "La contraseña debe tener al menos 8 caracteres.", "Aceptar");
                return;
            }

            if (newPassword != confirmPassword)
            {
                await DisplayAlert("Error", "Las contraseñas no coinciden.", "Aceptar");
                return;
            }

            // Nota: El código ya fue validado en CodeVerificationPage antes de llegar aquí

            try
            {
                // Mostrar indicador de carga
                if (ResetButton != null)
                {
                    ResetButton.IsEnabled = false;
                    ResetButton.Text = "Restableciendo...";
                }

                // Restablecer contraseña enviando SOLO email y nueva contraseña al endpoint
                // El código NO se envía al API, solo se usó para validación local
                var result = await _apiService.ResetPasswordAsync(_email, newPassword);

                if (result != null && !string.IsNullOrEmpty(result.success))
                {
                    // Mostrar el mensaje de éxito del servidor
                    await DisplayAlert("Éxito", result.success, "Aceptar");

                    // Volver al login
                    await Shell.Current.GoToAsync("///LoginPage");
                }
                else
                {
                    // Si no hay respuesta o está vacía
                    await DisplayAlert("Error", "No se pudo restablecer la contraseña. Por favor intenta nuevamente.", "Aceptar");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", 
                    $"Error al restablecer contraseña: {ex.Message}", 
                    "Aceptar");
            }
            finally
            {
                // Restaurar botón
                if (ResetButton != null)
                {
                    ResetButton.IsEnabled = true;
                    ResetButton.Text = "Restablecer contraseña";
                }
            }
        }

        private async void OnBackToLoginClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///LoginPage");
        }
    }
}
