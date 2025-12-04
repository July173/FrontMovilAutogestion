using System.Text.Json.Serialization;

namespace AutogestionSena.MAUI.Api.Dtos
{
    public class ApprenticeDashboardApiResponse
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("enterprise")]
        public int? EnterpriseId { get; set; }

        [JsonPropertyName("enterprise_name")]
        public string? EnterpriseName { get; set; }

        [JsonPropertyName("boss_name")]
        public string? BossName { get; set; }

        [JsonPropertyName("modality_productive_stage")]
        public int? ModalityProductiveStageId { get; set; }

        [JsonPropertyName("modality_productive_stage_name")]
        public string? ModalityProductiveStageName { get; set; }

        [JsonPropertyName("agreement_date")]
        public string? AgreementDate { get; set; }

        [JsonPropertyName("start_date")]
        public string? StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public string? EndDate { get; set; }

        [JsonPropertyName("city_id")]
        public int? CityId { get; set; }

        [JsonPropertyName("city_name")]
        public string? CityName { get; set; }

        [JsonPropertyName("request_date")]
        public string? RequestDate { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonPropertyName("state_display")]
        public string? StateDisplay { get; set; }

        [JsonPropertyName("state_color")]
        public string? StateColor { get; set; }

        [JsonPropertyName("pdf_url")]
        public string? PdfUrl { get; set; }

        [JsonPropertyName("instructor")]
        public ApprenticeDashboardInstructorApiResponse? Instructor { get; set; }
    }

    public class ApprenticeDashboardInstructorApiResponse
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("person")]
        public ApprenticeDashboardInstructorPersonApiResponse? Person { get; set; }

        [JsonPropertyName("contact_email")]
        public string? ContactEmail { get; set; }

        [JsonPropertyName("contact_phone")]
        public string? ContactPhone { get; set; }

        [JsonPropertyName("knowledge_area_name")]
        public string? KnowledgeAreaName { get; set; }

        [JsonPropertyName("assignation_date")]
        public string? AssignationDate { get; set; }

        [JsonPropertyName("show_contact")]
        public bool? ShowContact { get; set; }
    }

    public class ApprenticeDashboardInstructorPersonApiResponse
    {
        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }

        [JsonPropertyName("second_name")]
        public string? SecondName { get; set; }

        [JsonPropertyName("first_last_name")]
        public string? FirstLastName { get; set; }

        [JsonPropertyName("second_last_name")]
        public string? SecondLastName { get; set; }
    }
}
