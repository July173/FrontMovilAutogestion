using System.Collections.Generic;
using AutogestionSena.MAUI.Api.Dtos;

namespace AutogestionSenaMaui.Api.Dtos
{
    public class RoleModuleFormDto
    {
        public string? Rol { get; set; }
        public List<ModuleFormDto>? ModuleForm { get; set; }
    }
}
