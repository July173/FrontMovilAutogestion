using AutogestionSenaMaui.ViewModels;
using AutogestionSena.MAUI.Api.Services;
using Microsoft.Maui.Controls;

namespace AutogestionSenaMaui.Views
{
    /// <summary>
    /// HomePage - Página principal después del login
    /// Equivalente a Home.tsx en React
    /// 
    /// Funcionalidad:
    /// 1. Lee datos del usuario desde Preferences
    /// 2. Determina el dashboard apropiado según el rol
    /// 3. Carga dinámicamente el dashboard en el contenedor
    /// 4. Muestra MainLayout (TopBar + Dashboard + Footer)
    /// 
    /// Mapeo de roles (igual que React):
    /// 1 = Admin → AdminDashboardPage
    /// 2 = Aprendiz → ApprenticeDashboardPage
    /// 3 = Instructor → InstructorDashboardPage
    /// 4 = Coordinador → CoordinatorDashboardPage
    /// 5 = Operador SofiaPlus → SofiaOperatorDashboardPage
    /// </summary>
    public partial class HomePage : ContentPage
    {
        private readonly MainLayoutViewModel _viewModel;
        private int _userRole = 0;
        private int _apprenticeId = 0;
        private string _userName = "Usuario";

        public HomePage()
        {
            InitializeComponent();

            _viewModel = new MainLayoutViewModel();
            BindingContext = _viewModel;

            // Suscribirse al mensaje para cerrar el menú lateral
            try
            {
                MessagingCenter.Subscribe<object>(this, "CloseSideMenu", (sender) =>
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await CloseSideMenuAsync();
                    });
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

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Cargar datos del usuario y dashboard
            await LoadUserDataAndDashboard();
            // Suscribir al evento del TopBar si existe para togglear el DashboardLayout
            try
            {
                TopBarView.MenuButtonClicked += OnTopBarMenuClicked;
            }
            catch { }
        }

        /// <summary>
        /// Carga los datos del usuario desde Preferences y el dashboard apropiado
        /// Similar a useEffect in Home.tsx
        /// </summary>
        private async Task LoadUserDataAndDashboard()
        {
            try
            {
                // Leer datos del usuario desde Preferences
                _userRole = Preferences.Get("UserRole", 0);
                var userEmail = Preferences.Get("UserEmail", string.Empty);

                // Si no hay rol, redirigir a login
                if (_userRole == 0)
                {
                    await Shell.Current.GoToAsync("///LoginPage");
                    return;
                }

                // Obtener nombre del usuario desde el email
                _userName = GetUserNameFromEmail(userEmail);

                // Si el rol es Aprendiz, obtener apprentice_id
                if (_userRole == 2)
                {
                    await LoadApprenticeId();
                }

                // Actualizar breadcrumb según el rol
                UpdateBreadcrumbForRole(_userRole);

                // Cargar el dashboard apropiado
                await LoadDashboardForRole(_userRole);

                // Simular carga de notificaciones (opcional)
                await Task.Delay(500);
                _viewModel.UpdateNotificationCount(GetNotificationCountForRole(_userRole));
            }
            catch (Exception)
            {
                await DisplayAlert("Error", "Error al cargar el dashboard. Por favor, inicia sesión nuevamente.", "OK");
                await Shell.Current.GoToAsync("///LoginPage");
            }
        }

        /// <summary>
        /// Obtiene el apprentice_id para usuarios con rol Aprendiz
        /// Actualizado: Consulta primero los datos del usuario para obtener el apprentice.id correcto
        /// </summary>
        private async Task LoadApprenticeId()
        {
            try
            {
                // Obtener UserPerson desde Preferences
                var personId = Preferences.Get("UserPerson", 0);
                var userId = Preferences.Get("UserId", 0);

                if (userId > 0)
                {
                    // Consultar los datos completos del usuario
                    var userService = new UserService();
                    var userDetail = await userService.GetUserByIdAsync(userId);

                    if (userDetail != null)
                    {
                        // Verificar si es un aprendiz y tiene datos de apprentice
                        if (userDetail.Role?.Id == 2 && userDetail.Apprentice != null)
                        {
                            _apprenticeId = userDetail.Apprentice.Id;

                            // Guardar el apprentice ID en Preferences para uso futuro
                            Preferences.Set("ApprenticeId", _apprenticeId);
                        }
                        else if (userDetail.Role?.Id == 3 && userDetail.Instructor != null)
                        {
                            // Para instructores, usar el instructor ID
                            _apprenticeId = userDetail.Instructor.Id;
                        }
                        else
                        {
                            // Fallback: usar person ID
                            _apprenticeId = personId > 0 ? personId : userId;
                        }
                    }
                    else
                    {
                        // Fallback: usar person ID desde Preferences
                        _apprenticeId = personId > 0 ? personId : 0;
                    }
                }
                else
                {
                    // Intentar con los datos JSON guardados (método anterior)
                    await LoadApprenticeIdFromJson();
                }
            }
            catch (Exception)
            {
                // Fallback final
                var personId = Preferences.Get("UserPerson", 0);
                _apprenticeId = personId > 0 ? personId : 0;
            }
        }

