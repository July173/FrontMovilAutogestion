using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using AutogestionSena.MAUI.Api.Services;
using AutogestionSenaMaui.Helpers;
using AutogestionSena.MAUI.Api.Dtos;

namespace AutogestionSena.MAUI.ViewModels
{
    public class LoginViewModel : BindableObject
    {
        private readonly UserService _apiService;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private bool _isBusy;

        public LoginViewModel()
        {
            _apiService = new UserService();
            LoginCommand = new Command(async () => await LoginAsync(), () => !IsBusy);
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
                ((Command)LoginCommand).ChangeCanExecute();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
                ((Command)LoginCommand).ChangeCanExecute();
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
                ((Command)LoginCommand).ChangeCanExecute();
            }
        }

        public ICommand LoginCommand { get; }

        private async Task LoginAsync()
        {
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(Username))
            {
                await Application.Current?.MainPage?.DisplayAlert("Error", "Ingresa tu usuario", "Aceptar")!;
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current?.MainPage?.DisplayAlert("Error", "Ingresa tu contraseña", "Aceptar")!;
                return;
            }

            IsBusy = true;

                    try
            {
                var response = await _apiService.ValidateLoginAsync(Username, Password);

                if (response != null && !string.IsNullOrEmpty(response.Access))
                {
                    SaveUserDataAndNavigate(response);
                }
                else
                {
                    await Show2FAModal(Username);
                }
            }
            catch (Exception ex)
            {
                await Application.Current?.MainPage?.DisplayAlert(
                    "Error",
                    $"Error al iniciar sesión: {ex.Message}",
                    "Aceptar"
                )!;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task Show2FAModal(string email)
        {
            var code = await Application.Current?.MainPage?.DisplayPromptAsync(
                "Autenticación de Segundo Factor",
                $"Se envió un código de 6 dígitos a: {email}\n\nIngresa el código:",
                "Verificar",
                "Cancelar",
                maxLength: 6,
                keyboard: Keyboard.Numeric
            )!;

            if (!string.IsNullOrWhiteSpace(code) && code.Length == 6)
            {
                await Verify2FACode(email, code);
            }
        }

        private async Task Verify2FACode(string email, string code)
        {
            IsBusy = true;
            try
            {
                var request = new SecondFactorRequest
                {
                    Email = email,
                    Code = code
                };

                var response = await _apiService.ValidateSecondFactorAsync(request);

                if (response != null && !string.IsNullOrEmpty(response.Access))
                {
                    SaveUserDataAndNavigate(response);
                }
                else
                {
                    await Application.Current?.MainPage?.DisplayAlert(
                        "Error",
                        "Código inválido o expirado",
                        "Aceptar"
                    )!;
                }
            }
            catch (Exception ex)
            {
                await Application.Current?.MainPage?.DisplayAlert(
                    "Error",
                    $"Error al verificar código: {ex.Message}",
                    "Aceptar"
                )!;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void SaveUserDataAndNavigate(ValidateLoginResponse response)
        {
            Preferences.Set("AuthToken", response.Access ?? "");
            Preferences.Set("RefreshToken", response.Refresh ?? "");
            
            if (response.User != null)
            {
                Preferences.Set("UserEmail", response.User.Email ?? "");
                Preferences.Set("UserId", response.User.Id);
                Preferences.Set("UserRole", response.User.Role);
                Preferences.Set("UserPerson", response.User.Person);
                Preferences.Set("UserRegistered", response.User.Registered);
            }

            _apiService.SetAuthToken(response.Access ?? "");

            try
            {
                var userToSave = new { firstName = response.User?.Email ?? string.Empty, roleId = response.User?.Role ?? 0 };
                var userJson = System.Text.Json.JsonSerializer.Serialize(userToSave);
                Preferences.Set("user_data", userJson);
                await SecureStorage.SetAsync("user_data", userJson);
            }
            catch (Exception)
            {
                // SecureStorage save error handled silently
            }

            await Application.Current?.MainPage?.DisplayAlert(
                "Éxito",
                "Inicio de sesión exitoso",
                "Continuar"
            )!;

            int navigateRoleId = 0;
            if (response.User != null) navigateRoleId = response.User.Role;
            else if (response.Role != null) navigateRoleId = response.Role.Value;

            try
            {
                Preferences.Set("UserRole", navigateRoleId);
            }
            catch (Exception)
            {
                // Preferences save error handled silently
            }

            string route = "HomePage";
            switch (navigateRoleId)
            {
                case 1:
                    route = "SecurityMainPage";
                    break;
                case 2:
                    route = "ApprenticeDashboard";
                    break;
                case 3:
                    route = "InstructorDashboard";
                    break;
                case 4:
                    route = "CoordinatorDashboard";
                    break;
                case 5:
                    route = "SofiaOperatorDashboard";
                    break;
                default:
                    route = "HomePage";
                    break;
            }

            try
            {
                AuthEvents.NotifyUserLoggedIn(navigateRoleId, response.User?.Email ?? string.Empty, response.Access ?? string.Empty);
            }
            catch (Exception)
            {
                // AuthEvents notification error handled silently
            }
            try
            {
                await Shell.Current?.GoToAsync($"///{route}")!;
            }
            catch (Exception)
            {
                // Navigation error handled silently
            }
        }
    }
}