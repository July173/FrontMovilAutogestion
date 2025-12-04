using System;
using Microsoft.Maui.Controls;

namespace AutogestionSena.MAUI.Views
{
    public partial class TwoFactorModal : ContentView
    {
        private bool _isModalVisible;
        private string _email = string.Empty;
        
        public event EventHandler<string>? CodeVerified;
        public event EventHandler? Cancelled;

        public new bool IsVisible
        {
            get => _isModalVisible;
            set
            {
                _isModalVisible = value;
                ModalContainer.IsVisible = value;
                if (value)
                {
                    ClearCode();
                    Code1Entry.Focus();
                }
            }
        }

        public TwoFactorModal()
        {
            InitializeComponent();
            ModalContainer.IsVisible = false;
            this.SizeChanged += TwoFactorModal_SizeChanged;
        }

        private void TwoFactorModal_SizeChanged(object? sender, EventArgs e)
        {
            try
            {
                // Use the available width to size the modal frame proportionally
                var availableWidth = this.Width;
                if (availableWidth <= 0 && Application.Current?.MainPage != null)
                    availableWidth = Application.Current.MainPage.Width;

                if (availableWidth <= 0) return;

                // Modal width should be smaller - max 85% of screen width and not exceed 380
                var modalWidth = Math.Min(availableWidth * 0.85, 380);
                ModalFrame.WidthRequest = modalWidth;

                // Compute entry width, leave some spacing and padding
                const int entries = 6;
                const double entrySpacing = 8; // reduced spacing between entries
                var padding = 20 * 2; // ModalFrame padding left+right (reduced from 24)
                var totalSpacing = (entries - 1) * entrySpacing;
                var availableForEntries = Math.Max(0, modalWidth - padding - totalSpacing);
                var entryWidth = Math.Max(30, Math.Min(50, availableForEntries / entries));

                Code1Frame.WidthRequest = entryWidth;
                Code2Frame.WidthRequest = entryWidth;
                Code3Frame.WidthRequest = entryWidth;
                Code4Frame.WidthRequest = entryWidth;
                Code5Frame.WidthRequest = entryWidth;
                Code6Frame.WidthRequest = entryWidth;

                var entryHeight = Math.Max(36, entryWidth * 1.05);
                Code1Frame.HeightRequest = entryHeight;
                Code2Frame.HeightRequest = entryHeight;
                Code3Frame.HeightRequest = entryHeight;
                Code4Frame.HeightRequest = entryHeight;
                Code5Frame.HeightRequest = entryHeight;
                Code6Frame.HeightRequest = entryHeight;

                // Adjust fonts for smaller screens
                if (availableWidth <= 360)
                {
                    TitleLabel.FontSize = 15;
                    MessageLabel.FontSize = 11;
                    VerifyButton.FontSize = 13;
                }
                else if (availableWidth <= 420)
                {
                    TitleLabel.FontSize = 16;
                    MessageLabel.FontSize = 12;
                    VerifyButton.FontSize = 14;
                }
                else
                {
                    TitleLabel.FontSize = 18;
                    MessageLabel.FontSize = 13;
                    VerifyButton.FontSize = 15;
                }
            }
            catch (Exception)
            {
                // Responsive sizing error handled silently
            }
        }

        public void Show(string email)
        {
            _email = email;
            MessageLabel.Text = $"Código enviado a: {email}";
            IsVisible = true;
            _ = ShowOverlayAsync();
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
                ModalFrame.Scale = 0.95;
                ModalContainer.IsVisible = true;
                await ModalContainer.FadeTo(1, 175);
                await ModalFrame.ScaleTo(1.0, 175, Easing.CubicOut);
                ClearCode();
                Code1Entry.Focus();
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
                await ModalFrame.ScaleTo(0.98, 150, Easing.CubicIn);
                await ModalContainer.FadeTo(0, 150);
                ModalContainer.IsVisible = false;
            }
            catch (Exception)
            {
                ModalContainer.IsVisible = false;
            }
        }

        private void ClearCode()
        {
            Code1Entry.Text = string.Empty;
            Code2Entry.Text = string.Empty;
            Code3Entry.Text = string.Empty;
            Code4Entry.Text = string.Empty;
            Code5Entry.Text = string.Empty;
            Code6Entry.Text = string.Empty;
        }

        private void OnCodeEntryChanged(object? sender, TextChangedEventArgs e)
        {
            if (sender is Entry entry && !string.IsNullOrEmpty(entry.Text))
            {
                // Auto-avanzar al siguiente campo
                if (entry == Code1Entry && entry.Text.Length == 1)
                    Code2Entry.Focus();
                else if (entry == Code2Entry && entry.Text.Length == 1)
                    Code3Entry.Focus();
                else if (entry == Code3Entry && entry.Text.Length == 1)
                    Code4Entry.Focus();
                else if (entry == Code4Entry && entry.Text.Length == 1)
                    Code5Entry.Focus();
                else if (entry == Code5Entry && entry.Text.Length == 1)
                    Code6Entry.Focus();
            }
        }

        private void OnVerifyClicked(object? sender, EventArgs e)
        {
            var code = $"{Code1Entry.Text}{Code2Entry.Text}{Code3Entry.Text}{Code4Entry.Text}{Code5Entry.Text}{Code6Entry.Text}";
            
            if (code.Length != 6)
            {
                Application.Current?.MainPage?.DisplayAlert("Error", "Por favor ingresa los 6 dígitos del código", "Aceptar");
                return;
            }

            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            VerifyButton.IsEnabled = false;

            CodeVerified?.Invoke(this, code);
        }

        public void ShowError(string message)
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
            VerifyButton.IsEnabled = true;
            
            Application.Current?.MainPage?.DisplayAlert("Error", message, "Aceptar");
        }

        public void ShowSuccess()
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
            VerifyButton.IsEnabled = true;
            Hide();
        }

        private void OnCancelClicked(object? sender, EventArgs e)
        {
            Hide();
            Cancelled?.Invoke(this, EventArgs.Empty);
        }
    }
}
