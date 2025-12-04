using System;
using System.Text.RegularExpressions;
using Microsoft.Maui.Controls;
using AutogestionSena.MAUI.Api.Dtos;
using AutogestionSena.MAUI.Api.Services;

namespace AutogestionSena.MAUI.Views
{

    public partial class RegisterPage : ContentPage
    {
        private readonly UserService _userService;
        public List<DocumentTypeDto> DocumentTypes { get; set; } = new();

        public RegisterPage()
        {
            InitializeComponent();
            _userService = new UserService();
            BindingContext = this;
            LoadDocumentTypes();
        }

        private async void LoadDocumentTypes()
        {
            try
            {
                var documentTypes = await _userService.GetDocumentTypesAsync();
                if (documentTypes != null && documentTypes.Count > 0)
                {
                    DocumentTypes = documentTypes;
                    TipoDocumentoPicker.ItemsSource = DocumentTypes;
                }
                else
                {
                    // Si no hay conexión o no hay datos, usar datos locales
                    LoadLocalDocumentTypes();
                }
            }
            catch (Exception ex)
            {
                // Si hay error de conexión, cargar datos locales sin mostrar alerta
                LoadLocalDocumentTypes();
                
                // Mostrar mensaje discreto
                Console.WriteLine($"No se pudo conectar al servidor: {ex.Message}");
            }
        }

        private void LoadLocalDocumentTypes()
        {
            // Tipos de documento según el API real
            DocumentTypes = new List<DocumentTypeDto>
            {
                new DocumentTypeDto { Id = 1, Name = "Cédula de Ciudadanía", Acronyms = "CC", Active = true },
                new DocumentTypeDto { Id = 2, Name = "Tarjeta de Identidad", Acronyms = "TI", Active = true },
                new DocumentTypeDto { Id = 3, Name = "Cédula de Extranjería", Acronyms = "CE", Active = true },
                new DocumentTypeDto { Id = 4, Name = "Pasaporte", Acronyms = "PASSPORT", Active = true },
                new DocumentTypeDto { Id = 5, Name = "Número ciego - SENA", Acronyms = "NUMERO_CIEGO_SENA", Active = true },
                new DocumentTypeDto { Id = 6, Name = "Documento Nacional de Identificación", Acronyms = "DNI", Active = true },
                new DocumentTypeDto { Id = 7, Name = "Número de Identificación Tributaria", Acronyms = "NIT", Active = true },
                new DocumentTypeDto { Id = 8, Name = "Permiso por Protección Temporal", Acronyms = "PERMISO_TEMPORAL", Active = true }
            };
            TipoDocumentoPicker.ItemsSource = DocumentTypes;
        }

        /// <summary>
        /// Valida el formato del correo institucional SENA
        /// </summary>
        private bool ValidateEmail(string email, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(email))
            {
                errorMessage = "El correo institucional es obligatorio.";
                return false;
            }

            // Validar formato de email
            var emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
            if (!emailRegex.IsMatch(email))
            {
                errorMessage = "El formato del correo no es válido.";
                return false;
            }

