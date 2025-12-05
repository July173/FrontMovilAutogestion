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

            var response = await _dashboardService.GetOperatorDashboardAsync(token);

            if (response != null && response.Success && response.Data != null)
            {
                // Actualizar año
                YearLabel.Text = response.Data.Year.ToString();

                // Actualizar totales
                if (response.Data.Totals != null)
                {
                    RegisteredLabel.Text = response.Data.Totals.Registered.ToString();
                    PendingLabel.Text = response.Data.Totals.Pending.ToString();
                    TotalLabel.Text = response.Data.Totals.Total.ToString();
                }

                // Guardar datos mensuales y renderizar gráfico
                _monthlyData = response.Data.MonthlyData;
                RenderChart();

                NoDataLabel.IsVisible = false;
            }
            else
            {
                ShowError(response?.Message ?? "Error al cargar los datos del dashboard");
                NoDataLabel.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            ShowError($"Error: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Error en LoadDashboardDataAsync: {ex}");
        }
        finally
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
            ChartLoadingIndicator.IsVisible = false;
            ChartLoadingIndicator.IsRunning = false;
        }
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

    private void ShowError(string message)
    {
        ErrorLabel.Text = message;
        ErrorBorder.IsVisible = true;
    }
}
