using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace AutogestionSenaMaui.ViewModels;

public class DashboardChartsViewModel : BindableObject
{
    private bool _isLoading;
    public bool IsLoading { get => _isLoading; set { _isLoading = value; OnPropertyChanged(); } }

    public DashboardChartsViewModel() { }
    public async Task LoadAsync() { IsLoading = true; await Task.Delay(200); IsLoading = false; }
}
