using System;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using AutogestionSena.MAUI.Api.Dtos;
using AutogestionSena.MAUI.Api.Services;
using AutogestionSena.MAUI.Api;

namespace AutogestionSenaMaui.ViewModels
{
    public class ApprenticeDashboardViewModel : BindableObject
    {
        private readonly AssignationService _assignService;
        private readonly ApiService _apiService;
        private bool _isLoading;
        private ApprenticeDashboardDto? _dashboard;
        private int _apprenticesCount;
        private int _unassignedRequestsCount;
        private int _assignedRequestsCount;
        private string _apprenticeName = "Aprendiz";
        private string _diagnosticMessage = "";
        private bool _showDiagnosticMessage;

        public ApprenticeDashboardViewModel()
        {
            _assignService = new AssignationService();
            _apiService = new ApiService();
            RefreshCommand = new Command(async () => await LoadAsync(ApprenticeId));
            OpenPdfCommand = new Command(async () => await OpenPdfAsync());
            DiagnoseConnectionCommand = new Command(async () => await DiagnoseConnectionAsync());
            RequestActionCommand = new Command(async () => await OnRequestActionClickedAsync());
            LoadApprenticeMetadata();
        }

        public int ApprenticesCount
        {
            get => _apprenticesCount;
            set { _apprenticesCount = value; OnPropertyChanged(); }
        }

        public int UnassignedRequestsCount
        {
            get => _unassignedRequestsCount;
            set { _unassignedRequestsCount = value; OnPropertyChanged(); }
        }

        public int AssignedRequestsCount
        {
            get => _assignedRequestsCount;
            set { _assignedRequestsCount = value; OnPropertyChanged(); }
        }

        public int ApprenticeId { get; set; }

        public ApprenticeDashboardDto? Dashboard
        {
            get => _dashboard;
            set
            {
                _dashboard = value;
                OnPropertyChanged();
                RaiseDashboardDerivedProperties();
            }
        }

        public string DiagnosticMessage
        {
            get => _diagnosticMessage;
            set { _diagnosticMessage = value; OnPropertyChanged(); }
        }

        public bool ShowDiagnosticMessage
        {
            get => _showDiagnosticMessage;
            set { _showDiagnosticMessage = value; OnPropertyChanged(); }
        }

        public bool HasRequest => Dashboard?.HasRequest ?? false;

        public bool ShowRequestDetail => Dashboard?.HasRequest ?? false;

        public bool ShowEmptyRequestState => !ShowRequestDetail;

        public string ApprenticeName
        {
            get => _apprenticeName;
            set { _apprenticeName = value; OnPropertyChanged(); }
        }

        public string RequestStateLabel => Dashboard?.Request?.StateDisplay ?? Dashboard?.RequestState ?? "Sin solicitudes registradas";

        public string RequestStateColor => Dashboard?.Request?.StateColor ?? "#B8B8B8";

        public string RequestStateDescription => HasRequest
            ? $"Tu solicitud se encuentra en estado {RequestStateLabel}."
            : "Aún no has solicitado ningún proceso para tu etapa productiva. Por favor registra una solicitud.";

        public string RequestActionText => HasRequest ? "Ver solicitud" : "Hacer una solicitud";

        public string EnterpriseName => Dashboard?.Request?.EnterpriseName ?? "Sin empresa registrada";

        public string BossName => Dashboard?.Request?.BossName ?? "Sin jefe registrado";

        public string ModalityDisplay => Dashboard?.Request?.Modality ?? "Sin modalidad";

        public string RequestDateDisplay => FormatDate(Dashboard?.Request?.RequestDate);

        public string StartDateDisplay => FormatDate(Dashboard?.Request?.StartDate);

        public string EndDateDisplay => FormatDate(Dashboard?.Request?.EndDate);

        public string CityDisplay => Dashboard?.Request?.CityName ?? "Ciudad no registrada";

        public bool ShowPdfButton => Dashboard?.Request?.HasPdf ?? false;

