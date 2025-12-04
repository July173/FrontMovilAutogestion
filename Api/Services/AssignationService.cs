using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutogestionSena.MAUI.Api.Dtos;
using AutogestionSena.MAUI.Api;

namespace AutogestionSena.MAUI.Api.Services
{
    public class AssignationService
    {
        private readonly ApiService _apiService;
        public AssignationService() => _apiService = new ApiService();

        /// <summary>
        /// Obtiene el dashboard del aprendiz
        /// Endpoint: GET assign/request_asigation/aprendiz-dashboard/?aprendiz_id={id}
        /// NOTA: El API devuelve DOS formatos diferentes:
        /// 1. Array [{}] cuando NO hay instructor asignado (PRE-APROBADO)
        /// 2. Objeto {} cuando SÍ hay instructor asignado (VERIFICANDO, etc.)
        /// </summary>
        public async Task<ApprenticeDashboardDto> GetApprenticeDashboardAsync(int apprenticeId)
        {
            try
            {
                var endpoint = $"assign/request_asignation/aprendiz-dashboard/?aprendiz_id={apprenticeId}";

                var jsonResponse = await _apiService.GetRawAsync(endpoint);
                
                if (string.IsNullOrEmpty(jsonResponse))
                {
                    return CreateEmptyDashboard();
                }

                var trimmedJson = jsonResponse.Trim();
                
                if (trimmedJson.StartsWith("["))
                {
                    return await ProcessArrayResponse(trimmedJson);
                }
                else if (trimmedJson.StartsWith("{"))
                {
                    return await ProcessObjectResponse(trimmedJson);
                }
                else
                {
                    return CreateEmptyDashboard();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Procesa la respuesta cuando es un ARRAY (sin instructor asignado)
        /// </summary>
        private async Task<ApprenticeDashboardDto> ProcessArrayResponse(string json)
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var responses = JsonSerializer.Deserialize<List<ApprenticeDashboardBasicApiResponse>>(json, options);
                
                if (responses == null || responses.Count == 0)
                {
                    return CreateEmptyDashboard();
                }

                var response = responses.FirstOrDefault();
                
                if (response == null)
                {
                    return CreateEmptyDashboard();
                }

                return await MapBasicDashboardResponse(response);
            }
            catch (JsonException)
            {
                return CreateEmptyDashboard();
            }
        }

        /// <summary>
        /// Procesa la respuesta cuando es un OBJETO (con instructor asignado)
        /// </summary>
        private async Task<ApprenticeDashboardDto> ProcessObjectResponse(string json)
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var response = JsonSerializer.Deserialize<ApprenticeDashboardRealApiResponse>(json, options);
                
                if (response == null)
                {
                    return CreateEmptyDashboard();
                }

                return await MapRealDashboardResponse(response);
            }
            catch (JsonException)
            {
                return CreateEmptyDashboard();
            }
        }

        /// <summary>
        /// Crea un dashboard vacío cuando no hay datos
        /// </summary>
        private ApprenticeDashboardDto CreateEmptyDashboard()
        {
            return new ApprenticeDashboardDto
            {
                HasRequest = false,
                RequestState = "Sin solicitudes registradas",
                ShowInstructor = false
            };
        }

