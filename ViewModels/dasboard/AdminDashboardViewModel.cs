using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using AutogestionSena.MAUI.Api.Services;
using AutogestionSena.MAUI.Api.Dtos;

namespace AutogestionSenaMaui.ViewModels
{
    public class AdminDashboardViewModel : BindableObject
    {
        private readonly AdminService _adminService;
        private int _totalUsers;
        private int _totalRoles;
        private int _totalModules;
        private int _totalForms;
        private bool _isLoading;

        public AdminDashboardViewModel()
        {
            _adminService = new AdminService();
            RefreshCommand = new Command(async () => await LoadDataAsync());
        }

        public int TotalUsers { get => _totalUsers; set { _totalUsers = value; OnPropertyChanged(); } }
        public int TotalRoles { get => _totalRoles; set { _totalRoles = value; OnPropertyChanged(); } }
        public int TotalModules { get => _totalModules; set { _totalModules = value; OnPropertyChanged(); } }
        public int TotalForms { get => _totalForms; set { _totalForms = value; OnPropertyChanged(); } }

        public bool IsLoading { get => _isLoading; set { _isLoading = value; OnPropertyChanged(); } }

        public ICommand RefreshCommand { get; }

        public async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;
                var counts = await _adminService.GetAdminCountsAsync();
                if (counts != null)
                {
                    TotalUsers = counts.TotalUsers;
                    TotalRoles = counts.TotalRoles;
                    TotalModules = counts.TotalModules;
                    TotalForms = counts.TotalForms;
                }
            }
            catch (Exception)
            {
                // Admin counts load error handled silently
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
