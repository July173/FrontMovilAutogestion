using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using AutogestionSena.MAUI.Api.Services;

namespace AutogestionSenaMaui.ViewModels;

public class DashboardCardsViewModel : BindableObject
{
    private readonly ApprenticeSimpleService _apprenticeService;
    private readonly AssignmentService _assignmentService;
    private int _apprenticesCount;
    private int _unassignedRequestsCount;
    private int _assignedRequestsCount;
    private bool _isLoading;

    public int ApprenticesCount { get => _apprenticesCount; set { _apprenticesCount = value; OnPropertyChanged(); } }
    public int UnassignedRequestsCount { get => _unassignedRequestsCount; set { _unassignedRequestsCount = value; OnPropertyChanged(); } }
    public int AssignedRequestsCount { get => _assignedRequestsCount; set { _assignedRequestsCount = value; OnPropertyChanged(); } }
    public bool IsLoading { get => _isLoading; set { _isLoading = value; OnPropertyChanged(); } }

    public DashboardCardsViewModel()
    {
        _apprenticeService = new ApprenticeSimpleService();
        _assignmentService = new AssignmentService();
    }

    public async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            // Configurar token de autenticaci�n si existe
            var authToken = Preferences.Get("AuthToken", string.Empty);
            if (!string.IsNullOrEmpty(authToken))
            {
                _apprenticeService.SetAuthToken(authToken);
                _assignmentService.SetAuthToken(authToken);
            }

            // Cargar datos en paralelo
            var apprenticesTask = _apprenticeService.GetAllApprenticesAsync();
            var assignmentsTask = _assignmentService.GetFormRequestListAsync();

            await Task.WhenAll(apprenticesTask, assignmentsTask);

            // Procesar aprendices
            var apprentices = await apprenticesTask;
            if (apprentices != null)
            {
                var activeCount = apprentices.Count(a => a.Active);
                ApprenticesCount = activeCount;
            }
            else
            {
                ApprenticesCount = 0;
            }

            // Procesar asignaciones
            var assignments = await assignmentsTask;
            if (assignments != null && assignments.Success && assignments.Data != null)
            {
                UnassignedRequestsCount = assignments.Data.Count(a => a.RequestState == "SIN_ASIGNAR");
                AssignedRequestsCount = assignments.Data.Count(a => a.RequestState == "ASIGNADO" || a.RequestState == "APROBADO");
            }
            else
            {
                UnassignedRequestsCount = 0;
                AssignedRequestsCount = 0;
            }
        }
        catch (Exception)
        {
            ApprenticesCount = 0;
            UnassignedRequestsCount = 0;
            AssignedRequestsCount = 0;
        }
        finally
        {
            IsLoading = false;
        }
    }
}