        public string RequestPdfUrl => Dashboard?.Request?.PdfUrl ?? string.Empty;

        public string InstructorFullName
        {
            get
            {
                var instructor = Dashboard?.Instructor;
                if (instructor == null)
                {
                    return "Instructor no asignado";
                }

                var fullName = $"{instructor.FirstName} {instructor.SecondName ?? string.Empty} {instructor.FirstLastName} {instructor.SecondLastName ?? string.Empty}".Trim();
                return string.IsNullOrWhiteSpace(fullName) ? "Instructor asignado" : fullName;
            }
        }

        public bool HasInstructor => Dashboard?.Instructor != null;

        public bool ShowInstructorCard => Dashboard?.Instructor != null;

        public bool ShowInstructorContact => Dashboard?.ShowInstructor ?? false;

        public string InstructorKnowledgeArea => Dashboard?.Instructor?.KnowledgeArea ?? "Área no registrada";

        public string InstructorContactEmail => Dashboard?.Instructor?.Email ?? "Sin correo";

        public string InstructorContactPhone => Dashboard?.Instructor?.Phone ?? "Sin teléfono";

        public string InstructorAssignedAtDisplay => FormatDate(Dashboard?.Instructor?.AssignedAt);

        public string InstructorTagText => ShowInstructorCard ? "Asignado" : "Pendiente de asignar";

        public string InstructorTagBackground => ShowInstructorCard ? "#DCFCE7" : "#E5E7EB";

        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public ICommand RefreshCommand { get; }

        public ICommand OpenPdfCommand { get; }

        public ICommand DiagnoseConnectionCommand { get; }

        public ICommand RequestActionCommand { get; }

