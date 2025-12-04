using Microsoft.Maui.Controls;
using AutogestionSenaMaui.ViewModels;
using Microsoft.Maui.Storage;
using AutogestionSenaMaui.Helpers;

namespace AutogestionSenaMaui.Views;

public partial class ApprenticeDashboardPage : ContentPage
{
    public ApprenticeDashboardPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Set BindingContext and load dashboard data
        var vm = new ApprenticeDashboardViewModel();
        BindingContext = vm;

        // Cambiar el breadcrumb en el MainLayout TopBar
        MainLayoutHelper.UpdateCurrentBreadcrumb("Dashboard", "Aprendiz");

        try
        {
            // Get person id saved at login
            var personId = Preferences.Get("UserPerson", 0);
      
            if (personId > 0)
            {
                await vm.LoadAsync(personId);
            }
        }
        catch (Exception)
        {
            // Dashboard loading error handled silently
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }
}
