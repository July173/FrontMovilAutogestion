using Microsoft.Maui.Controls;
using AutogestionSenaMaui.ContentViews;
using AutogestionSenaMaui.Helpers;

namespace AutogestionSenaMaui.Views;

public partial class InstructorDashboardPage : ContentPage
{
    public InstructorDashboardPage()
    {
        InitializeComponent();
        LoadInstructorData();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        MainLayoutHelper.UpdateCurrentBreadcrumb("Dashboard", "Instructor");
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }

    // Override para aplicar dise�o responsive
    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        ApplyResponsiveLayout(width);
    }

    // Aplicar dise�o responsive seg�n el ancho de la pantalla
    private void ApplyResponsiveLayout(double width)
    {
        try
        {
            var welcomeGrid = this.FindByName<Grid>("WelcomeGrid");
            var instructorNameLabel = this.FindByName<Label>("InstructorNameLabel");

            if (welcomeGrid == null) return;

            System.Diagnostics.Debug.WriteLine($"[InstructorDashboard] Aplicando dise�o responsive para ancho: {width}px");

            // Ajustar tama�os de fuente seg�n el ancho de pantalla
            if (width <= 360) // Pantallas muy peque�as
            {
                // Ajustar tama�os para celulares peque�os
                if (instructorNameLabel != null)
                {
                    instructorNameLabel.FontSize = 16;
                }
            }
            else if (width <= 480) // Pantallas medianas (mayor�a de celulares)
            {
                if (instructorNameLabel != null)
                {
                    instructorNameLabel.FontSize = 18;
                }
            }
            else // Pantallas grandes (tablets y desktop)
            {
                if (instructorNameLabel != null)
                {
                    instructorNameLabel.FontSize = 20;
                }
            }
        }
        catch (Exception)
        {
            // Responsive layout error handled silently
        }
    }

    // Cargar datos del instructor (placeholder para datos reales)
    private void LoadInstructorData()
    {
        try
        {
            var instructorNameLabel = this.FindByName<Label>("InstructorNameLabel");

            string instructorName = GetInstructorNameFromStorage();

            if (instructorNameLabel != null && !string.IsNullOrEmpty(instructorName))
            {
                instructorNameLabel.Text = instructorName;
            }
        }
        catch (Exception)
        {
            // Load instructor data error handled silently
        }
    }

    // Obtener nombre del instructor desde almacenamiento local
    private string GetInstructorNameFromStorage()
    {
        try
        {
            // Intentar obtener datos del usuario desde Preferences
            var userDataJson = Preferences.Get("user_data", string.Empty);

            if (!string.IsNullOrEmpty(userDataJson))
            {
                var userData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(userDataJson);
                if (userData != null && userData.ContainsKey("firstName"))
                {
                    var firstName = userData["firstName"]?.ToString();
                    if (!string.IsNullOrEmpty(firstName))
                    {
                        return firstName;
                    }
                }
            }

            // Fallback: nombre por defecto
            return "Sneider Contreras Vargas";
        }
        catch (Exception)
        {
            return "Instructor";
        }
    }
}