        /// <summary>
        /// Mapea la respuesta básica (sin instructor) al DTO del dashboard
        /// </summary>
        private async Task<ApprenticeDashboardDto> MapBasicDashboardResponse(ApprenticeDashboardBasicApiResponse response)
        {
            EnterpriseDto? enterprise = null;
            if (response.Enterprise > 0)
            {
                try
                {
                    enterprise = await GetEnterpriseAsync(response.Enterprise);
                }
                catch (Exception)
                {
                    // Ignore enterprise fetch errors
                }
            }

            ModalityProductiveStageDto? modality = null;
            if (response.ModalityProductiveStage > 0)
            {
                try
                {
                    modality = await GetModalityByIdAsync(response.ModalityProductiveStage);
                }
                catch (Exception)
                {
                    // Ignore modality fetch errors
                }
            }

            var request = new RequestDto
            {
                Id = response.Id,
                EnterpriseName = enterprise?.Name ?? "Empresa no encontrada",
                BossName = enterprise?.ImmediateBoss ?? "Pendiente de asignación",
                Modality = modality?.Name ?? "N/A",
                StartDate = response.StartDate ?? response.DateStartProductionStage,
                EndDate = response.EndDate,
                RequestDate = response.RequestDate,
                RequestState = response.RequestState ?? "PENDIENTE",
                PdfUrl = response.PdfUrl ?? response.PdfRequest,
                CityName = enterprise?.Address ?? "N/A",
                StateDisplay = GetStateDisplay(response.RequestState),
                StateColor = GetStateColor(response.RequestState),
                StateCode = response.RequestState
            };

            return new ApprenticeDashboardDto
            {
                HasRequest = true,
                Request = request,
                Instructor = null, // Sin instructor en este estado
                RequestState = request.RequestState,
                ShowInstructor = false
            };
        }

    /// <summary>
    /// Mapea la respuesta real del API al DTO del dashboard
/// </summary>
     private async Task<ApprenticeDashboardDto> MapRealDashboardResponse(ApprenticeDashboardRealApiResponse? response)
     {
  const string defaultState = "Sin solicitudes registradas";

      if (response == null)
            {
 return new ApprenticeDashboardDto
          {
    HasRequest = false,
         RequestState = defaultState,
        ShowInstructor = false
         };
            }
      
      // Obtener datos adicionales de empresa si es necesario
        EnterpriseDto? enterprise = null;
        if (response.Enterprise > 0)
      {
            try
       {
       enterprise = await GetEnterpriseAsync(response.Enterprise);
       }
     catch (Exception)
        {
       // Ignore enterprise fetch errors
  }
    }

      // Obtener datos adicionales de modalidad si es necesario
      ModalityProductiveStageDto? modality = null;
      if (response.ModalityProductiveStage > 0)
      {
          try
          {
              modality = await GetModalityByIdAsync(response.ModalityProductiveStage);
          }
          catch (Exception)
          {
              // Ignore modality fetch errors
          }
      }

      // Crear el DTO de solicitud
    var request = new RequestDto
       {
Id = response.Id,
            EnterpriseName = enterprise?.Name ?? "Empresa no encontrada",
       BossName = enterprise?.ImmediateBoss ?? "No especificado",
         Modality = modality?.Name ?? "N/A",
        StartDate = response.StartDate,
       EndDate = response.EndDate,
  RequestDate = response.RequestDate,
          RequestState = response.RequestState ?? defaultState,
    PdfUrl = response.PdfUrl,
      CityName = enterprise?.Address ?? "N/A",
    StateDisplay = response.RequestState ?? defaultState,
             StateColor = GetStateColor(response.RequestState),
StateCode = response.RequestState
      };

    // Crear el DTO de instructor si existe
 DashboardInstructorDto? instructor = null;
    if (response.InstructorId.HasValue && response.InstructorId > 0)
 {
   instructor = new DashboardInstructorDto
     {
      Id = response.InstructorId.Value,
      FirstName = response.InstructorFirstName,
      SecondName = response.InstructorSecondName,
  FirstLastName = response.InstructorFirstLastName,
    SecondLastName = response.InstructorSecondLastName,
       Email = response.InstructorEmail,
   Phone = response.InstructorPhoneNumber?.ToString(),
   KnowledgeArea = response.InstructorKnowledgeArea,
     AssignedAt = DateTime.Now.ToString("yyyy-MM-dd"), // Temporal
  ShowContact = true
      };
 }

   var result = new ApprenticeDashboardDto
     {
          HasRequest = true,
     Request = request,
      Instructor = instructor,
    RequestState = request.RequestState,
         ShowInstructor = instructor != null
     };

            return result;
        }

