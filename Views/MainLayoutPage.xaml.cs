using AutogestionSenaMaui.ViewModels;
using Microsoft.Maui.Controls;

namespace AutogestionSenaMaui.Views
{
    /// <summary>
    /// MainLayoutPage - Página de layout principal de la aplicación
    /// Similar a MainLayout.tsx en React, proporciona estructura común:
    /// - TopBar con navegación breadcrumb y notificaciones
    /// - Área de contenido dinámico
    /// - Footer con información del sistema
    /// </summary>
    public partial class MainLayoutPage : ContentPage
    {
        private readonly MainLayoutViewModel _viewModel;

        public MainLayoutPage()
        {
            InitializeComponent();
            _viewModel = new MainLayoutViewModel();
            BindingContext = _viewModel;
            
            // Suscribirse al mensaje para cerrar el menú lateral
            try
            {
                MessagingCenter.Subscribe<object>(this, "CloseSideMenu", (sender) =>
                {
                    MainThread.BeginInvokeOnMainThread(async () => await CloseSideMenuAsync());
                });
            }
            catch (Exception)
            {
                // Subscription error handled silently
            }
            
            // Desuscribirse al descargar la página
            this.Unloaded += (s, e) =>
            {
                try
                {
                    MessagingCenter.Unsubscribe<object>(this, "CloseSideMenu");
                }
                catch { }
            };
        }

        /// <summary>
        /// Establece el contenido principal de la página
        /// </summary>
        /// <param name="content">View a mostrar en el área de contenido</param>
        public void SetMainContent(View content)
        {
            MainContent.Content = content;
        }

        /// <summary>
        /// Actualiza el breadcrumb de navegación
        /// Similar a handleMenuItemClick en MainLayout.tsx
        /// </summary>
        /// <param name="moduleName">Nombre del módulo activo</param>
        /// <param name="formName">Nombre del formulario activo</param>
        public void UpdateBreadcrumb(string moduleName, string formName)
        {
            _viewModel.ActiveModule = moduleName;
            _viewModel.ActiveFormName = formName;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                TopBarView.MenuButtonClicked += TopBarView_MenuButtonClicked;
            }
            catch (Exception)
            {
                // Subscription error handled silently
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            try
            {
                TopBarView.MenuButtonClicked -= TopBarView_MenuButtonClicked;
            }
            catch { }
        }

        private bool _isMenuOpen = false;

        private async void TopBarView_MenuButtonClicked(object? sender, EventArgs e)
        {
            await ToggleSideMenuAsync();
        }

        private async Task ToggleSideMenuAsync()
        {
            try
            {
                if (_isMenuOpen)
                {
                    await CloseSideMenuAsync();
                }
                else
                {
                    await OpenSideMenuAsync();
                }
            }
            catch (Exception)
            {
                // Toggle menu error handled silently
            }
        }

        private async Task OpenSideMenuAsync()
        {
            try
            {
                _isMenuOpen = true;
                
                // Cancelar cualquier animación anterior
                this.AbortAnimation("OpenMenu");
                this.AbortAnimation("CloseMenu");
                
                // Resetear WidthRequest a 0 antes de abrir
                SideMenu.WidthRequest = 0;
                SideMenu.IsVisible = true;
                
                // Esperar un frame para que el layout se actualice
                await Task.Delay(16);
                
                // Calculate desired width (100% of screen or parent)
                var desiredWidth = 0.0;
                if (this.Width > 0)
                {
                    desiredWidth = this.Width;
                }
                else
                {
                    var mainDisplay = Microsoft.Maui.Devices.DeviceDisplay.MainDisplayInfo;
                    var screenDpWidth = mainDisplay.Width / mainDisplay.Density;
                    desiredWidth = screenDpWidth;
                }
                
                // Animate width from 0 to desiredWidth
                var animation = new Animation(v => SideMenu.WidthRequest = v, 0, desiredWidth);
                animation.Commit(this, "OpenMenu", 16, 250, Easing.CubicOut);
                
                await Task.Delay(250);
            }
            catch (Exception)
            {
                // Open menu error handled silently
            }
        }

        private async Task CloseSideMenuAsync()
        {
            try
            {
                _isMenuOpen = false;
                
                // Cancelar cualquier animación anterior
                this.AbortAnimation("OpenMenu");
                this.AbortAnimation("CloseMenu");
                
                // Animate width from current to 0
                var currentWidth = SideMenu.WidthRequest > 0 ? SideMenu.WidthRequest : this.Width;
                var animation = new Animation(v => SideMenu.WidthRequest = v, currentWidth, 0);
                animation.Commit(this, "CloseMenu", 16, 250, Easing.CubicIn);
                
                await Task.Delay(250);
                
                // Resetear completamente el menú
                SideMenu.WidthRequest = 0;
                SideMenu.IsVisible = false;
            }
            catch (Exception)
            {
                // Asegurar que el menú esté cerrado incluso si hay error
                SideMenu.WidthRequest = 0;
                SideMenu.IsVisible = false;
            }
        }

        private T? FindChildOfType<T>(Microsoft.Maui.IView root) where T : Microsoft.Maui.IView
        {
            if (root is T t) return t;

            if (root is Microsoft.Maui.Controls.Layout layout)
            {
                foreach (var child in layout.Children)
                {
                    var found = FindChildOfType<T>(child);
                    if (found != null) return found;
                }
            }
            else if (root is Microsoft.Maui.Controls.ContentView cv && cv.Content is Microsoft.Maui.IView inner)
            {
                return FindChildOfType<T>(inner);
            }
            else if (root is Microsoft.Maui.Controls.ScrollView sv && sv.Content is Microsoft.Maui.IView sc)
            {
                return FindChildOfType<T>(sc);
            }

            return default;
        }
    }
}
