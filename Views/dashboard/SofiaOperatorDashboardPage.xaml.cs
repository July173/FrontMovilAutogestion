using Microsoft.Maui.Controls;

using AutogestionSenaMaui.ContentViews;
using AutogestionSenaMaui.Helpers;
namespace AutogestionSenaMaui.Views;

public partial class SofiaOperatorDashboardPage : ContentPage
{
    public SofiaOperatorDashboardPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        MainLayoutHelper.UpdateCurrentBreadcrumb("Dashboard", "Operador SofiaPlus");
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }
}