        /// <summary>
        /// Obtiene una modalidad por ID de la lista completa
        /// </summary>
        private async Task<ModalityProductiveStageDto?> GetModalityByIdAsync(int modalityId)
        {
            try
            {
                var modalities = await GetModalityProductiveStagesAsync();
                return modalities?.FirstOrDefault(m => m.Id == modalityId);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Obtiene el color del estado según el código
        /// Estados del sistema:
        /// - RECHAZADO: cuando el coordinador rechaza la solicitud
        /// - ASIGNADO: cuando el coordinador asigna un instructor para seguimiento
        /// - ASIGNAR: cuando la solicitud aún no tiene instructor asignado y está pendiente
        /// - VERIFICANDO: cuando se ha asignado instructor para valoración pero no la ha realizado
        /// - PRE-APROBADO: cuando el instructor ya realizó la valoración y el coordinador debe decidir
        /// </summary>
        private static string GetStateColor(string? state)
        {
            return state?.ToUpper() switch
            {
                "RECHAZADO" => "#EF4444",      // Rojo - Solicitud rechazada
                "ASIGNADO" => "#10B981",       // Verde - Instructor asignado para seguimiento
                "ASIGNAR" => "#F59E0B",        // Amarillo - Pendiente de asignar instructor
                "VERIFICANDO" => "#3B82F6",   // Azul - Instructor valorando
                "PRE-APROBADO" => "#8B5CF6",  // Púrpura - Valoración realizada, esperando coordinador
                "APROBADO" => "#10B981",       // Verde - Aprobado
                "EN_PROCESO" => "#3B82F6",     // Azul - En seguimiento
                "FINALIZADO" => "#6B7280",     // Gris - Terminado
                "CANCELADO" => "#EF4444",      // Rojo - Cancelado
                _ => "#CBD5E1"                 // Gris claro por defecto
            };
        }

        /// <summary>
        /// Obtiene el texto legible del estado
        /// </summary>
        private static string GetStateDisplay(string? state)
        {
            return state?.ToUpper() switch
            {
                "RECHAZADO" => "Rechazado",
                "ASIGNADO" => "Instructor Asignado",
                "ASIGNAR" => "Pendiente de Asignación",
                "VERIFICANDO" => "En Verificación",
                "PRE-APROBADO" => "Pre-aprobado (Esperando Decisión)",
                "APROBADO" => "Aprobado",
                "EN_PROCESO" => "En Proceso",
                "FINALIZADO" => "Finalizado",
                "CANCELADO" => "Cancelado",
                _ => state ?? "Pendiente"
            };
        }

        /// <summary>
    /// Obtiene los datos de una empresa por ID
  /// Endpoint: GET assign/enterprise/{id}/
   /// </summary>
        public async Task<EnterpriseDto?> GetEnterpriseAsync(int enterpriseId)
    {
     try
   {
      var endpoint = Endpoints.Assignment.GetEnterprise(enterpriseId);
 return await _apiService.GetAsync<EnterpriseDto>(endpoint);
       }
    catch (Exception)
     {
        throw;
         }
        }

        /// <summary>
    /// Obtiene la lista de modalidades de etapa productiva
 /// Endpoint: GET assign/modality_productive_stage/
     /// </summary>
     public async Task<List<ModalityProductiveStageDto>?> GetModalityProductiveStagesAsync()
  {
          try
         {
    var endpoint = Endpoints.Assignment.GetModalityProductiveStage;
      return await _apiService.GetAsync<List<ModalityProductiveStageDto>>(endpoint);
    }
 catch (Exception)
            {
      throw;
  }
        }
   
     /// <summary>
        /// Obtiene los datos completos de un instructor por ID
/// Endpoint: GET general/instructors/{id}/
        /// </summary>
 public async Task<InstructorDetailDto?> GetInstructorDetailAsync(int instructorId)
        {
 try
        {
          var endpoint = Endpoints.Instructor.GetInstructor(instructorId);
   return await _apiService.GetAsync<InstructorDetailDto>(endpoint);
     }
          catch (Exception)
    {
  throw;
         }
        }
    }
}
