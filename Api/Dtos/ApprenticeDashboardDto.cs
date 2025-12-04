namespace AutogestionSena.MAUI.Api.Dtos
{
  public class ApprenticeDashboardDto
  {
    public bool HasRequest { get; set; }
    public RequestDto? Request { get; set; }
    public DashboardInstructorDto? Instructor { get; set; }
    public string? RequestState { get; set; }
    public bool ShowInstructor { get; set; }
  }

  public class RequestDto
  {
    public int Id { get; set; }
    public string? EnterpriseName { get; set; }
    public string? BossName { get; set; }
    public string? Modality { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public string? RequestDate { get; set; }
    public string? RequestState { get; set; }
    public string? PdfUrl { get; set; }
    public string? CityName { get; set; }
    public string? StateDisplay { get; set; }
    public string? StateColor { get; set; }
    public string? StateCode { get; set; }

    public bool HasPdf => !string.IsNullOrWhiteSpace(PdfUrl);
  }

  public class DashboardInstructorDto
  {
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? SecondName { get; set; }
    public string? FirstLastName { get; set; }
    public string? SecondLastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? KnowledgeArea { get; set; }
    public string? AssignedAt { get; set; }
    public bool ShowContact { get; set; }
  }
}