namespace AutogestionSena.MAUI.Api.Dtos
{
    public class SupportSchedule
    {
        public int Id { get; set; }
        public string? DayRange { get; set; }
        public string? Hours { get; set; }
        public bool IsClosed { get; set; }
        public string? Notes { get; set; }
    }

    public class SupportContact
    {
        public int Id { get; set; }
        public string? Type { get; set; }
        public string? Label { get; set; }
        public string? Value { get; set; }
        public string? ExtraInfo { get; set; }
        public bool Active { get; set; }
    }
}