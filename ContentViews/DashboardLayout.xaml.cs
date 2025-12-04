using AutogestionSenaMaui.ViewModels;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
namespace AutogestionSenaMaui.ContentViews;

public partial class DashboardLayout : ContentView
{
    public DashboardLayout()
    {
        InitializeComponent();
        // Cuando el componente se haya cargado, intentar seleccionar el dashboard por defecto
        this.Loaded += DashboardLayout_Loaded;
      
        // 🔧 ARREGLADO: Asegurar que el BindingContext se propague correctamente
        this.BindingContextChanged += DashboardLayout_BindingContextChanged;
    }

    // Propiedad para establecer el contenido principal
    public View PageContent
    {
        get => MainContent.Content;
        set 
        {
            MainContent.Content = value;
   
            // 🔧 ARREGLADO: Propagar el BindingContext al contenido
            if (value != null && this.BindingContext != null)
            {
                value.BindingContext = this.BindingContext;
            }
        }
    }

    private void DashboardLayout_BindingContextChanged(object? sender, EventArgs e)
    {
        if (MainContent.Content != null && this.BindingContext != null)
        {
            MainContent.Content.BindingContext = this.BindingContext;
        }
    }

    private async void OnNotificationsTapped(object sender, EventArgs e)
    {
        try
        {
            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync("//notifications");
            }
        }
        catch (Exception)
        {
            // Navigation error handled silently
        }
    }

    private void DashboardLayout_Loaded(object? sender, EventArgs e)
    {
        // El menú lateral ahora es manejado por las páginas principales (MainLayoutPage/HomePage)
        // Este método se mantiene para compatibilidad pero ya no gestiona el menú
    }
}
