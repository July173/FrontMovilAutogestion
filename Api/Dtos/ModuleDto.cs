using System.Collections.Generic;

namespace AutogestionSena.MAUI.Api.Dtos
{
    public class ModuleDto
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool Active { get; set; }
    }
}