            // Validar que sea correo institucional SENA
            if (!email.ToLower().EndsWith("@soy.sena.edu.co") && 
                !email.ToLower().EndsWith("@sena.edu.co") &&
                !email.ToLower().EndsWith("@misena.edu.co"))
            {
                errorMessage = "Debe usar un correo institucional SENA (@soy.sena.edu.co, @sena.edu.co o @misena.edu.co).";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Valida los nombres (solo letras y espacios)
        /// </summary>
        private bool ValidateNames(string nombres, string apellidos, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(nombres))
            {
                errorMessage = "Los nombres son obligatorios.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(apellidos))
            {
                errorMessage = "Los apellidos son obligatorios.";
                return false;
            }

            // Solo letras, espacios y tildes
            var nameRegex = new Regex(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]+$");

            if (!nameRegex.IsMatch(nombres))
            {
                errorMessage = "Los nombres solo deben contener letras.";
                return false;
            }

            if (!nameRegex.IsMatch(apellidos))
            {
                errorMessage = "Los apellidos solo deben contener letras.";
                return false;
            }

            if (nombres.Length < 2 || nombres.Length > 50)
            {
                errorMessage = "Los nombres deben tener entre 2 y 50 caracteres.";
                return false;
            }

            if (apellidos.Length < 2 || apellidos.Length > 50)
            {
                errorMessage = "Los apellidos deben tener entre 2 y 50 caracteres.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Valida el número de documento
        /// </summary>
        private bool ValidateDocument(string documento, DocumentTypeDto? tipoDocumento, out string errorMessage, out int documentoNumero)
        {
            errorMessage = string.Empty;
            documentoNumero = 0;

            if (tipoDocumento == null)
            {
                errorMessage = "Debes seleccionar un tipo de documento.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(documento))
            {
                errorMessage = "El número de documento es obligatorio.";
                return false;
            }

            // Solo números
            if (!Regex.IsMatch(documento, @"^\d+$"))
            {
                errorMessage = "El número de documento solo debe contener números.";
                return false;
            }

            // Validar longitud según tipo de documento
            var docLength = documento.Length;
            var tipoAcronym = tipoDocumento.Acronyms?.ToUpper() ?? "";

            switch (tipoAcronym)
            {
                case "CC": // Cédula de Ciudadanía: 6-10 dígitos
                    if (docLength < 6 || docLength > 10)
                    {
                        errorMessage = "La Cédula de Ciudadanía debe tener entre 6 y 10 dígitos.";
                        return false;
                    }
                    break;
                case "TI": // Tarjeta de Identidad: 10-11 dígitos
                    if (docLength < 10 || docLength > 11)
                    {
                        errorMessage = "La Tarjeta de Identidad debe tener entre 10 y 11 dígitos.";
                        return false;
                    }
                    break;
                case "CE": // Cédula de Extranjería: 6-7 dígitos
                    if (docLength < 6 || docLength > 7)
                    {
                        errorMessage = "La Cédula de Extranjería debe tener entre 6 y 7 dígitos.";
                        return false;
                    }
                    break;
                default: // Otros documentos: 5-15 dígitos
                    if (docLength < 5 || docLength > 15)
                    {
                        errorMessage = "El número de documento debe tener entre 5 y 15 dígitos.";
                        return false;
                    }
                    break;
            }

            // Validar que el número no exceda el límite de int (2147483647)
            if (!int.TryParse(documento, out documentoNumero))
            {
                errorMessage = "El número de documento excede el límite permitido.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Valida el número de teléfono colombiano
        /// </summary>
        private bool ValidatePhone(string telefono, out string errorMessage, out long telefonoNumero)
        {
            errorMessage = string.Empty;
            telefonoNumero = 0;

            if (string.IsNullOrWhiteSpace(telefono))
            {
                errorMessage = "El teléfono es obligatorio.";
                return false;
            }

            // Solo números
            if (!Regex.IsMatch(telefono, @"^\d+$"))
            {
                errorMessage = "El teléfono solo debe contener números.";
                return false;
            }

            // Teléfono colombiano: 10 dígitos, empieza con 3
            if (telefono.Length != 10)
            {
                errorMessage = "El teléfono debe tener 10 dígitos.";
                return false;
            }

            if (!telefono.StartsWith("3"))
            {
                errorMessage = "El número de celular colombiano debe empezar con 3.";
                return false;
            }

            if (!long.TryParse(telefono, out telefonoNumero))
            {
                errorMessage = "El número de teléfono no es válido.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Valida los términos y condiciones
        /// </summary>
        private bool ValidateTerms(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!TermsCheckbox.IsChecked)
            {
                errorMessage = "Debes aceptar los términos y condiciones.";
                return false;
            }

            return true;
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            var email = EmailEntry.Text?.Trim();
            var nombres = NombresEntry.Text?.Trim();
            var apellidos = ApellidosEntry.Text?.Trim();
            var documento = DocumentoEntry.Text?.Trim();
            var telefono = TelefonoEntry.Text?.Trim();
            var tipoDocumento = TipoDocumentoPicker.SelectedItem as DocumentTypeDto;

            // Validar correo
            if (!ValidateEmail(email ?? "", out var emailError))
            {
                await DisplayAlert("Error", emailError, "Aceptar");
                EmailEntry.Focus();
                return;
            }

            // Validar nombres
            if (!ValidateNames(nombres ?? "", apellidos ?? "", out var namesError))
            {
                await DisplayAlert("Error", namesError, "Aceptar");
                if (string.IsNullOrWhiteSpace(nombres))
                    NombresEntry.Focus();
                else
                    ApellidosEntry.Focus();
                return;
            }

            // Validar documento
            if (!ValidateDocument(documento ?? "", tipoDocumento, out var docError, out var documentoNumero))
            {
                await DisplayAlert("Error", docError, "Aceptar");
                if (tipoDocumento == null)
                    TipoDocumentoPicker.Focus();
                else
                    DocumentoEntry.Focus();
                return;
            }

            // Validar teléfono
            if (!ValidatePhone(telefono ?? "", out var phoneError, out var telefonoNumero))
            {
                await DisplayAlert("Error", phoneError, "Aceptar");
                TelefonoEntry.Focus();
                return;
            }

            // Validar términos
            if (!ValidateTerms(out var termsError))
            {
                await DisplayAlert("Error", termsError, "Aceptar");
                return;
            }

            try
            {
                // Separar nombres y apellidos si tienen espacios
                var nombreParts = nombres!.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                var apellidoParts = apellidos!.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

                // Construir el payload de registro según la API
                var registerPayload = new RegisterPayloadDto
                {
                    Email = email,
                    FirstName = nombreParts.Length > 0 ? nombreParts[0] : nombres,
                    SecondName = nombreParts.Length > 1 ? nombreParts[1] : null,
                    FirstLastName = apellidoParts.Length > 0 ? apellidoParts[0] : apellidos,
                    SecondLastName = apellidoParts.Length > 1 ? apellidoParts[1] : null,
                    TypeIdentification = tipoDocumento!.Id,
                    NumberIdentification = documentoNumero,
                    PhoneNumber = telefonoNumero
                };

                // Mostrar indicador de carga
                RegisterButton.IsEnabled = false;
                RegisterButton.Text = "Registrando...";

                // Llamar a la API de registro
                var resultado = await _userService.RegisterApprenticeAsync(registerPayload);

                if (resultado != null && resultado.Success)
                {
                    await DisplayAlert("Éxito", "Registro exitoso. Revisa tu correo para activar tu cuenta.", "Continuar");
                    await Shell.Current.GoToAsync("///LoginPage");
                }
                else
                {
                    var errorMsg = resultado?.Detail ?? "No se pudo registrar el usuario.";
                    await DisplayAlert("Error", errorMsg, "Aceptar");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "Aceptar");
            }
            finally
            {
                RegisterButton.IsEnabled = true;
                RegisterButton.Text = "Registrarse";
            }
        }

        private async void OnBackToLoginClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///LoginPage");
        }

        private async void OnTermsClicked(object sender, EventArgs e)
        {
            // Mostrar los términos y condiciones
            bool accepted = await DisplayAlert(
                "Términos y Condiciones",
                "Al registrarte en AutoGestión CIES aceptas:\n\n" +
                "1. Proporcionar información veraz y actualizada.\n" +
                "2. Usar la plataforma únicamente para fines educativos.\n" +
                "3. Proteger tus credenciales de acceso.\n" +
                "4. Respetar las políticas del SENA.\n" +
                "5. No compartir información confidencial.\n\n" +
                "¿Aceptas los términos y condiciones?",
                "Aceptar",
                "Cancelar");

            if (accepted)
            {
                TermsCheckbox.IsChecked = true;
            }
        }
    }
}
