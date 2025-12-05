using System;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using AutogestionSena.MAUI.Api.Services;
using AutogestionSena.MAUI.Api.Dtos;
using System.Collections.Generic;

namespace AutogestionSena.MAUI.Views
{
    public partial class PrivacyModalView : ContentView
    {
        private readonly GeneralService _generalService;

        public PrivacyModalView()
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
                        TitleLabel.Text = string.IsNullOrWhiteSpace(target.Title) ? "Política de privacidad" : target.Title;
                        var related = sections?.Where(s => s.Document == target.Id && s.Parent == null).OrderBy(s => s.Order);
                        if (related != null && related.Any())
                        {
                            foreach (var sec in related)
                            {
                                var sectionFrame = new Frame { CornerRadius = 8, Padding = 12, BackgroundColor = Colors.White, BorderColor = Color.FromArgb("#e5e7eb"), HasShadow = true };
                                var sectionStack = new VerticalStackLayout { Spacing = 8 };
                                sectionStack.Children.Add(new Label { Text = sec.Title, FontAttributes = FontAttributes.Bold, FontSize = 18, TextColor = Colors.Black });
                                if (!string.IsNullOrEmpty(sec.Content))
                                {
                                    sectionStack.Children.Add(new Label { Text = sec.Content, LineBreakMode = LineBreakMode.WordWrap, FontSize = 14, TextColor = Colors.Black });
                                }

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
                            return;
                        }
                    }
                }

                TitleLabel.Text = "Política de privacidad";
                SubtitleLabel.Text = "Cómo el SENA protege y utiliza su información personal";

                AddSection("1. Información que recopilamos", new List<(string, string)>
                {
                    ("1.1. Información personal", "Nombres y apellidos completos\nNúmero de identificación\nFecha de nacimiento\nDirección de residencia\nCorreo electrónico\nNúmero de teléfono\nInformación académica y profesional\nEstado socioeconómico (cuando aplique)") ,
                    ("1.2. Información técnica", "Dirección IP\nTipo de navegador y versión\nSistema operativo\nPáginas visitadas y tiempo de permanencia\nCookies y tecnologías similares")
                });

                AddSection("2. Uso de la información", new List<(string, string)>
                {
                    ("2.1. Servicios educativos", "Gestión de inscripciones y matrículas\nSeguimiento académico y evaluación\nEmisión de certificados y títulos\nComunicación sobre programas y cursos"),
                    ("2.2. Servicios administrativos", "Verificación de identidad\nGestión de pagos (cuando aplique)\nSoporte técnico y atención al usuario\nCumplimiento de obligaciones legales"),
                    ("2.3. Mejora de servicios", "Análisis estadístico y de rendimiento\nPersonalización de la experiencia educativa\nDesarrollo de nuevos programas formativos\nInvestigación educativa institucional")
                });

                AddSection("3. Protección de datos", new List<(string, string)>
                {
                    ("3.1. Medidas técnicas", "Cifrado de datos en tránsito y en reposo\nFirewalls y sistemas de detección de intrusiones\nCopias de seguridad regulares\nActualizaciones de seguridad constantes\nControl de acceso basado en roles"),
                    ("3.2. Medidas organizativas", "Políticas internas de manejo de datos\nCapacitación del personal en protección de datos\nProcedimientos de respuesta a incidentes\nAuditorías regulares de seguridad\nAcuerdos de confidencialidad con terceros")
                });

                AddSimpleSection("4. Derechos sobre los datos", "Acceso: Conocer qué datos tenemos sobre usted\nRectificación: Corregir datos inexactos o incompletos\nActualización: Mantener sus datos actualizados\nSupresión: Solicitar la eliminación de sus datos (cuando sea posible)\nOposición: Oponerse al tratamiento de sus datos en ciertos casos\nPortabilidad: Obtener una copia de sus datos en formato estructurado\n\nPara ejercer estos derechos, puede contactarnos a través de los canales indicados al final de esta política.");

                AddSection("5. Compartir información", new List<(string, string)>
                {
                    ("5.1. Entidades Gubernamentales", "Con entidades del gobierno colombiano cuando sea requerido por ley o para cumplir con obligaciones regulatorias."),
                    ("5.2. Proveedores de Servicios", "Con proveedores de servicios tecnológicos bajo estrictos acuerdos de confidencialidad."),
                    ("5.3. Instituciones Educativas", "Con otras instituciones educativas para fines de articulación académica y reconocimiento de estudios.")
                });

