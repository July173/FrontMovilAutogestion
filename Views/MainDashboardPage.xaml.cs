namespace AutogestionSenaMaui.Views;

public partial class MainDashboardPage : ContentPage
{
    public MainDashboardPage()
    {
        InitializeComponent();
        
        // Configurar el breadcrumb para esta página (usar MainLayout TopBar)
        AutogestionSenaMaui.Helpers.MainLayoutHelper.UpdateCurrentBreadcrumb("Dashboard", "Inicio");
    }

    // No local TopBar subscription; MainLayout handles TopBar events
}