        /// <summary>
        /// Método de fallback: Obtener apprentice ID desde los datos JSON guardados (método anterior)
        /// </summary>
        private async Task LoadApprenticeIdFromJson()
        {
            try
            {
                var userDataJson = Preferences.Get("user_data", string.Empty);
                if (!string.IsNullOrEmpty(userDataJson))
                {
                    var userData = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(userDataJson);

                    if (userData.TryGetProperty("person", out var personElement))
                    {
                        var personId = personElement.GetInt32();
                        _apprenticeId = personId;
                    }
                }
            }
            catch (Exception)
            {
                // Error reading JSON data handled silently
            }
        }

        /// <summary>
        /// Carga el dashboard apropiado según el rol del usuario
        /// Similar al switch/case in Home.tsx
        /// </summary>
        private async Task LoadDashboardForRole(int roleId)
        {
            try
            {
                View? dashboardView = null;

                // Crear el dashboard según el rol
                switch (roleId)
                {
                    case 1:
                        dashboardView = CreateAdminDashboard();
                        break;
                    case 2:
                        dashboardView = await CreateApprenticeDashboardAsync(); // ⚠️ Cambio a async
                        break;
                    case 3:
                        dashboardView = CreateInstructorDashboard();
                        break;
                    case 4:
                        dashboardView = CreateCoordinatorDashboard();
                        break;
                    case 5:
                        dashboardView = CreateSofiaOperatorDashboard();
                        break;
                    default:
                        dashboardView = CreateGenericDashboard();
                        break;
                }

                if (dashboardView != null)
                {
                    // Ocultar loading y mostrar dashboard
                    LoadingView.IsVisible = false;
                    LoadingIndicator.IsRunning = false;
                    DashboardScrollView.IsVisible = true;

                    // Inyectar el dashboard en el contenedor
                    DashboardContainer.Content = dashboardView;
                }
            }
            catch (Exception ex)
            {
                // Ocultar loading y mostrar error
                LoadingView.IsVisible = false;
                LoadingIndicator.IsRunning = false;
                DashboardScrollView.IsVisible = true;

                DashboardContainer.Content = CreateErrorView(ex.Message);
            }
        }

        #region Dashboard Creation Methods

        /// <summary>
        /// Crea la vista del dashboard de administrador
        /// </summary>
        private View CreateAdminDashboard()
        {
            var adminPage = new AdminDashboardPage();
            return adminPage.Content;
        }

        /// <summary>
        /// Crea la vista del dashboard de aprendiz
        /// </summary>
        private async Task<View> CreateApprenticeDashboardAsync()
        {
            try
            {
                // Crear y configurar el ViewModel
                var viewModel = new ApprenticeDashboardViewModel();

                if (_apprenticeId > 0)
                {
                    await viewModel.LoadAsync(_apprenticeId);
                }
                else
                {
                    // Intentar obtener UserPerson como fallback
                    var personId = Preferences.Get("UserPerson", 0);
                    if (personId > 0)
                    {
                        await viewModel.LoadAsync(personId);
                    }
                    else
                    {
                        // Cargar con ID de ejemplo para debugging
                        await viewModel.LoadAsync(5);
                    }
                }

                // Crear un ContentView que mantiene su propio BindingContext
                var container = new ContentView
                {
                    BindingContext = viewModel,
                    Content = CreateApprenticeDashboardContent(viewModel)
                };

                return container;
            }
            catch (Exception ex)
            {
                // Retornar una vista de error específica para el dashboard del aprendiz
                return CreateApprenticeDashboardErrorView(ex.Message);
            }
        }

