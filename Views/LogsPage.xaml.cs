using AutogestionSena.MAUI.ViewModels;

namespace AutogestionSena.MAUI.Views
{
    public partial class LogsPage : ContentPage
    {
        public LogsPage()
        {
            InitializeComponent();
            BindingContext = new LogsViewModel();
        }
    }
}
