using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AutogestionSena.MAUI.Api.Dtos;
using AutogestionSena.MAUI.Api.Services;

namespace AutogestionSenaMaui.ViewModels
{
    public class SofiaOperatorDashboardViewModel : INotifyPropertyChanged
    {
        private readonly OperatorDashboardService _dashboardService;

        private bool _isLoading;
        private string _userName = string.Empty;
        private int _year;
        private int _totalRegistered;
        private int _totalPending;
        private int _totalGeneral;
        private string _errorMessage = string.Empty;
        private bool _hasError;

        public event PropertyChangedEventHandler? PropertyChanged;

        public SofiaOperatorDashboardViewModel()
        {
            _dashboardService = new OperatorDashboardService();
            MonthlyData = new ObservableCollection<MonthlyDataDto>();
            Year = DateTime.Now.Year;
        }

        #region Properties

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }

        public int Year
        {
            get => _year;
            set
            {
                _year = value;
                OnPropertyChanged();
            }
        }

        public int TotalRegistered
        {
            get => _totalRegistered;
            set
            {
                _totalRegistered = value;
                OnPropertyChanged();
            }
        }

        public int TotalPending
        {
            get => _totalPending;
            set
            {
                _totalPending = value;
                OnPropertyChanged();
            }
        }

        public int TotalGeneral
        {
            get => _totalGeneral;
            set
            {
                _totalGeneral = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public bool HasError
        {
            get => _hasError;
            set
            {
                _hasError = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MonthlyDataDto> MonthlyData { get; }

        #endregion

        #region Methods

        public async Task LoadDashboardDataAsync()
        {
            try
            {
                IsLoading = true;
                HasError = false;
                ErrorMessage = string.Empty;

                // Obtener email del usuario desde SecureStorage
                var userEmail = await SecureStorage.GetAsync("user_email");
                UserName = !string.IsNullOrEmpty(userEmail) ? userEmail : "Usuario";

                // Obtener token de autenticación
                var token = await SecureStorage.GetAsync("auth_token");

                var response = await _dashboardService.GetOperatorDashboardAsync(token);

                if (response != null && response.Success && response.Data != null)
                {
                    Year = response.Data.Year;

                    // Actualizar totales
                    if (response.Data.Totals != null)
                    {
                        TotalRegistered = response.Data.Totals.Registered;
                        TotalPending = response.Data.Totals.Pending;
                        TotalGeneral = response.Data.Totals.Total;
                    }

                    // Actualizar datos mensuales
                    MonthlyData.Clear();
                    foreach (var monthData in response.Data.MonthlyData)
                    {
                        MonthlyData.Add(monthData);
                    }
                }
                else
                {
                    HasError = true;
                    ErrorMessage = response?.Message ?? "Error al cargar los datos del dashboard";
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Error: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Error en LoadDashboardDataAsync: {ex}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