        /// <summary>
        /// Crea el contenido visual del dashboard del aprendiz
        /// </summary>
        private View CreateApprenticeDashboardContent(ApprenticeDashboardViewModel vm)
        {
            // Crear la página y extraer su contenido visual
            var apprenticePage = new ApprenticeDashboardPage();
            apprenticePage.BindingContext = vm;
            
            // Obtener el Grid principal (MainContainer)
            if (apprenticePage.Content is Grid mainGrid)
            {
                // Importante: Establecer el BindingContext en el Grid también
                mainGrid.BindingContext = vm;
                return mainGrid;
            }

            // Fallback: retornar el contenido tal cual
            var content = apprenticePage.Content;
            if (content != null)
            {
                content.BindingContext = vm;
            }
            return content ?? new Label { Text = "Error cargando dashboard" };
        }

        /// <summary>
        /// Crea la vista del dashboard de instructor
        /// </summary>
        private View CreateInstructorDashboard()
        {
            var instructorPage = new InstructorDashboardPage();
            return instructorPage.Content;
        }

        /// <summary>
        /// Crea la vista del dashboard de coordinador
        /// Similar al AdminDashboard según Home.tsx
        /// </summary>
        private View CreateCoordinatorDashboard()
        {
            return CreateGenericDashboard();
        }

        /// <summary>
        /// Crea la vista del dashboard de operador SofiaPlus
        /// </summary>
        private View CreateSofiaOperatorDashboard()
        {
            var sofiaOperatorPage = new SofiaOperatorDashboardPage();
            return sofiaOperatorPage.Content;
        }

