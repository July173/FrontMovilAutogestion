using AutogestionSena.MAUI.Views;
using AutogestionSenaMaui.Helpers;

namespace AutogestionSena.MAUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Use Shell as root MainPage for consistent navigation across the app
            MainPage = new AppShell();
            
            // Verificar sesión al iniciar y redirigir según corresponda
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await InitializeNavigationAsync();
            });
            
            // Capturar excepciones no controladas para ayudar en debugging en dispositivos
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        /// <summary>
        /// Inicializa la navegación según el estado de autenticación del usuario
        /// Por política de seguridad, siempre se requiere login al iniciar la app
        /// </summary>
        private async Task InitializeNavigationAsync()
        {
            try
            {
                await Task.Delay(100); // Pequeño delay para que el Shell se inicialice

                // POLÍTICA DE SEGURIDAD: Limpiar sesión anterior al iniciar la app
                // El usuario debe iniciar sesión cada vez que abre la aplicación
                System.Diagnostics.Debug.WriteLine("[APP] Limpiando sesión anterior por política de seguridad");
                
                // Limpiar tokens y datos de sesión
                Preferences.Remove("AuthToken");
                Preferences.Remove("RefreshToken");
                Preferences.Remove("UserRole");
                Preferences.Remove("user_data");
                Preferences.Remove("UserId");
                
                // Limpiar SecureStorage
                try
                {
                    SecureStorage.Remove("user_data");
                    SecureStorage.Remove("AuthToken");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[APP] Error limpiando SecureStorage: {ex}");
                }

                // Siempre navegar a LoginPage al iniciar
                System.Diagnostics.Debug.WriteLine("[APP] Navegando a LoginPage (inicio requerido)");
                
                if (MainPage is Shell shell)
                {
                    await shell.GoToAsync("///LoginPage");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[APP] Error en InitializeNavigationAsync: {ex}");
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"[UNHANDLED] {e.ExceptionObject}");
            // Intentar mostrar una alerta amigable si hay un MainPage
            try
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    if (Current?.MainPage != null)
                    {
                        await Current.MainPage.DisplayAlert("Error inesperado", "Se ha producido un error inesperado. Por favor, vuelve a intentarlo.", "Aceptar");
                    }
                });
            }
            catch { }
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"[UNOBSERVED TASK] {e.Exception}");
            e.SetObserved();
        }
    }
}
