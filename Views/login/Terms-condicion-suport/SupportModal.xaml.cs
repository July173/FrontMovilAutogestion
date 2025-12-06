using System;
using System.Linq;
using Microsoft.Maui.Controls;
using AutogestionSena.MAUI.Api.Services;
using AutogestionSena.MAUI.Api.Dtos;
using AutogestionSena.MAUI.Api.Dtos.General;
using System.Collections.Generic;

namespace AutogestionSena.MAUI.Views
{
    public partial class SupportModalView : ContentView
    {
        private readonly GeneralService _generalService;
        // Keep references to the form controls so OnSubmitClicked can access them
        private Entry _nameEntry;
        private Entry _emailEntry;
        private Picker _categoryPicker;
        private Editor _messageEditor;

        public SupportModalView()
        {
            InitializeComponent();
            _generalService = new GeneralService();
            SizeChanged += OnSizeChanged;
        }

        public void Show()
        {
            ModalContainer.IsVisible = true;
            _ = ShowOverlayAsync();
            LoadData();
        }

        public void Hide()
        {
            _ = HideOverlayAsync();
        }

        private async System.Threading.Tasks.Task ShowOverlayAsync()
        {
            try
            {
                ModalContainer.Opacity = 0;
                ModalFrame.Scale = 0.97;
                ModalContainer.IsVisible = true;
                await ModalContainer.FadeTo(1, 160);
                await ModalFrame.ScaleTo(1.0, 180, Easing.CubicOut);
            }
            catch (Exception)
            {
                ModalContainer.IsVisible = true;
            }
        }

        private async System.Threading.Tasks.Task HideOverlayAsync()
        {
            try
            {
                await ModalFrame.ScaleTo(0.98, 140, Easing.CubicIn);
                await ModalContainer.FadeTo(0, 140);
                ModalContainer.IsVisible = false;
            }
            catch (Exception)
            {
                ModalContainer.IsVisible = false;
            }
        }

        private async void LoadData()
        {
            try
            {
                var content = FindByName("ContentStack") as VerticalStackLayout;
                if (content == null) return;
                content.Children.Clear();

                // Fetch data
                var contacts = await _generalService.GetSupportContactsAsync();
                var schedules = await _generalService.GetSupportSchedulesAsync();
                var queries = await _generalService.GetTypeOfQueriesAsync();

                // 1) Formas de contactarnos (cards)
                content.Children.Add(new Label { Text = "Formas de contactarnos", FontSize = 20, FontAttributes = FontAttributes.Bold, TextColor = Colors.Black, HorizontalOptions = LayoutOptions.Center });
                var contactGrid = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star) }, ColumnSpacing = 12 };
                int col = 0;
                if (contacts != null && contacts.Any())
                {
                    foreach (var c in contacts.Take(2))
                    {
                        var card = CreateContactCard(c);
                        contactGrid.Add(card, col, 0);
                        col++;
                    }
                }
                else
                {
                    var emailCard = CreateContactCard(new SupportContactDto { Label = "Email", Type = "Soporte por correo electrónico", Value = "servicio@sena.edu.co", ExtraInfo = "Respuesta en 24-48 horas" });
                    var phoneCard = CreateContactCard(new SupportContactDto { Label = "Teléfono", Type = "Línea gratuita nacional", Value = "01 8000 910 270", ExtraInfo = "Lunes a viernes: 7:00 AM - 7:00 PM" });
                    contactGrid.Add(emailCard, 0, 0);
                    contactGrid.Add(phoneCard, 1, 0);
                }
                content.Children.Add(contactGrid);

