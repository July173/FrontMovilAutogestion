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

                    var assignmentEntries = new List<Microcharts.ChartEntry>();
                    foreach (var item in statusCounts)
                    {
                        assignmentEntries.Add(new Microcharts.ChartEntry(item.Count)
                        {
                            Label = GetShortStatus(item.Status),
                            ValueLabel = item.Count.ToString(),
                            Color = SkiaSharp.SKColor.Parse("#4CAF50")
                        });
                    }

                    if (!assignmentEntries.Any())
                    {
                        assignmentEntries.Add(new Microcharts.ChartEntry(0)
                        {
                            Label = "Sin datos",
                            ValueLabel = "0",
                            Color = SkiaSharp.SKColor.Parse("#CCCCCC")
                        });
                    }

                    AssignmentsChart.Chart = new Microcharts.BarChart
                    {
                        Entries = assignmentEntries,
                        BackgroundColor = SkiaSharp.SKColors.White,
                        LabelTextSize = 28,
                        ValueLabelOrientation = Microcharts.Orientation.Horizontal,
                        LabelOrientation = Microcharts.Orientation.Horizontal,
                        IsAnimated = true
                    };

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

                    var approvedEntries = new List<Microcharts.ChartEntry>();

                    if (allByMonth.Any())
                    {
                        foreach (var item in allByMonth)
                        {
                            approvedEntries.Add(new Microcharts.ChartEntry(item.Count)
                            {
                                Label = item.Date.ToString("MMM"),
                                ValueLabel = item.Count.ToString(),
                                Color = SkiaSharp.SKColor.Parse("#2196F3")
                            });
                        }
                    }
                    else
                    {
                        approvedEntries.Add(new Microcharts.ChartEntry(0)
                        {
                            Label = "Sin datos",
                            ValueLabel = "0",
                            Color = SkiaSharp.SKColor.Parse("#CCCCCC")
                        });
                    }

                    ApprovedChart.Chart = new Microcharts.LineChart
                    {
                        Entries = approvedEntries,
                        BackgroundColor = SkiaSharp.SKColors.White,
                        LabelTextSize = 28,
                        LineMode = Microcharts.LineMode.Straight,
                        PointMode = Microcharts.PointMode.Circle,
                        PointSize = 15,
                        IsAnimated = true
                    };
                }
                else
                {
                    var emptyEntry = new List<Microcharts.ChartEntry>
                    {
                        new Microcharts.ChartEntry(0) { Label = "Sin datos", ValueLabel = "0", Color = SkiaSharp.SKColor.Parse("#CCCCCC") }
                    };

                    AssignmentsChart.Chart = new Microcharts.BarChart { Entries = emptyEntry };
                    ApprovedChart.Chart = new Microcharts.LineChart { Entries = emptyEntry };
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
