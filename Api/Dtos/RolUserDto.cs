namespace AutogestionSena.MAUI.Api.Dtos
{
    public class RolUserDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool Active { get; set; }
        public int UserCount { get; set; }
    }

    public class RolUserCountDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int UserCount { get; set; }
    }
}