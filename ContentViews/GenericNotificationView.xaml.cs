using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;

namespace ContentViews
{
    public partial class GenericNotificationView : ContentView
    {
        public enum NotificationType
        {
            Error,
            Success,
            Info,
            Warning,
            Mail,
            PasswordChanged
        }

        public static readonly BindableProperty TypeProperty = BindableProperty.Create(
            nameof(Type), typeof(NotificationType), typeof(GenericNotificationView), NotificationType.Error, propertyChanged: OnTypeChanged);

        public static readonly BindableProperty TitleProperty = BindableProperty.Create(
            nameof(Title), typeof(string), typeof(GenericNotificationView), string.Empty, propertyChanged: OnTitleChanged);

        public static readonly BindableProperty MessageProperty = BindableProperty.Create(
            nameof(Message), typeof(string), typeof(GenericNotificationView), string.Empty, propertyChanged: OnMessageChanged);

        public static readonly BindableProperty PrimaryButtonTextProperty = BindableProperty.Create(
            nameof(PrimaryButtonText), typeof(string), typeof(GenericNotificationView), "Aceptar", propertyChanged: OnPrimaryButtonTextChanged);

        public static readonly BindableProperty SecondaryButtonTextProperty = BindableProperty.Create(
            nameof(SecondaryButtonText), typeof(string), typeof(GenericNotificationView), "Cancelar", propertyChanged: OnSecondaryButtonTextChanged);

        public static readonly BindableProperty ShowSecondaryButtonProperty = BindableProperty.Create(
            nameof(ShowSecondaryButton), typeof(bool), typeof(GenericNotificationView), false, propertyChanged: OnShowSecondaryButtonChanged);

        public NotificationType Type
        {
            get => (NotificationType)GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public string PrimaryButtonText
        {
            get => (string)GetValue(PrimaryButtonTextProperty);
            set => SetValue(PrimaryButtonTextProperty, value);
        }

        public string SecondaryButtonText
        {
            get => (string)GetValue(SecondaryButtonTextProperty);
            set => SetValue(SecondaryButtonTextProperty, value);
        }

        public bool ShowSecondaryButton
        {
            get => (bool)GetValue(ShowSecondaryButtonProperty);
            set => SetValue(ShowSecondaryButtonProperty, value);
        }

        public event EventHandler? PrimaryButtonClicked;
        public event EventHandler? SecondaryButtonClicked;

        public GenericNotificationView()
        {
            InitializeComponent();
            UpdateType();
            TitleLabel.Text = Title;
            MessageLabel.Text = Message;
            PrimaryButton.Text = PrimaryButtonText;
            SecondaryButton.Text = SecondaryButtonText;
            SecondaryButton.IsVisible = ShowSecondaryButton;
        }

        private static void OnTypeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is GenericNotificationView view)
                view.UpdateType();
        }

        private static void OnTitleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is GenericNotificationView view && newValue is string title)
                view.TitleLabel.Text = title;
        }

        private static void OnMessageChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is GenericNotificationView view && newValue is string message)
                view.MessageLabel.Text = message;
        }

        private static void OnPrimaryButtonTextChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is GenericNotificationView view && newValue is string text)
                view.PrimaryButton.Text = text;
        }

        private static void OnSecondaryButtonTextChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is GenericNotificationView view && newValue is string text)
                view.SecondaryButton.Text = text;
        }

        private static void OnShowSecondaryButtonChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is GenericNotificationView view && newValue is bool visible)
                view.SecondaryButton.IsVisible = visible;
        }

        private void UpdateType()
        {
            switch (Type)
            {
                case NotificationType.Error:
                    NotificationFrame.BorderColor = Colors.Red;
                    IconImage.Source = "https://www.figma.com/api/mcp/asset/09fd20bc-0a10-4564-a07c-24fbeae82a1f";
                    break;
                case NotificationType.Success:
                    NotificationFrame.BorderColor = Color.FromArgb("#61f659");
                    IconImage.Source = "https://www.figma.com/api/mcp/asset/93e9a3a0-dda0-4c11-9477-ff6a7d9c6a16";
                    break;
                case NotificationType.Info:
                    NotificationFrame.BorderColor = Colors.Blue;
                    IconImage.Source = "https://www.figma.com/api/mcp/asset/76d03de7-75c6-4d7e-8b0c-fb5cd4db558a";
                    break;
                case NotificationType.Warning:
                    NotificationFrame.BorderColor = Color.FromArgb("#fed801");
                    IconImage.Source = "https://www.figma.com/api/mcp/asset/d19787a3-6642-4f55-9734-c8b2f1826495";
                    break;
                case NotificationType.Mail:
                    NotificationFrame.BorderColor = Color.FromArgb("#7bcc7f");
                    IconImage.Source = "https://www.figma.com/api/mcp/asset/4157a06d-6d01-43c6-9e74-dc15d5303166";
                    break;
                case NotificationType.PasswordChanged:
                    NotificationFrame.BorderColor = Color.FromArgb("#7bcc7f");
                    IconImage.Source = "https://www.figma.com/api/mcp/asset/a25977d7-de19-454a-993a-36d76a178c34";
                    break;
            }
        }

        private void OnPrimaryButtonClicked(object sender, EventArgs e)
        {
            PrimaryButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        private void OnSecondaryButtonClicked(object sender, EventArgs e)
        {
            SecondaryButtonClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
