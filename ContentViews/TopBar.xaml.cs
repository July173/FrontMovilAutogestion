using AutogestionSenaMaui.ViewModels;
using Microsoft.Maui.Controls;
using AutogestionSenaMaui.Helpers;
using System.Timers;
using AutogestionSena.MAUI.Views.notificaciones;

namespace AutogestionSenaMaui.ContentViews
{
    /// <summary>
    /// TopBar - Barra superior de navegación responsive con overlay modal
    /// Frame 425: Muestra breadcrumb navigation y menú desplegable flotante
    /// Adaptable para pantallas pequeñas y grandes con overlay completo
    /// </summary>
    public partial class TopBar : ContentView
    {
        private bool _isMenuOpen = false;
        private System.Timers.Timer? _autoHideTimer;
        private const int AUTO_HIDE_DELAY = 7000; // 7 segundos para overlay

        // ARMAR ACCESORES: Exponer el texto del breadcrumb sin crear conflictos con el members auto-generado
        public string BreadcrumbRootText
        {
            get => this.FindByName<Label>("BreadcrumbRoot")?.Text ?? string.Empty;
            set { var l = this.FindByName<Label>("BreadcrumbRoot"); if (l != null) l.Text = value; }
        }

        public string BreadcrumbCurrentText
        {
            get => this.FindByName<Label>("BreadcrumbCurrent")?.Text ?? string.Empty;
            set { var l = this.FindByName<Label>("BreadcrumbCurrent"); if (l != null) l.Text = value; }
        }

        public TopBar()
        {
            InitializeComponent();
            InitializeAutoHideTimer();
        }

        // Evento público para notificar clicks del botón de menú (TopBar)
        public event EventHandler? MenuButtonClicked;

        // Inicializar timer para auto-ocultar el menú
        private void InitializeAutoHideTimer()
        {
            _autoHideTimer = new System.Timers.Timer(AUTO_HIDE_DELAY);
            _autoHideTimer.Elapsed += OnAutoHideTimer;
            _autoHideTimer.AutoReset = false; // Solo se ejecuta una vez por inicio
        }

        // Override para detectar cambios de tamaño y aplicar diseño responsive
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            ApplyResponsiveLayout(width);
        }

        // Aplicar diseño responsive según el ancho de la pantalla
        private void ApplyResponsiveLayout(double width)
        {
            try
            {
                var dropdownMenu = this.FindByName<Border>("DropdownMenu");
                if (dropdownMenu != null)
                {
                    if (width < 400)
                    {
                        dropdownMenu.WidthRequest = width * 0.85;
                        dropdownMenu.Margin = new Thickness(10, 80, 10, 20);
                    }
                    else if (width < 600)
                    {
                        dropdownMenu.WidthRequest = 300;
                        dropdownMenu.Margin = new Thickness(15, 80, 15, 20);
                    }
                    else
                    {
                        dropdownMenu.WidthRequest = 320;
                        dropdownMenu.Margin = new Thickness(20, 80, 20, 20);
                    }
                }
            }
            catch (Exception)
            {
                // Layout error handled silently
            }
        }

        // Handler para el botón hamburguesa
        private async void OnHamburgerTapped(object sender, EventArgs e)
        {
            try
            {
                if (_isMenuOpen)
                {
                    await HideOverlayMenu();
                }
                else
                {
                    await ShowOverlayMenu();
                }
            }
            catch (Exception)
            {
                // Menu toggle error handled silently
            }
        }

        // Handler para tap en el overlay (fondo) - oculta el menú
        private async void OnOverlayTapped(object sender, EventArgs e)
        {
            try
            {
                if (_isMenuOpen)
                {
                    await HideOverlayMenu();
                }
            }
            catch (Exception)
            {
                // Overlay tap error handled silently
            }
        }

        // Mostrar el overlay modal con animación
        private async Task ShowOverlayMenu()
        {
            try
            {
                var overlayContainer = this.FindByName<Grid>("OverlayContainer");
                var dropdownMenu = this.FindByName<Border>("DropdownMenu");
                
                if (overlayContainer == null || dropdownMenu == null) return;

                overlayContainer.IsVisible = true;
                overlayContainer.Opacity = 0;
                
                dropdownMenu.Opacity = 0;
                dropdownMenu.TranslationY = -100;
                dropdownMenu.Scale = 0.9;

                var overlayFadeIn = overlayContainer.FadeTo(1, 200, Easing.CubicOut);
                
                var menuFadeIn = dropdownMenu.FadeTo(1, 300, Easing.CubicOut);
                var menuSlideIn = dropdownMenu.TranslateTo(0, 0, 300, Easing.CubicOut);
                var menuScaleIn = dropdownMenu.ScaleTo(1, 300, Easing.CubicOut);

                await Task.WhenAll(overlayFadeIn);
                await Task.WhenAll(menuFadeIn, menuSlideIn, menuScaleIn);

                _isMenuOpen = true;

                StartAutoHideTimer();
            }
            catch (Exception)
            {
                // Show menu error handled silently
            }
        }