                AddSimpleSection("6. Retención de datos", "Datos académicos: permanentes para efectos de certificación\nDatos de contacto: mientras mantenga relación activa con el SENA\nDatos técnicos: máximo 2 años\nDatos financieros: según legislación contable y tributaria");

                AddSimpleSection("7. Menores de edad", "Los menores de edad pueden utilizar nuestros servicios con el consentimiento de sus padres o tutores legales.\nMedidas adicionales:\n- Verificación del consentimiento parental\n- Limitación en la recopilación de datos personales\n- Supervisión adicional en el procesamiento de datos\n- Derechos especiales de eliminación de datos");

                var footer = new Label { Text = "Última actualización: diciembre de 2025", FontSize = 12, TextColor = Color.FromArgb("#6B7280"), HorizontalOptions = LayoutOptions.Center };
                ContentStack.Children.Add(footer);
            }
            catch (Exception ex)
            {
                await Application.Current?.MainPage?.DisplayAlert("Error", ex.Message, "Aceptar");
            }
        }

        private void AddSection(string title, List<(string subtitle, string content)> blocks)
        {
            var sectionFrame = new Frame { CornerRadius = 8, Padding = 12, BackgroundColor = Colors.White, BorderColor = Color.FromArgb("#e5e7eb"), HasShadow = true };
            var sectionStack = new VerticalStackLayout { Spacing = 8 };
            sectionStack.Children.Add(new Label { Text = title, FontAttributes = FontAttributes.Bold, FontSize = 18, TextColor = Colors.Black });
            foreach (var b in blocks)
            {
                var childFrame = new Frame { CornerRadius = 6, Padding = 10, BackgroundColor = Color.FromArgb("#f9fafb"), BorderColor = Color.FromArgb("#e5e7eb"), HasShadow = false, Margin = new Thickness(0, 6, 0, 0) };
                var childStack = new VerticalStackLayout { Spacing = 4 };
                childStack.Children.Add(new Label { Text = b.subtitle, FontAttributes = FontAttributes.Bold, TextColor = Colors.Black });
                childStack.Children.Add(new Label { Text = b.content, LineBreakMode = LineBreakMode.WordWrap, FontSize = 13, TextColor = Colors.Black });
                childFrame.Content = childStack;
                sectionStack.Children.Add(childFrame);
            }
            sectionFrame.Content = sectionStack;
            ContentStack.Children.Add(sectionFrame);
        }

        private void AddSimpleSection(string title, string content)
        {
            var sectionFrame = new Frame { CornerRadius = 8, Padding = 12, BackgroundColor = Colors.White, BorderColor = Color.FromArgb("#e5e7eb"), HasShadow = true };
            var sectionStack = new VerticalStackLayout { Spacing = 8 };
            sectionStack.Children.Add(new Label { Text = title, FontAttributes = FontAttributes.Bold, FontSize = 18, TextColor = Colors.Black });
            sectionStack.Children.Add(new Label { Text = content, LineBreakMode = LineBreakMode.WordWrap, FontSize = 14, TextColor = Colors.Black });
            sectionFrame.Content = sectionStack;
            ContentStack.Children.Add(sectionFrame);
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

                // Adjust font sizes
                if (w <= 360)
                {
                    TitleLabel.FontSize = 20;
                    SubtitleLabel.FontSize = 11;
                }
                else if (w <= 420)
                {
                    TitleLabel.FontSize = 22;
                    SubtitleLabel.FontSize = 12;
                }
                else
                {
                    TitleLabel.FontSize = 24;
                    SubtitleLabel.FontSize = 12;
                }

                // Adjust modal proportional size using AbsoluteLayout bounds
                if (ModalFrame != null)
                {
                    if (w <= 360)
                    {
                        AbsoluteLayout.SetLayoutBounds(ModalFrame, new Rect(0.5, 0.5, 0.95, 0.9));
                    }
                    else if (w <= 420)
                    {
                        AbsoluteLayout.SetLayoutBounds(ModalFrame, new Rect(0.5, 0.5, 0.9, 0.85));
                    }
                    else if (w <= 760)
                    {
                        AbsoluteLayout.SetLayoutBounds(ModalFrame, new Rect(0.5, 0.5, 0.8, 0.8));
                    }
                    else
                    {
                        AbsoluteLayout.SetLayoutBounds(ModalFrame, new Rect(0.5, 0.5, 0.7, 0.8));
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
