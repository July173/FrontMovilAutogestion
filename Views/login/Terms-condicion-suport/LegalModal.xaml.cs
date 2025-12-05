using System;
using System.Linq;
using Microsoft.Maui.Controls;
using AutogestionSena.MAUI.Api.Services;
using AutogestionSena.MAUI.Api.Dtos;
using System.Collections.Generic;

namespace AutogestionSena.MAUI.Views
{
    public partial class LegalModalView : ContentView
    {
        private readonly GeneralService _generalService;

        public LegalModalView()
        {
            InitializeComponent();
            _generalService = new GeneralService();
            SizeChanged += OnSizeChanged;
        }

        public async void Show(string docType)
        {
            ModalContainer.IsVisible = true;
            await ShowOverlayAsync();
            await LoadData(docType);
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

        private async System.Threading.Tasks.Task LoadData(string docType)
        {
            try
            {
                ContentStack.Children.Clear();
                var docs = await _generalService.GetLegalDocumentsAsync();
                var sections = await _generalService.GetLegalSectionsAsync();

                if (docs != null)
                {
                    var target = docs.FirstOrDefault(d => string.Equals(d.Type, docType, StringComparison.OrdinalIgnoreCase));
                    if (target != null)
                    {
                        TitleLabel.Text = target.Title ?? "Documento Legal";
                        var related = sections?.Where(s => s.Document == target.Id && s.Parent == null).OrderBy(s => s.Order);
                        if (related != null && related.Any())
                        {
                            foreach (var sec in related)
                            {
                                // Sección principal en un cuadro
                                var sectionFrame = new Frame { CornerRadius = 8, Padding = 12, BackgroundColor = Colors.White, BorderColor = Color.FromArgb("#e5e7eb"), HasShadow = true };
                                var sectionStack = new VerticalStackLayout { Spacing = 8 };
                                sectionStack.Children.Add(new Label { Text = sec.Title, FontAttributes = FontAttributes.Bold, FontSize = 16, TextColor = Colors.Black });
                                if (!string.IsNullOrEmpty(sec.Content))
                                {
                                    sectionStack.Children.Add(new Label { Text = sec.Content, LineBreakMode = LineBreakMode.WordWrap, FontSize = 14, TextColor = Colors.Black });
                                }

                                // Hijos
                                var children = sections.Where(s => s.Parent == sec.Id).OrderBy(s => s.Order);
                                foreach (var child in children)
                                {
                                    var childFrame = new Frame { CornerRadius = 6, Padding = 10, BackgroundColor = Color.FromArgb("#f9fafb"), BorderColor = Color.FromArgb("#e5e7eb"), HasShadow = false, Margin = new Thickness(0, 6, 0, 0) };
                                    var childStack = new VerticalStackLayout { Spacing = 4 };
                                    childStack.Children.Add(new Label { Text = (!string.IsNullOrEmpty(child.Code) ? child.Code + " " : string.Empty) + child.Title, FontAttributes = FontAttributes.Bold, TextColor = Colors.Black });
                                    if (!string.IsNullOrEmpty(child.Content))
                                    {
                                        childStack.Children.Add(new Label { Text = child.Content, LineBreakMode = LineBreakMode.WordWrap, FontSize = 13, TextColor = Colors.Black });
                                    }
                                    childFrame.Content = childStack;
                                    sectionStack.Children.Add(childFrame);
                                }

                                sectionFrame.Content = sectionStack;
                                ContentStack.Children.Add(sectionFrame);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ContentStack.Children.Add(new Label { Text = $"Error cargando documento: {ex.Message}", TextColor = Colors.Red });
            }
        }

        private void OnCloseClicked(object sender, EventArgs e)
        {
            Hide();
        }

        private void OnSizeChanged(object? sender, EventArgs e)
        {
            try
            {
                var w = Width;
                if (w <= 0) return;
                var title = FindByName("TitleLabel") as Label;
                var frame = FindByName("ModalFrame") as Frame;
                var scroll = FindByName("ContentScroll") as ScrollView;

                if (w <= 360)
                {
                    if (title != null) title.FontSize = 20;
                    if (frame != null) frame.WidthRequest = 320;
                    if (scroll != null) scroll.HeightRequest = 420;
                }
                else if (w <= 420)
                {
                    if (title != null) title.FontSize = 22;
                    if (frame != null) frame.WidthRequest = 360;
                    if (scroll != null) scroll.HeightRequest = 480;
                }
                else if (w <= 760)
                {
                    if (title != null) title.FontSize = 24;
                    if (frame != null) frame.WidthRequest = 600;
                    if (scroll != null) scroll.HeightRequest = 520;
                }
                else
                {
                    if (title != null) title.FontSize = 24;
                    if (frame != null) frame.WidthRequest = 760;
                    if (scroll != null) scroll.HeightRequest = 520;
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
