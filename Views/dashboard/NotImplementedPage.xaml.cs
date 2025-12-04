using Microsoft.Maui.Controls;

namespace AutogestionSenaMaui.Views;

[QueryProperty("BackendPath", "backendPath")]
public partial class NotImplementedPage : ContentPage
{
    public NotImplementedPage()
    {
        InitializeComponent();
    }

    private string _backendPath = string.Empty;
    public string BackendPath
    {
        get => _backendPath;
        set
        {
            _backendPath = value;
            BackendPathLabel.Text = Uri.UnescapeDataString(_backendPath ?? string.Empty);
        }
    }
}
