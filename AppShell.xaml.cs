using AutogestionSenaMaui.Views;
using AutogestionSena.MAUI.Views;
using AutogestionSenaMaui.Helpers;
using AutogestionSena.MAUI.Views.notificaciones;

namespace AutogestionSena.MAUI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Deshabilitar el comportamiento del flyout (menú lateral)
            Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);

            // Registro de rutas de navegación
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(CodeVerificationPage), typeof(CodeVerificationPage));
            Routing.RegisterRoute(nameof(PasswordResetPage), typeof(PasswordResetPage));
            Routing.RegisterRoute(nameof(AdminDashboardPage), typeof(AdminDashboardPage));
            // Ruta para la página de notificaciones
            Routing.RegisterRoute("notifications", typeof(NotificacionesApp));

            // Dashboards de rol están definidos como ShellContent en AppShell.xaml

            // Suscribirse al evento de navegación para validar acceso
            Navigating += OnShellNavigating;
        }

        private async void OnShellNavigating(object? sender, ShellNavigatingEventArgs e)
        {
            try
            {
                // Obtener la ruta destino
                var targetRoute = e.Target.Location.OriginalString;
                
                System.Diagnostics.Debug.WriteLine($"[SHELL] Navegando a: {targetRoute}");

                // Rutas públicas que no requieren validación
                var publicRoutes = new[]
                {
                    "//LoginPage",
                    "//RegisterPage",
                    "//PasswordRecoveryPage",
                    "//CodeVerificationPage",
                    "//PasswordResetPage"
                };

                // Rutas protegidas que requieren autenticación
                var protectedRoutes = new[]
                {
                    "//HomePage",
                };

                // Si es una ruta pública, permitir navegación
                if (Array.Exists(publicRoutes, r => targetRoute.Contains(r)))
                {
                    return;
                }

                // Para rutas protegidas, verificar autenticación
                if (!NavigationHelper.IsUserAuthenticated())
                {
                    System.Diagnostics.Debug.WriteLine("[SHELL] Usuario no autenticado, cancelando navegación");
                    
                    // Cancelar navegación
                    e.Cancel();
                    
                    // Redirigir a login
                    await GoToAsync("//LoginPage");
                    
                    // Mostrar mensaje
                    if (Application.Current?.MainPage != null)
                    {
                        await Application.Current.MainPage.DisplayAlert(
                            "Sesión Requerida",
                            "Debes iniciar sesión para acceder a esta sección.",
                            "Aceptar"
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SHELL] Error en OnShellNavigating: {ex}");
            }
        }
    }
}
