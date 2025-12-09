using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutogestionSenaMaui.ContentViews;
using AutogestionSenaMaui.Helpers;
using AutogestionSena.MAUI.Api.Dtos;
using AutogestionSena.MAUI.Api.Services;

namespace AutogestionSenaMaui.Views;

public partial class SofiaOperatorDashboardPage : ContentPage
{
    private readonly OperatorDashboardService _dashboardService;
    private List<MonthlyDataDto> _monthlyData = new();

    public SofiaOperatorDashboardPage()
    {
        InitializeComponent();
        _dashboardService = new OperatorDashboardService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        MainLayoutHelper.UpdateCurrentBreadcrumb("Dashboard", "Operador SofiaPlus");
        await LoadDashboardDataAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }

    private async Task LoadDashboardDataAsync()
    {
        try
        {
            // Mostrar indicador de carga
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            ChartLoadingIndicator.IsVisible = true;
            ChartLoadingIndicator.IsRunning = true;
            ErrorBorder.IsVisible = false;

            // Obtener email del usuario desde SecureStorage
            var userEmail = await SecureStorage.GetAsync("user_email");
            UserEmailLabel.Text = !string.IsNullOrEmpty(userEmail) 
                ? $"Hola, {userEmail}" 
                : "Hola, Usuario";

            // Obtener token de autenticación
            var token = await SecureStorage.GetAsync("auth_token");

            System.Diagnostics.Debug.WriteLine($"[OperatorDashboard] Cargando dashboard con token: {(string.IsNullOrEmpty(token) ? "NO TOKEN" : "TOKEN OK")}");

            var response = await _dashboardService.GetOperatorDashboardAsync(token);

            System.Diagnostics.Debug.WriteLine($"[OperatorDashboard] Respuesta: Success={response?.Success}, Data={response?.Data != null}");

            if (response != null && response.Success && response.Data != null)
            {
                // Actualizar año
                YearLabel.Text = response.Data.Year.ToString();

                // Actualizar totales
                if (response.Data.Totals != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[OperatorDashboard] Totales: Registered={response.Data.Totals.Registered}, Pending={response.Data.Totals.Pending}, Total={response.Data.Totals.Total}");
                    
                    RegisteredLabel.Text = response.Data.Totals.Registered.ToString();
                    PendingLabel.Text = response.Data.Totals.Pending.ToString();
                    TotalLabel.Text = response.Data.Totals.Total.ToString();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[OperatorDashboard] Totals es null, mostrando valores de ejemplo");
                    ShowExampleData();
                }

                // Guardar datos mensuales y renderizar gráfico
                _monthlyData = response.Data.MonthlyData ?? new List<MonthlyDataDto>();
                RenderChart();

                NoDataLabel.IsVisible = false;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[OperatorDashboard] Error en respuesta: {response?.Message}");
                ShowError(response?.Message ?? "Error al cargar los datos del dashboard");
                
                // Mostrar datos de ejemplo para que el usuario vea la interfaz
                ShowExampleData();
                NoDataLabel.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            ShowError($"Error: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"[OperatorDashboard] Excepción: {ex}");
            
            // Mostrar datos de ejemplo en caso de error
            ShowExampleData();
        }
        finally
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
            ChartLoadingIndicator.IsVisible = false;
            ChartLoadingIndicator.IsRunning = false;
        }
    }

    /// <summary>
    /// Muestra datos de ejemplo cuando no hay conexión al API
    /// </summary>
    private void ShowExampleData()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            // Mostrar valores de ejemplo
            RegisteredLabel.Text = "125";
            PendingLabel.Text = "34";
            TotalLabel.Text = "159";
            YearLabel.Text = DateTime.Now.Year.ToString();

            // Crear datos de ejemplo para el gráfico
            _monthlyData = new List<MonthlyDataDto>
            {
                new MonthlyDataDto { Month = "Ene", MonthNumber = 1, Registered = 12, Pending = 5 },
                new MonthlyDataDto { Month = "Feb", MonthNumber = 2, Registered = 18, Pending = 3 },
                new MonthlyDataDto { Month = "Mar", MonthNumber = 3, Registered = 15, Pending = 8 },
                new MonthlyDataDto { Month = "Abr", MonthNumber = 4, Registered = 22, Pending = 4 },
                new MonthlyDataDto { Month = "May", MonthNumber = 5, Registered = 10, Pending = 6 },
                new MonthlyDataDto { Month = "Jun", MonthNumber = 6, Registered = 14, Pending = 2 },
                new MonthlyDataDto { Month = "Jul", MonthNumber = 7, Registered = 8, Pending = 3 },
                new MonthlyDataDto { Month = "Ago", MonthNumber = 8, Registered = 11, Pending = 1 },
                new MonthlyDataDto { Month = "Sep", MonthNumber = 9, Registered = 5, Pending = 2 },
                new MonthlyDataDto { Month = "Oct", MonthNumber = 10, Registered = 0, Pending = 0 },
                new MonthlyDataDto { Month = "Nov", MonthNumber = 11, Registered = 0, Pending = 0 },
                new MonthlyDataDto { Month = "Dic", MonthNumber = 12, Registered = 0, Pending = 0 }
            };

            RenderChart();
        });
    }

    private void RenderChart()
    {
        // Limpiar gráfico anterior
        ChartGrid.Children.Clear();

        if (_monthlyData == null || _monthlyData.Count == 0)
        {
            NoDataLabel.IsVisible = true;
            return;
        }

        // Encontrar el valor máximo para escalar las barras
        int maxValue = 1; // Mínimo para evitar división por cero
        foreach (var data in _monthlyData)
        {
            if (data.Registered > maxValue) maxValue = data.Registered;
            if (data.Pending > maxValue) maxValue = data.Pending;
        }

        // Crear barras para cada mes
        for (int i = 0; i < _monthlyData.Count && i < 12; i++)
        {
            var monthData = _monthlyData[i];
            
            // Calcular alturas proporcionales (máximo 180px)
            double registeredHeight = maxValue > 0 ? (monthData.Registered / (double)maxValue) * 180 : 0;
            double pendingHeight = maxValue > 0 ? (monthData.Pending / (double)maxValue) * 180 : 0;

            // Si no hay datos, mostrar una línea mínima
            if (registeredHeight < 2 && pendingHeight < 2)
            {
                registeredHeight = 2;
                pendingHeight = 2;
            }

            // Contenedor para las dos barras del mes
            var barContainer = new HorizontalStackLayout
            {
                Spacing = 2,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End
            };

            // Barra de registrados (verde)
            var registeredBar = new Border
            {
                BackgroundColor = Color.FromArgb("#43A047"),
                HeightRequest = Math.Max(registeredHeight, 2),
                WidthRequest = 8,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 2 },
                Stroke = Colors.Transparent
            };

            // Barra de pendientes (azul)
            var pendingBar = new Border
            {
                BackgroundColor = Color.FromArgb("#1976D2"),
                HeightRequest = Math.Max(pendingHeight, 2),
                WidthRequest = 8,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 2 },
                Stroke = Colors.Transparent
            };

            barContainer.Children.Add(registeredBar);
            barContainer.Children.Add(pendingBar);

            Grid.SetColumn(barContainer, i);
            ChartGrid.Children.Add(barContainer);
        }

        NoDataLabel.IsVisible = false;
    }

    // Manejar click en botón de recargar
    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        // Evitar múltiples clics mientras se carga
        if (RefreshSpinner.IsRunning)
            return;

        await RefreshDashboardWithAnimation();
    }

    // Refrescar dashboard con animación en el botón
    private async Task RefreshDashboardWithAnimation()
    {
        try
        {
            // Mostrar estado de carga en el botón
            SetRefreshButtonLoading(true);

            // Cargar los datos
            await LoadDashboardDataAsync();
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
            RefreshIcon.IsVisible = !isLoading;
            RefreshSpinner.IsVisible = isLoading;
            RefreshSpinner.IsRunning = isLoading;
            RefreshButtonText.Text = isLoading ? "Actualizando..." : "Actualizar Dashboard";
            RefreshButton.Opacity = isLoading ? 0.7 : 1.0;
        });
    }

    private void ShowError(string message)
    {
        ErrorLabel.Text = message;
        ErrorBorder.IsVisible = true;
    }
}
