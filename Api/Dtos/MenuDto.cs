namespace AutogestionSenaMaui.Api.Dtos;

/// <summary>
/// DTO para el menú dinámico basado en permisos del rol
/// </summary>
public class MenuDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public string? BackendPath { get; set; }
    public int? ParentId { get; set; }
    public int Order { get; set; }
    public bool IsExpanded { get; set; }
    public bool IsActive { get; set; }
    public string ModuleName { get; set; } = string.Empty;
    public List<MenuDto> SubMenus { get; set; } = new();
    public List<MenuDto> Children { get; set; } = new();
}

/// <summary>
/// DTO para la respuesta del endpoint de menú
/// </summary>
public class MenuResponseDto
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public List<MenuDto> MenuItems { get; set; } = new();
}

/// <summary>
/// Información del usuario para el menú
/// </summary>
public class MenuUserInfo
{
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public string? Email { get; set; }
    public string Initials { get; set; } = string.Empty;
}

/// <summary>
/// Data procesada del menú incluyendo items y usuario
/// </summary>
public class ProcessedMenuData
{
    public List<MenuDto> MenuItems { get; set; } = new();
    public MenuUserInfo UserInfo { get; set; } = new();
}
