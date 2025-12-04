using System;
using Microsoft.Maui.Controls;
using AutogestionSena.MAUI.Api.Dtos;
using AutogestionSenaMaui.Helpers;
using AutogestionSena.MAUI.Api.Services;
using AutogestionSena.MAUI.Views;

namespace AutogestionSena.MAUI.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly UserService _userService;
        private string _currentEmail = string.Empty;
        private string _currentPassword = string.Empty;

        public LoginPage()
        {
            InitializeComponent();
            _userService = new UserService();
            
            // Suscribirse a eventos del modal
            TwoFactorModal.CodeVerified += OnCodeVerified;
            TwoFactorModal.Cancelled += OnTwoFactorCancelled;
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string? username = UsernameEntry.Text?.Trim();
            string? password = PasswordEntry.Text?.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Por favor ingresa usuario y contraseña.", "Aceptar");
                return;
            }

            // Validación adicional: solo correos institucionales
            if (!AutogestionSena.MAUI.Validators.LoginValidator.IsSenaEmail(username))
            {
                await DisplayAlert("Error", "Utiliza un correo institucional: @soy.sena.edu.co o @sena.edu.co.", "Aceptar");
                return;
            }

            try
            {
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;
                IsEnabled = false;

                // Guardar credenciales para usar después del 2FA
                _currentEmail = username;
                _currentPassword = password;

                var response = await _userService.ValidateLoginAsync(username, password);

                if (response != null)
                {
                    // Si hay token, significa que el login fue exitoso
                    // Ahora mostrar el modal de segundo factor
                    LoadingIndicator.IsVisible = false;
                    LoadingIndicator.IsRunning = false;
                    
                    TwoFactorModal.Show(username);
                }
                else
                {
                    await DisplayAlert("Error", "Usuario o contraseña incorrectos.", "Aceptar");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un problema al iniciar sesión: {ex.Message}", "Aceptar");
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
                IsEnabled = true;
            }
        }

        private async void OnRegisterLinkTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///RegisterPage");
        }

        private async void OnForgotPasswordTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///PasswordRecoveryPage");
        }

        private async void OnCodeVerified(object? sender, string code)
        {
            try
            {
                var request = new SecondFactorRequest
                {
                    Email = _currentEmail,
                    Code = code
                };

                // Validar el código 2FA directamente - la API devuelve los tokens
                var response = await _userService.ValidateSecondFactorAsync(request);

                if (response != null && !string.IsNullOrEmpty(response.Access))
                {
                    // Guardar tokens
                    Preferences.Set("AuthToken", response.Access);
                    if (!string.IsNullOrEmpty(response.Refresh))
                    {
                        Preferences.Set("RefreshToken", response.Refresh);
                    }
                    
                    _userService.SetAuthToken(response.Access);
                    
                    // Guardar datos del usuario para el menú dinámico y para toda la app
                    try
                    {
                        int roleId =0;
                        string firstName = string.Empty;
                        
                        if (response.User != null)
                        {
                            roleId = response.User.Role;
                            firstName = response.User.Email ?? _currentEmail;
                            // Guardar datos completos del usuario
                            Preferences.Set("UserEmail", response.User.Email ?? "");
                            Preferences.Set("UserId", response.User.Id);
                            Preferences.Set("UserRole", response.User.Role);
                            Preferences.Set("UserPerson", response.User.Person);
                            Preferences.Set("UserRegistered", response.User.Registered);
                        }
                        else if (response.Role != null)
                        {
                            roleId = response.Role.Value;
                        }

                        var userToSave = new
                        {
                            firstName = firstName,
                            roleId = roleId
                        };
                        var userJson = System.Text.Json.JsonSerializer.Serialize(userToSave);
                        Preferences.Set("user_data", userJson);
                        try
                        {
                            await SecureStorage.SetAsync("user_data", userJson);
                        }
                        catch (Exception)
                        {
                            // SecureStorage not available
                        }
                    }
                    catch (Exception)
                    {
                        // Error saving user_data handled silently
                    }
                    
                    TwoFactorModal.ShowSuccess();
                    
                    // Navegar según el roleId
                    int navigateRoleId =0;
                    if (response.User != null) navigateRoleId = response.User.Role;
                    else if (response.Role != null) navigateRoleId = response.Role.Value;

                    // Guardar role en Preferences para menú y navegación futura
                    try
                    {
                        Preferences.Set("UserRole", navigateRoleId);
                    }
                    catch (Exception)
                    {
                        // Error saving UserRole handled silently
                    }

                    // Navegar a HomePage que cargará el dashboard apropiado según el rol
                    string route = "HomePage";
                    
                    // Notificar a subscriptores (DynamicSideMenuViewModel) que el usuario ha iniciado sesión
                    try
                    {
                        AuthEvents.NotifyUserLoggedIn(navigateRoleId, (response.User?.Email ?? _currentEmail), response.Access ?? string.Empty);
                    }
                    catch (Exception)
                    {
                        // AuthEvents notification error handled silently
                    }

                    try
                    {
                        if (Shell.Current != null)
                        {
                            await Shell.Current.GoToAsync($"///{route}");
                        }
                    }
                    catch (Exception)
                    {
                        await DisplayAlert("Error", "No se pudo navegar al dashboard.", "Aceptar");
                    }
                }
                else
                {
                    TwoFactorModal.ShowError("Código incorrecto. Por favor intenta de nuevo.");
                }
            }
            catch (Exception ex)
            {
                TwoFactorModal.ShowError($"Error al verificar el código: {ex.Message}");
            }
        }

        private void OnTwoFactorCancelled(object? sender, EventArgs e)
        {
            IsEnabled = true;
            _currentEmail = string.Empty;
            _currentPassword = string.Empty;
        }
    }
}
