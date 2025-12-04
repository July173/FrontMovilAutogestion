using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using AutogestionSenaMaui.Helpers;

namespace AutogestionSenaMaui.Views
{
    /// <summary>
    /// Clase base para páginas protegidas que requieren autenticación.
    /// Similar a ProtectedRoute en React.
    /// </summary>
    public abstract class ProtectedPage : ContentPage
    {
        /// <summary>
        /// Roles permitidos para acceder a esta página.
        /// Si está vacío, todos los roles autenticados pueden acceder.
        /// </summary>
        protected virtual int[] AllowedRoles => Array.Empty<int>();

        /// <summary>
        /// Indica si se debe validar la sesión cada vez que aparece la página
        /// </summary>
        protected virtual bool ValidateOnAppearing => true;

        protected ProtectedPage()
        {
            // Suscribirse al evento Appearing para validar sesión
            if (ValidateOnAppearing)
            {
                Appearing += OnPageAppearing;
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (ValidateOnAppearing)
            {
                await ValidateAccessAsync();
            }
        }

        private async void OnPageAppearing(object? sender, EventArgs e)
        {
            await ValidateAccessAsync();
        }

        /// <summary>
        /// Valida el acceso a la página
        /// </summary>
        protected virtual async Task<bool> ValidateAccessAsync()
        {
            try
            {
                // Verificar si el usuario está autenticado
                if (!NavigationHelper.IsUserAuthenticated())
                {
                    await OnUnauthorizedAccess();
                    return false;
                }

                var userRole = NavigationHelper.GetUserRole();

                // Si hay roles permitidos definidos, verificar permisos
                if (AllowedRoles.Length > 0 && !Array.Exists(AllowedRoles, r => r == userRole))
                {
                    await OnForbiddenAccess(userRole);
                    return false;
                }

                await OnAuthorizedAccess(userRole);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Se llama cuando el usuario no está autenticado
        /// </summary>
        protected virtual async Task OnUnauthorizedAccess()
        {
            await DisplayAlert(
                "Sesión Requerida",
                "Debes iniciar sesión para acceder a esta sección.",
                "Aceptar"
            );

            // Redirigir a login
            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync("///LoginPage", true);
            }
        }

        /// <summary>
        /// Se llama cuando el usuario no tiene permisos para acceder
        /// </summary>
        protected virtual async Task OnForbiddenAccess(int userRole)
        {
            await DisplayAlert(
                "Acceso Denegado",
                $"No tienes permisos para acceder a esta sección.\nTu rol: {NavigationHelper.GetRoleName(userRole)}",
                "Aceptar"
            );

            // Redirigir al dashboard correspondiente
            await NavigationHelper.NavigateToDashboardAsync(true);
        }

        /// <summary>
        /// Se llama cuando el usuario tiene acceso autorizado
        /// </summary>
        protected virtual Task OnAuthorizedAccess(int userRole)
        {
            // Las páginas derivadas pueden sobrescribir este método
            return Task.CompletedTask;
        }

        /// <summary>
        /// Navega de forma segura a otra ruta
        /// </summary>
        protected async Task NavigateToAsync(string route)
        {
            await NavigationHelper.NavigateToAsync(route);
        }

        /// <summary>
        /// Cierra sesión desde la página
        /// </summary>
        protected async Task LogoutAsync()
        {
            var confirm = await DisplayAlert(
                "Cerrar Sesión",
                "¿Estás seguro que deseas cerrar sesión?",
                "Sí",
                "No"
            );

            if (confirm)
            {
                await NavigationHelper.LogoutAsync();
            }
        }
    }
}