        public async Task LoadAsync(int apprenticeId)
        {
            try
            {
                IsLoading = true;
                ShowDiagnosticMessage = false;

                var (isConnected, message) = await _apiService.DiagnoseConnectionAsync();

                if (!isConnected)
                {
                    DiagnosticMessage = $"⚠️ Problema de conexión: {message}";
                    ShowDiagnosticMessage = true;
                    Dashboard = new ApprenticeDashboardDto
                    {
                        HasRequest = false,
                        RequestState = "Error de conexión",
                        ShowInstructor = false
                    };
                    return;
                }

                await Task.Delay(500);

                var result = await _assignService.GetApprenticeDashboardAsync(apprenticeId);

                Dashboard = result;
                ApprenticeId = apprenticeId;

                if (Dashboard != null)
                {
                    ApprenticesCount = 125000000;
                    UnassignedRequestsCount = 20;
                    AssignedRequestsCount = 30;
                }
            }
            catch (Exception ex)
            {
                DiagnosticMessage = $"❌ Error: {ex.Message}";
                ShowDiagnosticMessage = true;

                Dashboard = new ApprenticeDashboardDto
                {
                    HasRequest = false,
                    RequestState = "Error al cargar datos",
                    ShowInstructor = false
                };
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task DiagnoseConnectionAsync()
        {
            try
            {
                DiagnosticMessage = "🔍 Diagnosticando conexión...";
                ShowDiagnosticMessage = true;

                var (isConnected, message) = await _apiService.DiagnoseConnectionAsync();

                DiagnosticMessage = isConnected ? $"✅ {message}" : $"❌ {message}";

#if ANDROID
                var platformInfo = "📱 Plataforma: Android";
#elif IOS
                var platformInfo = "📱 Plataforma: iOS";
#else
                var platformInfo = "📱 Plataforma: Windows";
#endif
                var networkInfo = $"\n🌐 URL: {Endpoints.API_BASE_URL}";
                DiagnosticMessage += $"\n{platformInfo}" + networkInfo;
            }
            catch (Exception ex)
            {
                DiagnosticMessage = $"❌ Error en diagnóstico: {ex.Message}";
            }
        }

        private void LoadApprenticeMetadata()
        {
            try
            {
                var userDataRaw = Preferences.Get("user_data", string.Empty);
                if (!string.IsNullOrWhiteSpace(userDataRaw))
                {
                    using var document = JsonDocument.Parse(userDataRaw);
                    if (document.RootElement.TryGetProperty("firstName", out var firstName) &&
                        !string.IsNullOrWhiteSpace(firstName.GetString()))
                    {
                        ApprenticeName = firstName.GetString()!;
                        return;
                    }
                }
            }
            catch (Exception)
            {
                // Error parsing user_data handled silently
            }

            var fallback = Preferences.Get("UserEmail", string.Empty);
            ApprenticeName = string.IsNullOrWhiteSpace(fallback) ? "Aprendiz" : fallback;
        }

        private void RaiseDashboardDerivedProperties()
        {
            OnPropertyChanged(nameof(HasRequest));
            OnPropertyChanged(nameof(ShowRequestDetail));
            OnPropertyChanged(nameof(ShowEmptyRequestState));
            OnPropertyChanged(nameof(RequestStateLabel));
            OnPropertyChanged(nameof(RequestStateColor));
            OnPropertyChanged(nameof(RequestStateDescription));
            OnPropertyChanged(nameof(RequestActionText));
            OnPropertyChanged(nameof(EnterpriseName));
            OnPropertyChanged(nameof(BossName));
            OnPropertyChanged(nameof(ModalityDisplay));
            OnPropertyChanged(nameof(RequestDateDisplay));
            OnPropertyChanged(nameof(StartDateDisplay));
            OnPropertyChanged(nameof(EndDateDisplay));
            OnPropertyChanged(nameof(CityDisplay));
            OnPropertyChanged(nameof(ShowPdfButton));
            OnPropertyChanged(nameof(RequestPdfUrl));
            OnPropertyChanged(nameof(InstructorFullName));
            OnPropertyChanged(nameof(HasInstructor));
            OnPropertyChanged(nameof(ShowInstructorCard));
            OnPropertyChanged(nameof(ShowInstructorContact));
            OnPropertyChanged(nameof(InstructorKnowledgeArea));
            OnPropertyChanged(nameof(InstructorContactEmail));
            OnPropertyChanged(nameof(InstructorContactPhone));
            OnPropertyChanged(nameof(InstructorAssignedAtDisplay));
            OnPropertyChanged(nameof(InstructorTagText));
            OnPropertyChanged(nameof(InstructorTagBackground));
        }

        private static string FormatDate(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "Sin definir";
            }

            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var parsed))
            {
                return parsed.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            }

            return value;
        }

        private async Task OpenPdfAsync()
        {
            if (!ShowPdfButton)
            {
                return;
            }

            try
            {
                if (Uri.TryCreate(RequestPdfUrl, UriKind.Absolute, out var uri))
                {
                    await Launcher.OpenAsync(uri);
                }
            }
            catch (Exception)
            {
                // Error opening PDF handled silently
            }
        }

        /// <summary>
        /// Maneja el clic en el botón de acción de solicitud
        /// </summary>
        private async Task OnRequestActionClickedAsync()
        {
            if (HasRequest)
            {
                // Si ya tiene una solicitud, podría navegar a los detalles
                // Por ahora solo muestra un mensaje
                await Application.Current!.MainPage!.DisplayAlert(
                    "Solicitud Activa",
                    "Ya tienes una solicitud registrada. Puedes ver los detalles en esta misma pantalla.",
                    "Entendido");
            }
            else
            {
                // Si no tiene solicitud, mostrar mensaje de que debe hacerlo desde la web
                await Application.Current!.MainPage!.DisplayAlert(
                    "Realizar Solicitud",
                    "Para realizar una nueva solicitud de etapa productiva, debes acceder a la plataforma web de Autogestión SENA.\n\n" +
                    "📌 Ingresa a: http://167.114.98.199:81/\n\n" +
                    "Desde allí podrás registrar tu solicitud con todos los documentos requeridos.",
                    "Entendido");
            }
        }
    }
}
