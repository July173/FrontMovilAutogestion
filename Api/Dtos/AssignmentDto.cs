using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AutogestionSena.MAUI.Api.Dtos
{
    /// <summary>
    /// DTO para solicitudes de asignación
    /// </summary>
    public class AssignmentRequestDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("aprendiz_id")]
        public int AprendizId { get; set; }

        [JsonPropertyName("nombre")]
        public string? Nombre { get; set; }

        [JsonPropertyName("tipo_identificacion")]
        public int TipoIdentificacion { get; set; }

        [JsonPropertyName("numero_identificacion")]
        public int NumeroIdentificacion { get; set; }

        [JsonPropertyName("fecha_solicitud")]
        public string? FechaSolicitud { get; set; }

        /// <summary>
        /// Fecha de creación parseada como DateTime
        /// </summary>
        public DateTime? CreatedAt
        {
            get
            {
                if (string.IsNullOrEmpty(FechaSolicitud))
                    return null;

                if (DateTime.TryParse(FechaSolicitud, out DateTime result))
                    return result;

                return null;
            }
        }

        [JsonPropertyName("request_state")]
        public string? RequestState { get; set; }

        [JsonPropertyName("nombre_modalidad")]
        public string? NombreModalidad { get; set; }
    }

    /// <summary>
    /// Respuesta del endpoint de lista de solicitudes
    /// </summary>
    public class AssignmentRequestListResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("data")]
        public List<AssignmentRequestDto>? Data { get; set; }
    }

    /// <summary>
    /// DTO simple de aprendiz (solo lo básico)
    /// </summary>
    public class ApprenticeSimpleDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("person")]
        public int Person { get; set; }

        [JsonPropertyName("ficha")]
        public int? Ficha { get; set; }

        [JsonPropertyName("active")]
        public bool Active { get; set; }
    }

    /// <summary>
    /// DTO para los datos de la empresa
    /// Usado para GET assign/enterprise/{id}/
    /// Estructura real del API: {"id":2,"name_enterprise":"asdfg","locate":"afxcv","nit_enterprise":21436,"active":true,"email_enterprise":"vcxcbv@gmail.com"}
    /// </summary>
    public class EnterpriseDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name_enterprise")]
        public string? Name { get; set; }

        [JsonPropertyName("nit_enterprise")]
        public int? Nit { get; set; }

        [JsonPropertyName("locate")]
        public string? Address { get; set; }

        [JsonPropertyName("email_enterprise")]
        public string? Email { get; set; }

        [JsonPropertyName("immediate_boss")]
        public string? ImmediateBoss { get; set; }

        [JsonPropertyName("active")]
        public bool Active { get; set; }
    }

    /// <summary>
    /// DTO para los datos de modalidad de etapa productiva
    /// Usado para GET assign/modality_productive_stage/
    /// Estructura real: {"id":2,"name_modality":"Vínculo Laboral","description":"...","active":true}
    /// </summary>
    public class ModalityProductiveStageDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name_modality")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("active")]
        public bool Active { get; set; }
    }

    /// <summary>
    /// DTO para los datos completos del instructor
    /// Usado para GET general/instructors/{id}/
    /// </summary>
    public class InstructorDetailDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }

        [JsonPropertyName("second_name")]
        public string? SecondName { get; set; }

        [JsonPropertyName("first_last_name")]
        public string? FirstLastName { get; set; }

        [JsonPropertyName("second_last_name")]
        public string? SecondLastName { get; set; }

        [JsonPropertyName("phone_number")]
        public long? PhoneNumber { get; set; }

        [JsonPropertyName("type_identification")]
        public int TypeIdentification { get; set; }

        [JsonPropertyName("number_identification")]
        public long NumberIdentification { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("role")]
        public int Role { get; set; }

        [JsonPropertyName("contract_type")]
        public int ContractType { get; set; }

        [JsonPropertyName("contract_start_date")]
        public string? ContractStartDate { get; set; }

        [JsonPropertyName("contract_end_date")]
        public string? ContractEndDate { get; set; }

        [JsonPropertyName("knowledge_area")]
        public int KnowledgeArea { get; set; }

        [JsonPropertyName("sede")]
        public int Sede { get; set; }

        [JsonPropertyName("is_followup_instructor")]
        public bool IsFollowupInstructor { get; set; }

        [JsonPropertyName("assigned_learners")]
        public int AssignedLearners { get; set; }

        [JsonPropertyName("max_assigned_learners")]
        public int MaxAssignedLearners { get; set; }

        [JsonPropertyName("active")]
        public bool Active { get; set; }
    }

    /// <summary>
    /// DTO para la respuesta del dashboard del aprendiz (estructura real del API)
    /// Usado para GET assign/request_asignation/aprendiz-dashboard/?aprendiz_id={id}
    /// </summary>
    public class ApprenticeDashboardRealApiResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("enterprise")]
        public int Enterprise { get; set; }

        [JsonPropertyName("modality_productive_stage")]
        public int ModalityProductiveStage { get; set; }

        [JsonPropertyName("start_date")]
        public string? StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public string? EndDate { get; set; }

        [JsonPropertyName("request_date")]
        public string? RequestDate { get; set; }

        [JsonPropertyName("request_state")]
        public string? RequestState { get; set; }

        [JsonPropertyName("pdf_url")]
        public string? PdfUrl { get; set; }

        [JsonPropertyName("instructor_id")]
        public int? InstructorId { get; set; }

        [JsonPropertyName("instructor_first_name")]
        public string? InstructorFirstName { get; set; }

        [JsonPropertyName("instructor_second_name")]
        public string? InstructorSecondName { get; set; }

        [JsonPropertyName("instructor_first_last_name")]
        public string? InstructorFirstLastName { get; set; }

        [JsonPropertyName("instructor_second_last_name")]
        public string? InstructorSecondLastName { get; set; }

        [JsonPropertyName("instructor_number_identification")]
        public long? InstructorNumberIdentification { get; set; }

        [JsonPropertyName("instructor_phone_number")]
        public long? InstructorPhoneNumber { get; set; }

        [JsonPropertyName("instructor_type_identification")]
        public string? InstructorTypeIdentification { get; set; }

        [JsonPropertyName("instructor_knowledge_area")]
        public string? InstructorKnowledgeArea { get; set; }

        [JsonPropertyName("instructor_email")]
        public string? InstructorEmail { get; set; }
    }

    /// <summary>
    /// DTO para la respuesta del dashboard cuando NO hay instructor asignado (array)
    /// Usado para GET assign/request_asignation/aprendiz-dashboard/?aprendiz_id={id}
    /// cuando el estado es PRE-APROBADO o similar
    /// Estructura: [{"id":1,"apprentice":1,"enterprise":1,"modality_productive_stage":2,...}]
    /// </summary>
    public class ApprenticeDashboardBasicApiResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("apprentice")]
        public int Apprentice { get; set; }

        [JsonPropertyName("enterprise")]
        public int Enterprise { get; set; }

        [JsonPropertyName("modality_productive_stage")]
        public int ModalityProductiveStage { get; set; }

        [JsonPropertyName("request_date")]
        public string? RequestDate { get; set; }

        [JsonPropertyName("date_start_production_stage")]
        public string? DateStartProductionStage { get; set; }

        [JsonPropertyName("start_date")]
        public string? StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public string? EndDate { get; set; }

        [JsonPropertyName("pdf_request")]
        public string? PdfRequest { get; set; }

        [JsonPropertyName("pdf_url")]
        public string? PdfUrl { get; set; }

        [JsonPropertyName("request_state")]
        public string? RequestState { get; set; }
    }
}
