using System.Collections.Generic;

namespace AutogestionSena.MAUI.Api.Dtos
{
    public class FormDto
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Path { get; set; }
        public string? Icon { get; set; }
        public bool Active { get; set; }
        public int Order { get; set; }
    }
}