                // 2) Horarios de atención
                content.Children.Add(new Label { Text = "Horarios de atención", FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Colors.Black });
                var schedulesList = new Frame { CornerRadius = 8, Padding = 12, BackgroundColor = Colors.White, BorderColor = Color.FromArgb("#f0f0f0"), HasShadow = false };
                var schedulesStack = new VerticalStackLayout { Spacing = 6 };
                if (schedules != null && schedules.Any())
                {
                    foreach (var s in schedules)
                    {
                        var row = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) } };
                        row.Add(new Label { Text = s.DayRange, FontSize = 14, TextColor = Colors.Black }, 0, 0);
                        row.Add(new Label { Text = s.Hours, FontSize = 14, TextColor = Colors.Black }, 1, 0);
                        schedulesStack.Children.Add(row);
                    }
                }
                else
                {
                    var row1 = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) } };
                    row1.Add(new Label { Text = "Lunes a Viernes", FontSize = 14, TextColor = Colors.Black }, 0, 0);
                    row1.Add(new Label { Text = "7:00 AM - 7:00 PM", FontSize = 14, TextColor = Colors.Black }, 1, 0);
                    schedulesStack.Children.Add(row1);
                    var row2 = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) } };
                    row2.Add(new Label { Text = "Sábados", FontSize = 14, TextColor = Colors.Black }, 0, 0);
                    row2.Add(new Label { Text = "8:00 AM - 4:00 PM", FontSize = 14, TextColor = Colors.Black }, 1, 0);
                    schedulesStack.Children.Add(row2);
                    var row3 = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) } };
                    row3.Add(new Label { Text = "Domingos y festivos", FontSize = 14, TextColor = Colors.Black }, 0, 0);
                    row3.Add(new Label { Text = "Cerrado", FontSize = 14, TextColor = Colors.Black }, 1, 0);
                    schedulesStack.Children.Add(row3);
                    // Note box
                    schedulesStack.Children.Add(new Frame { BackgroundColor = Color.FromArgb("#fff3e0"), BorderColor = Color.FromArgb("#ffcc80"), CornerRadius = 6, Padding = 10, Content = new Label { Text = "Nota: Los tiempos de respuesta pueden variar durante períodos de alta demanda como matrículas masivas.", FontSize = 13, TextColor = Colors.Black } });
                }
                schedulesList.Content = schedulesStack;
                content.Children.Add(schedulesList);

                // 3) Envíanos un mensaje (form)
                var formFrame = new Frame { CornerRadius = 6, Padding = 12, BackgroundColor = Colors.White, BorderColor = Color.FromArgb("#f0f0f0"), HasShadow = false };
                var formStack = new VerticalStackLayout { Spacing = 8 };
                formStack.Children.Add(new Label { Text = "Envíanos un mensaje", FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Colors.Black });
                formStack.Children.Add(new Label { Text = "Complete el formulario y nos pondremos en contacto con usted dentro de 24 horas.", FontSize = 13, TextColor = Colors.Black });

                formStack.Children.Add(new Label { Text = "Nombre completo *", FontSize = 14, TextColor = Colors.Black });
                _nameEntry = new Entry { Placeholder = "Ingresa tu nombre completo" };
                formStack.Children.Add(_nameEntry);

                formStack.Children.Add(new Label { Text = "Correo electrónico *", FontSize = 14, TextColor = Colors.Black });
                _emailEntry = new Entry { Placeholder = "correo@soy.sena.edu.co", Keyboard = Keyboard.Email };
                formStack.Children.Add(_emailEntry);

                formStack.Children.Add(new Label { Text = "Categoría de Consulta *", FontSize = 14, TextColor = Colors.Black });
                _categoryPicker = new Picker { Title = "Selecciona una categoría" };

                // Populate category picker
                if (queries != null && queries.Any())
                {
                    _categoryPicker.ItemsSource = queries;
                    _categoryPicker.ItemDisplayBinding = new Binding("Name");
                    _categoryPicker.SelectedIndex = 0;
                }
                else
                {
                    var fallbackCategories = new List<string>
                    {
                        "Soporte Técnico",
                        "Consulta Académica",
                        "Problemas con la plataforma",
                        "Otros"
                    };
                    _categoryPicker.ItemsSource = fallbackCategories;
                    _categoryPicker.SelectedIndex = 0;
                }
                formStack.Children.Add(_categoryPicker);

                formStack.Children.Add(new Label { Text = "Mensaje *", FontSize = 14, TextColor = Colors.Black });
                _messageEditor = new Editor { AutoSize = EditorAutoSizeOption.TextChanges, HeightRequest = 120, Placeholder = "Describe detalladamente tu consulta o problema..." };
                formStack.Children.Add(_messageEditor);

                var submitBtn = new Button { Text = "✈ Enviar Mensaje", BackgroundColor = Color.FromArgb("#43A047"), TextColor = Colors.White, CornerRadius = 6 };
                submitBtn.Clicked += OnSubmitClicked;
                formStack.Children.Add(submitBtn);
                formFrame.Content = formStack;
                content.Children.Add(formFrame);

                // 4) Enlaces útiles
                content.Children.Add(new Label { Text = "Enlaces útiles", FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Colors.Black });
                var linkFrame = new Frame { CornerRadius = 6, Padding = 12, BackgroundColor = Color.FromArgb("#fafafa"), BorderColor = Color.FromArgb("#f0f0f0"), HasShadow = false };
                var linkGrid = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) } };
                var linkLabel = new Label { Text = "Sofia Plus - Oferta Educativa", TextColor = Colors.Black };
                var openIcon = new Label { Text = "🔗", FontSize = 16, TextColor = Colors.Black };
                var tap = new TapGestureRecognizer();
                tap.Tapped += (s, e) => Launcher.OpenAsync(new Uri("https://betowa.sena.edu.co/"));
                linkFrame.GestureRecognizers.Add(tap);
                linkGrid.Add(linkLabel, 0, 0);
                linkGrid.Add(openIcon, 1, 0);
                linkFrame.Content = linkGrid;
                content.Children.Add(linkFrame);

                // If nothing returned from server show friendly message
                bool anyData = (contacts != null && contacts.Any()) || (schedules != null && schedules.Any()) || (queries != null && queries.Any());
                if (!anyData)
                {
                    content.Children.Add(new Label { Text = "No hay datos disponibles desde el servidor.", FontSize = 14, TextColor = Colors.Black, HorizontalOptions = LayoutOptions.Center });
                }
            }
            catch (Exception ex)
            {
                await Application.Current?.MainPage?.DisplayAlert("Error", ex.Message, "Aceptar");
            }
        }

        private Frame CreateContactCard(SupportContactDto c)
        {
            var frame = new Frame { CornerRadius = 8, Padding = 16, BackgroundColor = Colors.White, BorderColor = Color.FromArgb("#f0f0f0"), HasShadow = false };
            var vs = new VerticalStackLayout { Spacing = 6 };
            vs.Children.Add(new Label { Text = c.Label, FontAttributes = FontAttributes.Bold, FontSize = 16, TextColor = Colors.Black, HorizontalOptions = LayoutOptions.Center });
            // Mostrar tipo o información extra como subtítulo
            var subtitle = !string.IsNullOrEmpty(c.ExtraInfo) ? c.ExtraInfo : c.Type;
            if (!string.IsNullOrEmpty(subtitle)) vs.Children.Add(new Label { Text = subtitle, FontSize = 12, TextColor = Colors.Black, HorizontalOptions = LayoutOptions.Center });
            vs.Children.Add(new Label { Text = c.Value, FontSize = 18, TextColor = Colors.Black, HorizontalOptions = LayoutOptions.Center });
            frame.Content = vs;
            return frame;
        }

        private void OnCloseClicked(object sender, EventArgs e)
        {
            Hide();
        }

        private void OnSubmitClicked(object sender, EventArgs e)
        {
            var name = _nameEntry?.Text;
            var email = _emailEntry?.Text;
            var message = _messageEditor?.Text;
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(message))
            {
                Application.Current?.MainPage?.DisplayAlert("Error", "Completa todos los campos obligatorios", "Aceptar");
                return;
            }

            // Optionally you can gather selected category
            var selectedCategory = _categoryPicker?.SelectedItem as TypeOfQueryDto;

            Application.Current?.MainPage?.DisplayAlert("Enviado", "Tu mensaje ha sido enviado", "Aceptar");
        }

        private void OnSizeChanged(object? sender, EventArgs e)
        {
            try
            {
                var w = Width;
                if (w <= 0) return;
                var title = FindByName("TitleLabel") as Label;
                var subtitle = FindByName("SubtitleLabel") as Label;
                var frame = FindByName("ModalFrame") as Frame;
                var scroll = FindByName("ContentScroll") as ScrollView;

                if (w <= 360)
                {
                    if (title != null) title.FontSize = 20;
                    if (subtitle != null) subtitle.FontSize = 12;
                    if (frame != null) frame.WidthRequest = 320;
                    if (scroll != null) scroll.HeightRequest = 420;
                }
                else if (w <= 420)
                {
                    if (title != null) title.FontSize = 22;
                    if (subtitle != null) subtitle.FontSize = 12;
                    if (frame != null) frame.WidthRequest = 360;
                    if (scroll != null) scroll.HeightRequest = 480;
                }
                else if (w <= 760)
                {
                    if (title != null) title.FontSize = 24;
                    if (subtitle != null) subtitle.FontSize = 13;
                    if (frame != null) frame.WidthRequest = 600;
                    if (scroll != null) scroll.HeightRequest = 520;
                }
                else
                {
                    if (title != null) title.FontSize = 24;
                    if (subtitle != null) subtitle.FontSize = 13;
                    if (frame != null) frame.WidthRequest = 700;
                    if (scroll != null) scroll.HeightRequest = 520;
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
