namespace AutogestionSena.MAUI.Api.Dtos
{
    public class ApprenticeDto
    {
        public string? Id { get; set; }
        public int Person { get; set; }
        public int Ficha { get; set; }
        public bool Active { get; set; }
    }

    public class CreateApprenticeDto
    {
        public string? TypeIdentification { get; set; }
        public string? NumberIdentification { get; set; }
        public string? FirstName { get; set; }
        public string? SecondName { get; set; }
        public string? FirstLastName { get; set; }
        public string? SecondLastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public int Program { get; set; }
        public string? Ficha { get; set; }
        public int? Role { get; set; }
    }
}