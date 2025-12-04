using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;
using AutogestionSena.MAUI.Api.Services;
using AutogestionSena.MAUI.Api.Dtos;
using AutogestionSenaMaui.Views;
using AutogestionSenaMaui.Helpers;
using Microsoft.Maui.Storage;

namespace AutogestionSena.MAUI.Views
{
    [QueryProperty(nameof(Email), "email")]
    [QueryProperty(nameof(IsPasswordReset), "isPasswordReset")]
    public partial class CodeVerificationPage : ContentPage
    {
        private readonly UserService _apiService;
        private string _email = string.Empty;
        private bool _isPasswordReset = false;

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                UpdateInstructionText();
            }
        }

        public string IsPasswordReset
        {
            get => _isPasswordReset.ToString();
            set
            {
                _isPasswordReset = bool.TryParse(value, out var result) && result;
                UpdateInstructionText();
            }
        }

        public CodeVerificationPage() : this(string.Empty, false)
        {
        }

        public CodeVerificationPage(string email, bool isPasswordReset = false)
        {
            InitializeComponent();
            _apiService = new UserService();
            _email = email;
            _isPasswordReset = isPasswordReset;
            UpdateInstructionText();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            // Ajustar tamaños de fuente y elementos según el ancho de pantalla
            try
            {
                if (width <= 360)
                {
                    TitleLabel.FontSize = 22;
                    SubtitleLabel.FontSize = 18;
                    InstructionLabel.FontSize = 13;
                    VerifyButton.FontSize = 16;
                    LogoImage.HeightRequest = 90;
                    LogoImage.WidthRequest = 90;
                }
                else if (width <= 420)
                {
                    TitleLabel.FontSize = 26;
                    SubtitleLabel.FontSize = 20;
                    InstructionLabel.FontSize = 15;
                    VerifyButton.FontSize = 18;
                    LogoImage.HeightRequest = 110;
                    LogoImage.WidthRequest = 110;
                }
                else
                {
                    TitleLabel.FontSize = 30;
                    SubtitleLabel.FontSize = 22;
                    InstructionLabel.FontSize = 16;
                    VerifyButton.FontSize = 18;
                    LogoImage.HeightRequest = 120;
                    LogoImage.WidthRequest = 120;
                }
            }
            catch (Exception)
            {
                // Responsive sizing error handled silently
            }
        }

        private void UpdateInstructionText()
        {
            if (InstructionLabel != null)
            {
                if (_isPasswordReset)
                {
                    InstructionLabel.Text = "Ingresa el código que recibiste en tu correo electrónico para continuar con la recuperación de contraseña.";
                }
                else
                {
                    InstructionLabel.Text = "Ingresa el código de verificación de dos factores que recibiste en tu correo electrónico.";
                }
            }
        }

        private async void OnVerifyCodeClicked(object sender, EventArgs e)
        {
            var code = CodeEntry?.Text?.Trim();
            if (string.IsNullOrEmpty(code))
            {
                await DisplayAlert("Error", "Por favor ingresa el código de verificación.", "Aceptar");
                return;
            }

            // Validaciones de formato
            if (code.Length != 6)
            {
                await DisplayAlert("Error", "El código debe tener exactamente 6 dígitos.", "Aceptar");
                return;
            }

            if (!code.All(char.IsDigit))
            {
                await DisplayAlert("Error", "El código solo debe contener números.", "Aceptar");
                return;
            }

            try
            {
                if (_isPasswordReset)
                {
                    // Flujo de recuperación de contraseña - validar código localmente
                    if (!ValidateStoredCode(code))
                    {
                        await DisplayAlert("Error", "Código inválido o expirado. Por favor solicita un nuevo código.", "Aceptar");
                        return;
                    }

                    // Si el código es válido, navegar a la pantalla de cambio de contraseña
                    var encodedEmail = Uri.EscapeDataString(_email);
                    var encodedCode = Uri.EscapeDataString(code ?? string.Empty);

                    // Limpiar los datos del código una vez validado
                    Preferences.Remove("password_reset_data");

                    await Shell.Current.GoToAsync($"///PasswordResetPage?email={encodedEmail}&code={encodedCode}");
                }
                else
                {
                    // Flujo de 2FA para login - validar con el servidor
                    var result = await _apiService.ValidateSecondFactorAsync(new SecondFactorRequest
                    {
                        Email = _email,
                        Code = code
                    });

                    if (result != null && !string.IsNullOrEmpty(result.Access))
                    {
                        // Guardar tokens (usando Preferences en lugar de SecureStorage para evitar incompatibilidades)
                        try
                        {
                            Preferences.Set("AuthToken", result.Access ?? string.Empty);
                            Preferences.Set("RefreshToken", result.Refresh ?? string.Empty);
                        }
                        catch (Exception)
                        {
                            // Error saving tokens handled silently
                        }


                        // Guardar datos del usuario (asegurar que el json tenga firstName y roleId para el menú dinámico)
                        int roleId = 0;
                        string firstName = string.Empty;
                        try
                        {
                            if (result.User != null)
                            {
                                roleId = result.User.Role;
                                firstName = result.User.Email ?? string.Empty;
                            }
                            else if (result.Role != null)
                            {
                                int.TryParse(result.Role.ToString(), out roleId);
                            }

                            var userToSave = new
                            {
                                firstName = firstName,
                                roleId = roleId
                            };
                            var userJson = System.Text.Json.JsonSerializer.Serialize(userToSave);
                            Preferences.Set("user_data", userJson);
                            // Guardar también en SecureStorage para que el DynamicSideMenu lo lea
                            try
                            {
                                await SecureStorage.SetAsync("user_data", userJson);
                            }
                            catch (Exception)
                            {
                                // SecureStorage.SetAsync error handled silently
                            }
                            // Guardar rol explícito
                            try
                            {
                                Preferences.Set("UserRole", roleId);
                            }
                            catch (Exception)
                            {
                                // Error saving UserRole handled silently
                            }
                        }
                        catch (Exception)
                        {
                            // Error saving user_data handled silently
                        }

                        // Configurar token para el servicio para llamadas subsecuentes (ej: cargar menú)
                        try
                        {
                            _apiService.SetAuthToken(result.Access ?? string.Empty);
                        }
                        catch (Exception)
                        {
                            // Error setting token handled silently
                        }

                        // Navegar a la página correspondiente según el roleId. Default: MainDashboard
                        int navigateRoleId = 0;
                        if (result.User != null) navigateRoleId = result.User.Role;
                        else if (result.Role != null) navigateRoleId = result.Role.Value;

                        // Navegar a HomePage que cargará el dashboard apropiado según el rol
                        string route = "HomePage";

                        // Notificar a subscriptores (DynamicSideMenuViewModel) que el usuario ha iniciado sesión
                        try
                        {
                            AuthEvents.NotifyUserLoggedIn(navigateRoleId, firstName, result.Access ?? string.Empty);
                        }
                        catch (Exception)
                        {
                            // AuthEvents error handled silently
                        }

                        try
                        {
                            // Usar NavigationHelper para navegación segura
                            await NavigationHelper.NavigateToAsync(route);
                        }
                        catch (Exception)
                        {
                            await DisplayAlert("Error", "No se pudo navegar al dashboard.", "Aceptar");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Error", "Código inválido o expirado.", "Aceptar");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al verificar código: {ex.Message}", "Aceptar");
            }
        }

        private void OnCodeTextChanged(object sender, TextChangedEventArgs e)
        {
  // Validar que solo se ingresen números
  if (sender is Entry entry)
      {
         var newText = e.NewTextValue;
                if (!string.IsNullOrEmpty(newText))
             {
// Filtrar solo números
   var numericText = new string(newText.Where(char.IsDigit).ToArray());
   
          // Limitar a 6 caracteres
         if (numericText.Length > 6)
          {
            numericText = numericText.Substring(0, 6);
 }
      
             // Solo actualizar si hay cambios para evitar bucles infinitos
       if (numericText != newText)
      {
      entry.Text = numericText;
       }
      }
     }
        }

        private bool ValidateStoredCode(string inputCode)
  {
       try
 {
      // Obtener los datos guardados del código
      var codeDataJson = Preferences.Get("password_reset_data", string.Empty);
    if (string.IsNullOrEmpty(codeDataJson))
      {
            return false;
     }

 var codeData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(codeDataJson);
         if (codeData == null || !codeData.ContainsKey("code"))
      {
     return false;
            }

   var storedCode = codeData["code"]?.ToString();
           var expirationDate = codeData.ContainsKey("fecha_expiracion") ? codeData["fecha_expiracion"]?.ToString() : null;

         // Validar fecha de expiración si está disponible
          if (!string.IsNullOrEmpty(expirationDate))
    {
            try
        {
     // Formato esperado: "24/11/2025 22:19"
    if (DateTime.TryParseExact(expirationDate, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime expiry))
             {
                if (DateTime.Now > expiry)
         {
          return false;
   }
          }
         }
      catch (Exception)
        {
             // Si no podemos validar la fecha, continuamos con la validación del código
        }
                }

           // Validar el código
    return !string.IsNullOrEmpty(storedCode) && storedCode.Equals(inputCode, StringComparison.OrdinalIgnoreCase);
         }
    catch (Exception)
    {
        return false;
            }
        }
    }
}
