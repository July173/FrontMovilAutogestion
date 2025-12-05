using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutogestionSenaMaui.ViewModels;
using AutogestionSena.MAUI.Api.Services;
using AutogestionSena.MAUI.Api.Dtos;
using Microsoft.Maui.Controls;
using AutogestionSenaMaui.Helpers;
using AutogestionSena.MAUI.Api;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace AutogestionSenaMaui.Views
{
    public partial class AdminDashboardPage : ContentPage
    {
        private readonly AdminDashboardViewModel _viewModel;
        private readonly MenuService _menuService;
        private readonly AssignmentService _assignmentService;
        private readonly ApprenticeSimpleService _apprenticeService;
        private readonly DashboardCardsViewModel _cardsViewModel;

        // Timer para refresco automático
        private System.Timers.Timer? _autoRefreshTimer;

        public AdminDashboardPage()
        {
            InitializeComponent();
            _viewModel = new AdminDashboardViewModel();
            _menuService = new MenuService();
            _assignmentService = new AssignmentService();
            _apprenticeService = new ApprenticeSimpleService();
            _cardsViewModel = new DashboardCardsViewModel();
            BindingContext = _cardsViewModel; // Cambiar BindingContext para las cards
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                await _viewModel.LoadDataAsync();
                await LoadStatisticsAsync();
                await _cardsViewModel.LoadAsync();

                // Iniciar refresco automático cada 30 segundos
                if (_autoRefreshTimer == null)
                {
                    _autoRefreshTimer = new System.Timers.Timer(30000); // 30 segundos
                    _autoRefreshTimer.Elapsed += async (s, e) =>
                    {
                        // Ejecutar en el hilo principal de UI
                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            await LoadStatisticsAsync();
                        });
                    };
                    _autoRefreshTimer.AutoReset = true;
                    _autoRefreshTimer.Start();
                }
            }
            catch (Exception)
            {
                // Error loading data handled silently
            }
            // Configurar breadcrumb en el MainLayout TopBar
            MainLayoutHelper.UpdateCurrentBreadcrumb("Dashboard", "Administración");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (_autoRefreshTimer != null)
            {
                _autoRefreshTimer.Stop();
                _autoRefreshTimer.Dispose();
                _autoRefreshTimer = null;
            }
            // (TopBar is in MainLayout; menu toggling is handled by MainLayoutPage)
        }

        /// <summary>
        /// Obtiene el nombre del rol
        /// </summary>
        private string GetRoleName(int roleId)
        {
            return roleId switch
            {
                1 => "Administrador",
                2 => "Aprendiz",
                3 => "Instructor",
                4 => "Coordinador",
                5 => "Operador SofiaPlus",
                _ => "Desconocido"
            };
        }

        /// <summary>
        /// Extrae el nombre del usuario desde el email
        /// </summary>
        private string GetUserNameFromEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return "N/A";

            try
            {
                var emailPart = email.Split('@')[0];
                var nameParts = emailPart.Split('.');

                if (nameParts.Length >= 2)
                {
                    var firstName = char.ToUpper(nameParts[0][0]) + nameParts[0].Substring(1).ToLower();
                    var lastName = char.ToUpper(nameParts[1][0]) + nameParts[1].Substring(1).ToLower();
                    return $"{firstName} {lastName}";
                }
                else if (nameParts.Length == 1)
                {
                    return char.ToUpper(nameParts[0][0]) + nameParts[0].Substring(1).ToLower();
                }
            }
            catch { }

            return email;
        }

        /// <summary>
        /// Carga las estadísticas de aprendices y asignaciones
        /// </summary>
        private async Task LoadStatisticsAsync()
        {
            try
            {
                // Configurar token de autenticación
                var authToken = Preferences.Get("AuthToken", string.Empty);
                if (!string.IsNullOrEmpty(authToken))
                {
                    _assignmentService.SetAuthToken(authToken);
                    _apprenticeService.SetAuthToken(authToken);
                }

                // CARGAR APRENDICES
                List<ApprenticeSimpleDto>? apprentices = null;
                try
                {
                    apprentices = await _apprenticeService.GetAllApprenticesAsync();

                    if (apprentices != null)
                    {
                        var activeCount = apprentices.Count(a => a.Active);
                        TotalApprenticesLabel.Text = activeCount.ToString("N0");
                    }
                    else
                    {
                        TotalApprenticesLabel.Text = "0";
                    }
                }
                catch (Exception)
                {
                    TotalApprenticesLabel.Text = "Error";
                }

                // CARGAR ASIGNACIONES
                AssignmentRequestListResponse? assignments = null;
                int assignedCount = 0;
                int unassignedCount = 0;

                try
                {
                    assignments = await _assignmentService.GetFormRequestListAsync();

                    if (assignments != null && assignments.Success && assignments.Data != null)
                    {
                        assignedCount = assignments.Data.Count(a => a.RequestState == "ASIGNADO" || a.RequestState == "APROBADO");
                        unassignedCount = assignments.Data.Count(a => a.RequestState == "SIN_ASIGNAR");

                        TotalAssignmentsLabel.Text = assignedCount.ToString();
                        UnassignedCountLabel.Text = unassignedCount.ToString();
                    }
                    else
                    {
                        TotalAssignmentsLabel.Text = "0";
                        UnassignedCountLabel.Text = "0";
                    }
                }
                catch (Exception)
                {
                    TotalAssignmentsLabel.Text = "Error";
                    UnassignedCountLabel.Text = "Error";
                }

                // Cargar gráficas
                LoadCharts(assignedCount, unassignedCount);
            }
            catch (Exception)
            {
                // General statistics error handled silently
            }
        }

        /// <summary>
        /// Carga las gráficas de barras y líneas con datos reales
        /// </summary>
        private async void LoadCharts(int assigned, int unassigned)
        {
            try
            {
                // Configurar token de autenticación
                var authToken = Preferences.Get("AuthToken", string.Empty);
                if (!string.IsNullOrEmpty(authToken))
                {
                    _assignmentService.SetAuthToken(authToken);
                }

                // Obtener todas las asignaciones
                var response = await _assignmentService.GetFormRequestListAsync();

                if (response != null && response.Success && response.Data != null)
                {
                    var assignments = response.Data;

                    var statusCounts = assignments
                        .GroupBy(a => a.RequestState ?? "DESCONOCIDO")
                        .Select(g => new { Status = g.Key, Count = g.Count() })
                        .OrderByDescending(x => x.Count)
                        .ToList();

                    // Create bar chart series for assignments
                    var barValues = statusCounts.Select(x => (double)x.Count).ToArray();
                    var barLabels = statusCounts.Select(x => GetShortStatus(x.Status)).ToArray();

                    if (!barValues.Any())
                    {
                        barValues = new double[] { 0 };
                        barLabels = new string[] { "Sin datos" };
                    }

                    AssignmentsChart.Series = new ISeries[]
                    {
                        new ColumnSeries<double>
                        {
                            Values = barValues,
                            Fill = new SolidColorPaint(SKColor.Parse("#4CAF50")),
                            Name = "Asignaciones"
                        }
                    };

                    AssignmentsChart.XAxes = new Axis[]
                    {
                        new Axis
                        {
                            Labels = barLabels,
                            LabelsRotation = 0,
                            TextSize = 12
                        }
                    };

                    // Line chart for approved over time
                    var allByMonth = assignments
                        .Where(a => a.CreatedAt.HasValue)
                        .GroupBy(a => new { Year = a.CreatedAt.Value.Year, Month = a.CreatedAt.Value.Month })
                        .Select(g => new
                        {
                            Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                            Count = g.Count()
                        })
                        .OrderBy(x => x.Date)
                        .TakeLast(6)
                        .ToList();

                    double[] lineValues;
                    string[] lineLabels;

                    if (allByMonth.Any())
                    {
                        lineValues = allByMonth.Select(x => (double)x.Count).ToArray();
                        lineLabels = allByMonth.Select(x => x.Date.ToString("MMM")).ToArray();
                    }
                    else
                    {
                        lineValues = new double[] { 0 };
                        lineLabels = new string[] { "Sin datos" };
                    }

                    ApprovedChart.Series = new ISeries[]
                    {
                        new LineSeries<double>
                        {
                            Values = lineValues,
                            Stroke = new SolidColorPaint(SKColor.Parse("#2196F3")) { StrokeThickness = 3 },
                            Fill = null,
                            GeometryFill = new SolidColorPaint(SKColor.Parse("#2196F3")),
                            GeometrySize = 10,
                            Name = "Solicitudes"
                        }
                    };

                    ApprovedChart.XAxes = new Axis[]
                    {
                        new Axis
                        {
                            Labels = lineLabels,
                            LabelsRotation = 0,
                            TextSize = 12
                        }
                    };
                }
                else
                {
                    // Empty charts
                    AssignmentsChart.Series = new ISeries[]
                    {
                        new ColumnSeries<double>
                        {
                            Values = new double[] { 0 },
                            Fill = new SolidColorPaint(SKColor.Parse("#CCCCCC")),
                            Name = "Sin datos"
                        }
                    };

                    ApprovedChart.Series = new ISeries[]
                    {
                        new LineSeries<double>
                        {
                            Values = new double[] { 0 },
                            Stroke = new SolidColorPaint(SKColor.Parse("#CCCCCC")),
                            Name = "Sin datos"
                        }
                    };
                }
            }
            catch (Exception)
            {
                // Charts loading error handled silently
            }
        }

        /// <summary>
        /// Obtiene una versión corta del estado para mostrar en las gráficas
        /// </summary>
        private string GetShortStatus(string status)
        {
            return status switch
            {
                "SIN_ASIGNAR" => "Sin Asig.",
                "ASIGNADO" => "Asignado",
                "APROBADO" => "Aprobado",
                "RECHAZADO" => "Rechazado",
                "EN_PROCESO" => "En Proc.",
                _ => status?.Length > 8 ? status.Substring(0, 8) : status ?? "N/A"
            };
        }

        /// <summary>
        /// Botón para recargar las estadísticas
        /// </summary>
        private async void OnReloadStatistics(object sender, EventArgs e)
        {
            try
            {
                await LoadStatisticsAsync();
            }
            catch (Exception)
            {
                // Reload error handled silently
            }
        }
    }
}
