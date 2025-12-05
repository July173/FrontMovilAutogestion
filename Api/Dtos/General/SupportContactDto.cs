namespace AutogestionSena.MAUI.Api.Dtos.General
{
    public class SupportContactDto
    {
        public int Id { get; set; }
        public string? Type { get; set; }
        public string? Label { get; set; }
        public string? Value { get; set; }
        public string? ExtraInfo { get; set; }
        public bool? Active { get; set; }
    }
}