        // Ocultar el overlay modal con animación
        private async Task HideOverlayMenu()
        {
            try
            {
                var overlayContainer = this.FindByName<Grid>("OverlayContainer");
                var dropdownMenu = this.FindByName<Border>("DropdownMenu");
                
                if (overlayContainer == null || !overlayContainer.IsVisible || dropdownMenu == null) return;

                StopAutoHideTimer();

                var menuFadeOut = dropdownMenu.FadeTo(0, 200, Easing.CubicIn);
                var menuSlideOut = dropdownMenu.TranslateTo(0, -50, 200, Easing.CubicIn);
                var menuScaleOut = dropdownMenu.ScaleTo(0.9, 200, Easing.CubicIn);

                await Task.WhenAll(menuFadeOut, menuSlideOut, menuScaleOut);

                await overlayContainer.FadeTo(0, 150, Easing.CubicIn);

                overlayContainer.IsVisible = false;
                _isMenuOpen = false;
            }
            catch (Exception)
            {
                // Hide menu error handled silently
            }
        }

        // Iniciar el timer para auto-ocultar (tiempo extendido para overlay)
        private void StartAutoHideTimer()
        {
            try
            {
                _autoHideTimer?.Stop();
                _autoHideTimer?.Start();
            }
            catch (Exception)
            {
                // Timer start error handled silently
            }
        }

        // Parar el timer
        private void StopAutoHideTimer()
        {
            try
            {
                _autoHideTimer?.Stop();
            }
            catch (Exception)
            {
                // Timer stop error handled silently
            }
        }

        // Handler para el timer de auto-ocultar
        private async void OnAutoHideTimer(object? sender, ElapsedEventArgs e)
        {
            try
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    if (_isMenuOpen)
                    {
                        await HideOverlayMenu();
                    }
                });
            }
            catch (Exception)
            {
                // Auto-hide timer error handled silently
            }
        }

        // Handler para notificaciones - oculta el menú y navega
        private async void OnNotificationsTapped(object sender, EventArgs e)
        {
            try
            {
                if (_isMenuOpen)
                {
                    await HideOverlayMenu();
                }

                await Task.Delay(100);

                try
                {
                    if (Shell.Current != null)
                    {
                        await Shell.Current.GoToAsync("notifications");
                        return;
                    }
                }
                catch (Exception)
                {
                    // Shell navigation failed, try fallback
                }

                try
                {
                    var page = new NotificacionesApp();
                    if (Application.Current?.MainPage is Shell shell && shell.CurrentPage != null)
                    {
                        await shell.CurrentPage.Navigation.PushAsync(page);
                    }
                    else if (Application.Current?.MainPage != null)
                    {
                        await Application.Current.MainPage.Navigation.PushAsync(page);
                    }
                }
                catch (Exception)
                {
                    // Push navigation failed
                }
            }
            catch (Exception)
            {
                // Navigation error handled silently
            }
        }

        // Handler para cerrar sesión - oculta el menú y muestra confirmación
        private async void OnLogoutTapped(object sender, EventArgs e)
        {
            try
            {
                if (_isMenuOpen)
                {
                    await HideOverlayMenu();
                }

                await Task.Delay(200);

                bool confirmLogout = await Application.Current.MainPage.DisplayAlert(
                    "Cerrar Sesión",
                    "¿Estás seguro que deseas cerrar sesión?",
                    "Sí, cerrar sesión",
                    "Cancelar");

                if (confirmLogout)
                {
                    await PerformLogout();
                }
            }
            catch (Exception)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Ocurrió un error al cerrar sesión.", "Aceptar");
            }
        }

        // Realiza el proceso completo de logout
        private async Task PerformLogout()
        {
            try
            {
                ClearPreferencesData();

                await ClearSecureStorageData();

                try
                {
                    AuthEvents.NotifyUserLoggedOut();
                }
                catch (Exception)
                {
                    // Logout notification error handled silently
                }

                await Shell.Current.GoToAsync("///LoginPage");
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Limpia todos los datos de Preferences
        private void ClearPreferencesData()
        {
            try
            {
                string[] preferencesKeys = {
                    "AuthToken",
                    "RefreshToken", 
                    "user_data",
                    "UserRole",
                    "password_reset_data",
                    "IsLoggedIn"
                };

                foreach (string key in preferencesKeys)
                {
                    if (Preferences.ContainsKey(key))
                    {
                        Preferences.Remove(key);
                    }
                }

                Preferences.Clear();
            }
            catch (Exception)
            {
                // Preferences clear error handled silently
            }
        }

        // Limpia todos los datos de SecureStorage
        private async Task ClearSecureStorageData()
        {
            try
            {
                string[] secureStorageKeys = {
                    "user_data",
                    "AuthToken",
                    "RefreshToken"
                };

                foreach (string key in secureStorageKeys)
                {
                    try
                    {
                        SecureStorage.Remove(key);
                    }
                    catch (Exception)
                    {
                        // Individual key removal error handled silently
                    }
                }

                SecureStorage.RemoveAll();
            }
            catch (Exception)
            {
                // SecureStorage clear error handled silently
            }
        }

        // Dispose para limpiar el timer
        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();
            
            if (Handler == null)
            {
                _autoHideTimer?.Dispose();
                _autoHideTimer = null;
            }
        }
    }
}