        /// <summary>
        /// Crea una vista genérica para roles no reconocidos
        /// Similar a GenericDashboardView in React
        /// </summary>
        private View CreateGenericDashboard()
        {
            return new ScrollView
            {
                Content = new VerticalStackLayout
                {
                    Padding = 40,
                    Spacing = 24,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Frame
                        {
                            BackgroundColor = Color.FromArgb("#F5F5F5"),
                            CornerRadius = 12,
                            HasShadow = false,
                            Padding = 24,
                            Content = new VerticalStackLayout
                            {
                                Spacing = 12,
                                Children =
                                {
                                    new Label
                                    {
                                        Text = "¡ Bienvenido !",
                                        FontSize = 28,
                                        FontAttributes = FontAttributes.Bold,
                                        TextColor = Color.FromArgb("#39A900"),
                                        HorizontalOptions = LayoutOptions.Center
                                    },
                                    new Label
                                    {
                                        Text = _userName,
                                        FontSize = 20,
                                        TextColor = Color.FromArgb("#333333"),
                                        HorizontalOptions = LayoutOptions.Center
                                    }
                                }
                            }
                        },
                        new Frame
                        {
                            BackgroundColor = Colors.White,
                            CornerRadius = 8,
                            HasShadow = true,
                            Padding = 24,
                            Content = new VerticalStackLayout
                            {
                                Spacing = 8,
                                Children =
                                {
                                    new Label
                                    {
                                        Text = "No tienes una vista personalizada asignada",
                                        FontSize = 16,
                                        TextColor = Color.FromArgb("#666666"),
                                        HorizontalOptions = LayoutOptions.Center
                                    },
                                    new Label
                                    {
                                        Text = "Por favor contacta al administrador si necesitas acceso a funcionalidades especiales.",
                                        FontSize = 14,
                                        TextColor = Color.FromArgb("#999999"),
                                        HorizontalOptions = LayoutOptions.Center,
                                        HorizontalTextAlignment = TextAlignment.Center
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Crea una vista de error
        /// </summary>
        private View CreateErrorView(string errorMessage)
        {
            return new VerticalStackLayout
            {
                Padding = 40,
                Spacing = 16,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    new Label
                    {
                        Text = "⚠️ Error al cargar el dashboard",
                        FontSize = 20,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.FromArgb("#DC395F"),
                        HorizontalOptions = LayoutOptions.Center
                    },
                    new Label
                    {
                        Text = errorMessage,
                        FontSize = 14,
                        TextColor = Color.FromArgb("#666666"),
                        HorizontalOptions = LayoutOptions.Center,
                        HorizontalTextAlignment = TextAlignment.Center
                    },
                    new Button
                    {
                        Text = "Reintentar",
                        BackgroundColor = Color.FromArgb("#39A900"),
                        TextColor = Colors.White,
                        CornerRadius = 8,
                        Padding = new Thickness(24, 12),
                        Command = new Command(async () => await LoadUserDataAndDashboard())
                    }
                }
            };
        }

        /// <summary>
        /// Crea una vista de error específica para el dashboard del aprendiz
        /// </summary>
        private View CreateApprenticeDashboardErrorView(string errorMessage)
        {
            return new ScrollView
            {
                Content = new VerticalStackLayout
                {
                    Padding = 20,
                    Spacing = 16,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
        {
  new Label
  {
      Text = "⚠️ Error en Dashboard del Aprendiz",
  FontSize = 18,
      FontAttributes = FontAttributes.Bold,
       TextColor = Color.FromArgb("#DC395F"),
    HorizontalOptions = LayoutOptions.Center
     },
 new Label
     {
          Text = errorMessage,
  FontSize = 14,
         TextColor = Color.FromArgb("#666666"),
       HorizontalOptions = LayoutOptions.Center,
   HorizontalTextAlignment = TextAlignment.Center
 },
            new Button
       {
        Text = "🔍 Diagnosticar Conexión",
    BackgroundColor = Color.FromArgb("#F0AD4E"),
  TextColor = Colors.White,
   CornerRadius = 8,
    Padding = new Thickness(24, 12),
       Command = new Command(async () => await DiagnoseConnectionForApprenticeDashboard())
        },
       new Button
     {
      Text = "Reintentar Dashboard",
     BackgroundColor = Color.FromArgb("#39A900"),
     TextColor = Colors.White,
  CornerRadius = 8,
    Padding = new Thickness(24, 12),
    Command = new Command(async () => await LoadDashboardForRole(2))
         }
       }
                }
            };
        }

        /// <summary>
        /// Método de diagnóstico específico para el dashboard del aprendiz
        /// </summary>
        private async Task DiagnoseConnectionForApprenticeDashboard()
        {
            try
            {
                var apiService = new ApiService();
                var (isConnected, message) = await apiService.DiagnoseConnectionAsync();

                var diagnosticMsg = isConnected ? $"✅ Conexión OK: {message}" : $"❌ Error: {message}";

                await DisplayAlert("Diagnóstico de Conexión", diagnosticMsg, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error de Diagnóstico", $"Error al diagnosticar: {ex.Message}", "OK");
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Extrae el nombre del usuario desde el email
        /// Similar a getUserName en Home.tsx
        /// </summary>
        private string GetUserNameFromEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return "Usuario";

            try
            {
                var emailPart = email.Split('@')[0];
                var nameParts = emailPart.Split('.');

                if (nameParts.Length >= 2)
                {
                    // Capitalizar primera letra de cada parte
                    var firstName = char.ToUpper(nameParts[0][0]) + nameParts[0].Substring(1).ToLower();
                    var lastName = char.ToUpper(nameParts[1][0]) + nameParts[1].Substring(1).ToLower();
                    return $"{firstName} {lastName}";
                }
                else if (nameParts.Length == 1)
                {
                    return char.ToUpper(nameParts[0][0]) + nameParts[0].Substring(1).ToLower();
                }
            }
            catch (Exception)
            {
                // Error extracting name handled silently
            }

            return "Usuario";
        }

        /// <summary>
        /// Actualiza el breadcrumb según el rol
        /// </summary>
        private void UpdateBreadcrumbForRole(int roleId)
        {
            var moduleName = roleId switch
            {
                1 => "Administración",
                2 => "Aprendiz",
                3 => "Instructor",
                4 => "Coordinación",
                5 => "Operador SofiaPlus",
                _ => "General"
            };

            _viewModel.UpdateBreadcrumb(moduleName, "Dashboard");
        }

        /// <summary>
        /// Obtiene el número de notificaciones según el rol (simulado)
        /// En producción, esto debería venir de un servicio
        /// </summary>
        private int GetNotificationCountForRole(int roleId)
        {
            // TODO: Implementar servicio real de notificaciones
            return roleId switch
            {
                1 => 5,  // Admin suele tener más notificaciones
                2 => 2,  // Aprendiz
                3 => 3,  // Instructor
                4 => 4,  // Coordinador
                5 => 1,  // Operador
                _ => 0
            };
        }

        #endregion

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            try
            {
                TopBarView.MenuButtonClicked -= OnTopBarMenuClicked;
            }
            catch { }
        }

        private bool _isMenuOpen = false;

        private async void OnTopBarMenuClicked(object? sender, EventArgs e)
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
                await Task.CompletedTask;
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
                await Task.CompletedTask;
            }
            catch (Exception)
            {
                // Close menu error handled silently
            }
        }
    }
}
