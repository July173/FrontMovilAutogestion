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

            if (instructorId <= 0)
            {
                System.Diagnostics.Debug.WriteLine("[InstructorDashboard] No se encontró ID del instructor");
                ShowEmptyState();
                return;
            }

            System.Diagnostics.Debug.WriteLine($"[InstructorDashboard] Cargando dashboard para instructor ID: {instructorId}");

            var dashboardData = await _instructorService.GetDashboardDataSafeAsync(instructorId);

            // Actualizar estadísticas
            UpdateStats(dashboardData.Stats);

            // Actualizar próximas visitas
            UpdateProximasVisitas(dashboardData.ProximasVisitas);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[InstructorDashboard] Error al cargar dashboard: {ex.Message}");
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

    // Crear tarjeta de visita programada
    private View CreateVisitaCard(ProximaVisitaDto visita)
    {
        var card = new Frame
        {
            BackgroundColor = Colors.White,
            CornerRadius = 16,
            Padding = new Thickness(24),
            HasShadow = true,
            WidthRequest = 380,
            MinimumHeightRequest = 320,
            Margin = new Thickness(0, 8, 0, 8)
        };

        var mainStack = new VerticalStackLayout { Spacing = 14 };

        // Header con icono y tipo de visita usando Grid para mejor espaciado
        var headerGrid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
            },
            ColumnSpacing = 12
        };

        var iconFrame = new Frame
        {
            BackgroundColor = Color.FromArgb("#C8F7DC"),
            CornerRadius = 20,
            HeightRequest = 40,
            WidthRequest = 40,
            Padding = 0,
            HasShadow = false,
            VerticalOptions = LayoutOptions.Center,
            Content = new Label
            {
                Text = "\uf274",
                FontFamily = "FA_Solid",
                FontSize = 18,
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
            FontSize = 20,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#222"),
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start
        };
        Grid.SetColumn(titleLabel, 1);
        headerGrid.Children.Add(titleLabel);

        mainStack.Children.Add(headerGrid);

        // Descripción
        mainStack.Children.Add(new Label
        {
            Text = "Tienes una visita programada",
            FontSize = 15,
            TextColor = Color.FromArgb("#222"),
            Margin = new Thickness(0, 0, 0, 2)
        });

        // Aprendiz con identificación
        var aprendizStack = new HorizontalStackLayout { Spacing = 10, VerticalOptions = LayoutOptions.Center };
        aprendizStack.Children.Add(new Label
        {
            Text = "\uf007",
            FontFamily = "FA_Solid",
            FontSize = 14,
            TextColor = Color.FromArgb("#2E7D32"),
            VerticalOptions = LayoutOptions.Center
        });
        aprendizStack.Children.Add(new Label
        {
            Text = $"{visita.AprendizNombre ?? "N/A"} : {visita.AprendizIdentificacion}",
            FontSize = 15,
            TextColor = Color.FromArgb("#222"),
            VerticalOptions = LayoutOptions.Center
        });
        mainStack.Children.Add(aprendizStack);

        // Número de ficha
        var fichaStack = new HorizontalStackLayout { Spacing = 10, VerticalOptions = LayoutOptions.Center };
        fichaStack.Children.Add(new Label
        {
            Text = "\uf02d",
            FontFamily = "FA_Solid",
            FontSize = 14,
            TextColor = Color.FromArgb("#2E7D32"),
            VerticalOptions = LayoutOptions.Center
        });
        fichaStack.Children.Add(new Label
        {
            Text = $"Ficha: {visita.NumeroFicha}",
            FontSize = 15,
            TextColor = Color.FromArgb("#222"),
            VerticalOptions = LayoutOptions.Center
        });
        mainStack.Children.Add(fichaStack);

        // Programa de formación
        mainStack.Children.Add(new Label
        {
            Text = "Programa de formación:",
            FontSize = 15,
            TextColor = Color.FromArgb("#222"),
            Margin = new Thickness(0, 6, 0, 0)
        });
        mainStack.Children.Add(new Label
        {
            Text = visita.Programa ?? "N/A",
            FontSize = 15,
            TextColor = Color.FromArgb("#222"),
            FontAttributes = FontAttributes.Bold,
            Margin = new Thickness(0, 0, 0, 4)
        });

        // Fecha (usando fecha_texto del API)
        var fechaStack = new HorizontalStackLayout { Spacing = 10, VerticalOptions = LayoutOptions.Center };
        fechaStack.Children.Add(new Label
        {
            Text = "\uf017",
            FontFamily = "FA_Solid",
            FontSize = 14,
            TextColor = Color.FromArgb("#2E7D32"),
            VerticalOptions = LayoutOptions.Center
        });
        fechaStack.Children.Add(new Label
        {
            Text = visita.FechaTexto ?? FormatFechaVisita(visita.FechaProgramada),
            FontSize = 15,
            TextColor = Color.FromArgb("#222"),
            VerticalOptions = LayoutOptions.Center
        });
        mainStack.Children.Add(fechaStack);

        // Botón ver detalles
        var btnFrame = new Frame
        {
            BackgroundColor = Colors.White,
            BorderColor = Color.FromArgb("#C8F7DC"),
            CornerRadius = 8,
            Padding = 0,
            Margin = new Thickness(0, 12, 0, 0),
            HasShadow = false,
            Content = new Label
            {
                Text = "Ver detalles",
                FontSize = 16,
                TextColor = Color.FromArgb("#2E7D32"),
                HorizontalTextAlignment = TextAlignment.Center,
                Padding = new Thickness(0, 12)
            }
        };

        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += async (s, e) => await OnVerDetallesClicked(visita);
        btnFrame.GestureRecognizers.Add(tapGesture);

        mainStack.Children.Add(btnFrame);

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
            var userDataJson = Preferences.Get("user_data", string.Empty);

            if (!string.IsNullOrEmpty(userDataJson))
            {
                var userData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(userDataJson);

                // Buscar el ID del instructor
                if (userData != null)
                {
                    // Primero intentar con instructor_id
                    if (userData.TryGetValue("instructor_id", out var instructorIdObj))
                    {
                        if (instructorIdObj is System.Text.Json.JsonElement jsonElement)
                        {
                            if (jsonElement.TryGetInt32(out int id))
                                return id;
                        }
                        else if (int.TryParse(instructorIdObj?.ToString(), out int id))
                        {
                            return id;
                        }
                    }

                    // Luego intentar con id general
                    if (userData.TryGetValue("id", out var idObj))
                    {
                        if (idObj is System.Text.Json.JsonElement jsonElement)
                        {
                            if (jsonElement.TryGetInt32(out int id))
                                return id;
                        }
                        else if (int.TryParse(idObj?.ToString(), out int id))
                        {
                            return id;
                        }
                    }
                }
            }

            return 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[InstructorDashboard] Error obteniendo instructor ID: {ex.Message}");
            return 0;
        }
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
