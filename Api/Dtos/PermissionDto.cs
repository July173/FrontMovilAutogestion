namespace AutogestionSena.MAUI.Api.Dtos
{
    public class PermissionDto
    {
        public int Rol { get; set; }
        public int Form { get; set; }
        public bool Ver { get; set; }
        public bool Editar { get; set; }
        public bool Registrar { get; set; }
        public bool Eliminar { get; set; }
        public bool Activar { get; set; }
    }
}