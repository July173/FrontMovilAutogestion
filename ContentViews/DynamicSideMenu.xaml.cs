namespace AutogestionSenaMaui.ContentViews;

public partial class DynamicSideMenu : ContentView
{
    public DynamicSideMenu()
    {
        InitializeComponent();
        // Inicializar el ViewModel y asignarlo al BindingContext
        this.BindingContext = new AutogestionSenaMaui.ViewModels.DynamicSideMenuViewModel();
        this.Loaded += DynamicSideMenu_Loaded;
        // Ensure the view anchors to the start vertically (under TopBar)
        this.VerticalOptions = Microsoft.Maui.Controls.LayoutOptions.Start;
        this.HorizontalOptions = Microsoft.Maui.Controls.LayoutOptions.Start;
    }

    public AutogestionSenaMaui.ViewModels.DynamicSideMenuViewModel? ViewModel => this.BindingContext as AutogestionSenaMaui.ViewModels.DynamicSideMenuViewModel;

    private void DynamicSideMenu_Loaded(object? sender, EventArgs e)
    {
        try
        {
            // El ancho y posicionamiento ahora es manejado por las páginas principales
            // mediante WidthRequest animado. Este método se mantiene para configuración básica.
            
            System.Diagnostics.Debug.WriteLine($"[DynamicSideMenu] Loaded");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DynamicSideMenu] Error in Loaded: {ex}");
        }
    }
}
