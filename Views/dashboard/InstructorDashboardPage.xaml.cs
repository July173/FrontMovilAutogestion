using Microsoft.Maui.Controls;
using AutogestionSenaMaui.ContentViews;
using AutogestionSenaMaui.Helpers;
using AutogestionSena.MAUI.Api.Services;
using AutogestionSena.MAUI.Api.Dtos;

namespace AutogestionSenaMaui.Views;

public partial class InstructorDashboardPage : ContentPage
{
    private readonly InstructorService _instructorService;

    public InstructorDashboardPage()
    {
        InitializeComponent();
        _instructorService = new InstructorService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        MainLayoutHelper.UpdateCurrentBreadcrumb("Dashboard", "Instructor");
        await LoadDashboardDataAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }

    // Override para aplicar diseño responsive
    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        ApplyResponsiveLayout(width);
    }

    // Cargar datos del dashboard desde el API
    private async Task LoadDashboardDataAsync()
    {
        try
        {
            ShowLoading(true);

            // Obtener el ID del instructor desde el almacenamiento local
            int instructorId = GetInstructorIdFromStorage();

            // Si no se encontró en storage, consultar el API
            if (instructorId <= 0)
            {
                System.Diagnostics.Debug.WriteLine("[InstructorDashboard] No se encontró ID en storage, consultando API...");
                instructorId = await GetInstructorIdFromApiAsync();
            }

            if (instructorId <= 0)
            {
                System.Diagnostics.Debug.WriteLine("[InstructorDashboard] No se pudo obtener ID del instructor");
                await DisplayAlert("Error", "No se pudo obtener la información del instructor. Por favor, inicia sesión nuevamente.", "OK");
                ShowEmptyState();
                return;
            }

            System.Diagnostics.Debug.WriteLine($"[InstructorDashboard] Cargando dashboard para instructor ID: {instructorId}");

            var dashboardData = await _instructorService.GetDashboardDataSafeAsync(instructorId);

            System.Diagnostics.Debug.WriteLine($"[InstructorDashboard] Datos recibidos - Stats: {dashboardData.Stats != null}, Visitas: {dashboardData.ProximasVisitas?.Count ?? 0}");

            // Actualizar estadísticas
            UpdateStats(dashboardData.Stats);

            // Actualizar próximas visitas
            UpdateProximasVisitas(dashboardData.ProximasVisitas);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[InstructorDashboard] Error al cargar dashboard: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"[InstructorDashboard] Stack trace: {ex.StackTrace}");
            ShowEmptyState();
        }
        finally
        {
            ShowLoading(false);
        }
    }

    // Actualizar las estadísticas en la UI
    private void UpdateStats(InstructorStatsDto? stats)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (stats != null)
            {
                VisitasProgramadasLabel.Text = stats.VisitasProgramadas.ToString();
                AprendicesAsignadosLabel.Text = stats.AprendicesAsignados.ToString();
                AprendicesEvaluadosLabel.Text = stats.AprendicesEvaluados.ToString();
            }
            else
            {
                VisitasProgramadasLabel.Text = "0";
                AprendicesAsignadosLabel.Text = "0";
                AprendicesEvaluadosLabel.Text = "0";
            }
        });
    }

    // Actualizar las próximas visitas en la UI
    private void UpdateProximasVisitas(List<ProximaVisitaDto>? visitas)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            VisitasContainer.Children.Clear();

            if (visitas == null || visitas.Count == 0)
            {
                NoVisitasFrame.IsVisible = true;
                return;
            }

            NoVisitasFrame.IsVisible = false;

            foreach (var visita in visitas)
            {
                var card = CreateVisitaCard(visita);
                VisitasContainer.Children.Add(card);
            }
        });
    }

    // Crear tarjeta de visita programada - Optimizada para móvil
    private View CreateVisitaCard(ProximaVisitaDto visita)
    {
        var card = new Frame
        {
            BackgroundColor = Colors.White,
            CornerRadius = 12,
            Padding = new Thickness(16),
            HasShadow = true,
            HorizontalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 4, 0, 4)
        };

        var mainStack = new VerticalStackLayout { Spacing = 10 };

        // Header con icono y tipo de visita
        var headerGrid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = GridLength.Auto }
            },
            ColumnSpacing = 10
        };

        var iconFrame = new Frame
        {
            BackgroundColor = Color.FromArgb("#C8F7DC"),
            CornerRadius = 16,
            HeightRequest = 32,
            WidthRequest = 32,
            Padding = 0,
            HasShadow = false,
            VerticalOptions = LayoutOptions.Center,
            Content = new Label
            {
                Text = "\uf274",
                FontFamily = "FA_Solid",
                FontSize = 14,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                TextColor = Color.FromArgb("#2E7D32")
            }
        };
        Grid.SetColumn(iconFrame, 0);
        headerGrid.Children.Add(iconFrame);

        var titleLabel = new Label
        {
            Text = visita.TipoVisita ?? "Visita programada",
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#222"),
            VerticalOptions = LayoutOptions.Center,
            LineBreakMode = LineBreakMode.TailTruncation
        };
        Grid.SetColumn(titleLabel, 1);
        headerGrid.Children.Add(titleLabel);

        // Badge de fecha
        var fechaBadge = new Frame
        {
            BackgroundColor = Color.FromArgb("#E8F5E9"),
            CornerRadius = 8,
            Padding = new Thickness(8, 4),
            HasShadow = false,
            VerticalOptions = LayoutOptions.Center,
            Content = new Label
            {
                Text = visita.FechaTexto ?? FormatFechaVisita(visita.FechaProgramada),
                FontSize = 11,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#2E7D32")
            }
        };
        Grid.SetColumn(fechaBadge, 2);
        headerGrid.Children.Add(fechaBadge);

        mainStack.Children.Add(headerGrid);

        // Línea separadora
        mainStack.Children.Add(new BoxView
        {
            HeightRequest = 1,
            BackgroundColor = Color.FromArgb("#E0E0E0"),
            Margin = new Thickness(0, 4)
        });

        // Info del aprendiz - Layout compacto
        var infoGrid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
            },
            RowSpacing = 8,
            ColumnSpacing = 12
        };

        // Aprendiz
        var aprendizStack = new VerticalStackLayout { Spacing = 2 };
        aprendizStack.Children.Add(new Label
        {
            Text = "Aprendiz",
            FontSize = 11,
            TextColor = Color.FromArgb("#888")
        });
        aprendizStack.Children.Add(new Label
        {
            Text = visita.AprendizNombre ?? "N/A",
            FontSize = 13,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#333"),
            LineBreakMode = LineBreakMode.TailTruncation
        });
        Grid.SetRow(aprendizStack, 0);
        Grid.SetColumn(aprendizStack, 0);
        infoGrid.Children.Add(aprendizStack);

        // Identificación
        var idStack = new VerticalStackLayout { Spacing = 2 };
        idStack.Children.Add(new Label
        {
            Text = "Identificación",
            FontSize = 11,
            TextColor = Color.FromArgb("#888")
        });
        idStack.Children.Add(new Label
        {
            Text = visita.AprendizIdentificacion.ToString(),
            FontSize = 13,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#333")
        });
        Grid.SetRow(idStack, 0);
        Grid.SetColumn(idStack, 1);
        infoGrid.Children.Add(idStack);

        // Ficha
        var fichaStack = new VerticalStackLayout { Spacing = 2 };
        fichaStack.Children.Add(new Label
        {
            Text = "Ficha",
            FontSize = 11,
            TextColor = Color.FromArgb("#888")
        });
        fichaStack.Children.Add(new Label
        {
            Text = visita.NumeroFicha.ToString(),
            FontSize = 13,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#333")
        });
        Grid.SetRow(fichaStack, 1);
        Grid.SetColumn(fichaStack, 0);
        infoGrid.Children.Add(fichaStack);

        // Fecha programada
        var fechaStack = new VerticalStackLayout { Spacing = 2 };
        fechaStack.Children.Add(new Label
        {
            Text = "Fecha",
            FontSize = 11,
            TextColor = Color.FromArgb("#888")
        });
        fechaStack.Children.Add(new Label
        {
            Text = visita.FechaProgramada ?? "Por definir",
            FontSize = 13,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#333")
        });
        Grid.SetRow(fechaStack, 1);
        Grid.SetColumn(fechaStack, 1);
        infoGrid.Children.Add(fechaStack);

        mainStack.Children.Add(infoGrid);

        // Programa de formación
        var programaStack = new VerticalStackLayout { Spacing = 2, Margin = new Thickness(0, 4, 0, 0) };
        programaStack.Children.Add(new Label
        {
            Text = "Programa",
            FontSize = 11,
            TextColor = Color.FromArgb("#888")
        });
        programaStack.Children.Add(new Label
        {
            Text = visita.Programa ?? "N/A",
            FontSize = 13,
            TextColor = Color.FromArgb("#333"),
            LineBreakMode = LineBreakMode.TailTruncation,
            MaxLines = 2
        });
        mainStack.Children.Add(programaStack);

        // Botón ver detalles
        var btnDetalles = new Button
        {
            Text = "Ver detalles",
            BackgroundColor = Colors.Transparent,
            TextColor = Color.FromArgb("#2E7D32"),
            FontSize = 14,
            FontAttributes = FontAttributes.Bold,
            BorderColor = Color.FromArgb("#2E7D32"),
            BorderWidth = 1,
            CornerRadius = 8,
            HeightRequest = 36,
            Margin = new Thickness(0, 8, 0, 0)
        };
        btnDetalles.Clicked += async (s, e) => await OnVerDetallesClicked(visita);
        mainStack.Children.Add(btnDetalles);

        card.Content = mainStack;
        return card;
    }

    // Formatear fecha de visita
    private string FormatFechaVisita(string? fecha)
    {
        if (string.IsNullOrEmpty(fecha))
            return "Fecha por definir";

        try
        {
            if (DateTime.TryParse(fecha, out var fechaDate))
            {
                var hoy = DateTime.Today;
                var manana = hoy.AddDays(1);

                if (fechaDate.Date == hoy)
                    return "Hoy";
                if (fechaDate.Date == manana)
                    return "Mañana";

                var dias = (fechaDate.Date - hoy).Days;
                if (dias > 0 && dias <= 7)
                    return $"En {dias} días";

                return fechaDate.ToString("dd/MM/yyyy");
            }
        }
        catch
        {
            // Ignore parsing errors
        }

        return fecha;
    }

    // Manejar click en ver detalles
    private async Task OnVerDetallesClicked(ProximaVisitaDto visita)
    {
        // TODO: Navegar a la página de detalles de la visita
        await DisplayAlert(
            visita.TipoVisita ?? "Visita", 
            $"Aprendiz: {visita.AprendizNombre}\n" +
            $"Identificación: {visita.AprendizIdentificacion}\n" +
            $"Programa: {visita.Programa}\n" +
            $"Ficha: {visita.NumeroFicha}\n" +
            $"Fecha: {visita.FechaProgramada}", 
            "OK");
    }

    // Mostrar estado vacío
    private void ShowEmptyState()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            VisitasProgramadasLabel.Text = "0";
            AprendicesAsignadosLabel.Text = "0";
            AprendicesEvaluadosLabel.Text = "0";
            NoVisitasFrame.IsVisible = true;
            VisitasContainer.Children.Clear();
        });
    }

    // Mostrar/ocultar loading
    private void ShowLoading(bool isLoading)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            LoadingIndicator.IsRunning = isLoading;
            LoadingIndicator.IsVisible = isLoading;
        });
    }

    // Obtener ID del instructor desde el almacenamiento local
    private int GetInstructorIdFromStorage()
    {
        try
        {
            // Primero intentar obtener del Preferences directo (si se guardó previamente)
            var instructorId = Preferences.Get("InstructorId", 0);
            if (instructorId > 0)
            {
                System.Diagnostics.Debug.WriteLine($"[InstructorDashboard] Instructor ID desde Preferences: {instructorId}");
                return instructorId;
            }

            // Intentar obtener desde user_data JSON
            var userDataJson = Preferences.Get("user_data", string.Empty);
            System.Diagnostics.Debug.WriteLine($"[InstructorDashboard] user_data JSON: {userDataJson}");

            if (!string.IsNullOrEmpty(userDataJson))
            {
                var userData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(userDataJson);

                if (userData != null)
                {
                    // Intentar con instructor_id
                    if (userData.TryGetValue("instructor_id", out var instructorIdObj))
                    {
                        if (instructorIdObj is System.Text.Json.JsonElement jsonElement)
                        {
                            if (jsonElement.TryGetInt32(out int id))
                            {
                                System.Diagnostics.Debug.WriteLine($"[InstructorDashboard] instructor_id desde JSON: {id}");
                                return id;
                            }
                        }
                        else if (int.TryParse(instructorIdObj?.ToString(), out int id))
                        {
                            return id;
                        }
                    }

                    // Intentar con instructorId (camelCase)
                    if (userData.TryGetValue("instructorId", out var instructorIdCamel))
                    {
                        if (instructorIdCamel is System.Text.Json.JsonElement jsonElement)
                        {
                            if (jsonElement.TryGetInt32(out int id))
                            {
                                System.Diagnostics.Debug.WriteLine($"[InstructorDashboard] instructorId desde JSON: {id}");
                                return id;
                            }
                        }
                    }
                }
            }

            // Fallback: Intentar obtener desde UserId y consultar el API
            // Por ahora usamos el UserId directamente si el rol es instructor
            var userId = Preferences.Get("UserId", 0);
            var userRole = Preferences.Get("UserRole", 0);
            
            System.Diagnostics.Debug.WriteLine($"[InstructorDashboard] UserId: {userId}, UserRole: {userRole}");

            // Si el rol es 3 (instructor), necesitamos obtener el instructor ID real
            // Por ahora retornamos 0 para forzar la consulta async
            if (userRole == 3 && userId > 0)
            {
                System.Diagnostics.Debug.WriteLine($"[InstructorDashboard] Rol instructor detectado, UserId: {userId} - necesita consulta API");
                return 0; // Retornamos 0 para indicar que necesitamos consultar el API
            }

            return 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[InstructorDashboard] Error obteniendo instructor ID: {ex.Message}");
            return 0;
        }
    }

    /// <summary>
    /// Obtiene el ID del instructor consultando el API de usuarios
    /// </summary>
    private async Task<int> GetInstructorIdFromApiAsync()
    {
        try
        {
            var userId = Preferences.Get("UserId", 0);
            if (userId <= 0)
            {
                System.Diagnostics.Debug.WriteLine("[InstructorDashboard] No hay UserId guardado");
                return 0;
            }

            System.Diagnostics.Debug.WriteLine($"[InstructorDashboard] Consultando API para UserId: {userId}");

            var userService = new UserService();
            var userDetail = await userService.GetUserByIdAsync(userId);

            if (userDetail != null && userDetail.Instructor != null)
            {
                var instructorId = userDetail.Instructor.Id;
                System.Diagnostics.Debug.WriteLine($"[InstructorDashboard] Instructor ID obtenido del API: {instructorId}");
                
                // Guardar para uso futuro
                Preferences.Set("InstructorId", instructorId);
                
                return instructorId;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[InstructorDashboard] userDetail.Instructor es null");
            }

            return 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[InstructorDashboard] Error consultando API: {ex.Message}");
            return 0;
        }
    }

    // Manejar click en botón de recargar
    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("[InstructorDashboard] Botón de actualizar presionado");
        
        // Evitar múltiples clics mientras se carga
        if (RefreshSpinner.IsRunning)
        {
            System.Diagnostics.Debug.WriteLine("[InstructorDashboard] Ya está cargando, ignorando clic");
            return;
        }

        await RefreshDashboardWithAnimation();
    }

    // Refrescar dashboard con animación en el botón
    private async Task RefreshDashboardWithAnimation()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("[InstructorDashboard] Iniciando refresh con animación");
            
            // Mostrar estado de carga en el botón
            SetRefreshButtonLoading(true);

            // Cargar los datos
            await LoadDashboardDataAsync();
            
            System.Diagnostics.Debug.WriteLine("[InstructorDashboard] Refresh completado");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[InstructorDashboard] Error en refresh: {ex.Message}");
        }
        finally
        {
            // Restaurar estado normal del botón
            SetRefreshButtonLoading(false);
        }
    }

    // Cambiar estado visual del botón de recargar
    private void SetRefreshButtonLoading(bool isLoading)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            RefreshButton.IsVisible = !isLoading;
            RefreshButton.IsEnabled = !isLoading;
            RefreshLoadingContainer.IsVisible = isLoading;
            RefreshSpinner.IsRunning = isLoading;
        });
    }

    // Aplicar diseño responsive según el ancho de la pantalla
    private void ApplyResponsiveLayout(double width)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"[InstructorDashboard] Aplicando diseño responsive para ancho: {width}px");

            // Ajustar tamaños según el ancho de pantalla
            if (width <= 480)
            {
                // Para móviles, cambiar a layout vertical
                if (StatsContainer != null)
                {
                    // Las tarjetas se adaptarán automáticamente con FlexLayout
                }
            }
        }
        catch (Exception)
        {
            // Responsive layout error handled silently
        }
    }
}
