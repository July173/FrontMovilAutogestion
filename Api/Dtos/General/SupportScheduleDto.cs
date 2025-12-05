namespace AutogestionSena.MAUI.Api.Dtos.General
{
    public class SupportScheduleDto
    {
        public int Id { get; set; }
        public string? DayRange { get; set; }
        public string? Hours { get; set; }
        public bool? IsClosed { get; set; }
        public string? Notes { get; set; }
        public bool? Active { get; set; }
    }
}
