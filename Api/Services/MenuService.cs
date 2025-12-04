using AutogestionSena.MAUI.Api.Dtos;
using AutogestionSenaMaui.Api.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutogestionSena.MAUI.Api.Services
{
    /// <summary>
    /// Servicio para operaciones relacionadas con el menú dinámico del usuario.
    /// Incluye recuperación y procesamiento de items del menú.
    /// Similar al servicio Menu.ts de React.
    /// </summary>
    public class MenuService
    {
        private readonly ApiService _apiService;
        
        // Mapeo de módulos a iconos (Bootstrap Icons Unicode)
        private static readonly Dictionary<string, string> ModuleIconMap = new()
        {
            ["inicio"] = "\uf425",           // bi-house-fill
            ["home"] = "\uf425",
            ["seguridad"] = "\uf59d",        // bi-shield-fill
            ["security"] = "\uf59d",
            ["administración"] = "\uf4cb",   // bi-person-check-fill
            ["administracion"] = "\uf4cb",
            ["administration"] = "\uf4cb",
            ["user-check"] = "\uf4cb",
            ["asignar seguimientos"] = "\uf4dd", // bi-person-workspace
            ["asignar seguimiento"] = "\uf4dd",
            ["seguimientos"] = "\uf4dd",
            ["usuario"] = "\uf4da",          // bi-person-fill
            ["user"] = "\uf4da",
            ["reportes"] = "\uf2e6",         // bi-bar-chart-fill
            ["chart"] = "\uf2e6",
            ["configuración"] = "\uf3e5",    // bi-gear-fill
            ["configuracion"] = "\uf3e5",
            ["settings"] = "\uf3e5",
        };

        public MenuService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public MenuService()
        {
            _apiService = new ApiService();
        }

        /// <summary>
        /// Obtiene los elementos del menú para un usuario específico y los procesa.
        /// Endpoint: GET /api/security/rol-form-permissions/{userId}/get-menu/
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="userName">Nombre del usuario (opcional)</param>
        /// <returns>Promise con los datos del menú procesados</returns>
        public async Task<ProcessedMenuData> GetMenuItemsAsync(string userId, string? userName = null)
        {
            try
            {
                var apiData = await _apiService.GetAsync<List<RoleModuleFormDto>>(
                    $"security/rol-form-permissions/{userId}/get-menu/");

                if (apiData == null || !apiData.Any())
                {
                    return CreateEmptyMenuData(userName);
                }

                return ProcessApiResponse(apiData, userName);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Procesa la respuesta de la API y la convierte al formato necesario para el componente.
        /// Similar al método processApiResponse de React.
        /// </summary>
        /// <param name="apiData">Respuesta de la API</param>
        /// <param name="userName">Nombre del usuario (opcional)</param>
        /// <returns>Datos procesados para menú y usuario</returns>
        public ProcessedMenuData ProcessApiResponse(List<RoleModuleFormDto> apiData, string? userName = null)
        {
            if (apiData == null || !apiData.Any())
            {
                return CreateEmptyMenuData(userName);
            }

            var userData = apiData.First();
            var menuItems = new List<MenuDto>();
            int idCounter = 1000;

            // Procesar cada módulo y sus formularios
            foreach (var module in userData.ModuleForm ?? Enumerable.Empty<ModuleFormDto>())
            {
                var moduleName = module.Name ?? "Module";
                var moduleIconKey = moduleName.ToLower();
                var moduleIcon = ModuleIconMap.ContainsKey(moduleIconKey) 
                    ? ModuleIconMap[moduleIconKey] 
                    : "\uf425"; // default: house icon

                // Generar un ID único combinando módulo y formulario
                var moduleId = $"{moduleName.ToLower().Replace(" ", "-")}";
                
                // Determinar la ruta del módulo (usar la primera ruta de formulario si existe)
                var moduleRoute = string.Empty;
                var firstForm = module.Form?.FirstOrDefault();
                if (firstForm != null && !string.IsNullOrEmpty(firstForm.Path))
                {
                    moduleRoute = firstForm.Path;
                }

                var moduleItem = new MenuDto
                {
                    Id = idCounter++,
                    Name = moduleName,
                    Icon = module.Icon ?? moduleIcon,
                    Route = moduleRoute,
                    BackendPath = moduleRoute,
                    ModuleName = moduleName,
                    Order = 0,
                    IsActive = false,
                    IsExpanded = false,
                    SubMenus = new List<MenuDto>(),
                    Children = new List<MenuDto>()
                };

                // Procesar formularios del módulo
                if (module.Form != null && module.Form.Any())
                {
                    int subOrder = 1;
                    foreach (var form in module.Form.OrderBy(f => f.Order))
                    {
                        if (!form.Active) continue; // Skip inactive forms

                        var formId = $"{moduleId}-{form.Name?.ToLower().Replace(" ", "-")}";
                        var formPath = form.Path ?? $"/{moduleName.ToLower()}/{form.Name?.ToLower().Replace(" ", "-")}";

                        var formItem = new MenuDto
                        {
                            Id = idCounter++,
                            Name = form.Name ?? "Form",
                            Icon = form.Icon ?? moduleIcon,
                            Route = formPath,
                            BackendPath = form.Path,
                            ModuleName = moduleName,
                            ParentId = moduleItem.Id,
                            Order = subOrder++,
                            IsActive = false,
                            IsExpanded = false,
                            SubMenus = new List<MenuDto>(),
                            Children = new List<MenuDto>()
                        };

                        moduleItem.SubMenus.Add(formItem);
                        moduleItem.Children.Add(formItem);
                    }
                }

                menuItems.Add(moduleItem);
            }

            return new ProcessedMenuData
            {
                MenuItems = menuItems,
                UserInfo = new MenuUserInfo
                {
                    Name = userName ?? "Usuario",
                    Role = userData.Rol ?? "Sin rol",
                    Initials = GetInitials(userName ?? "Usuario")
                }
            };
        }

        /// <summary>
        /// Crea un objeto de menú vacío con información básica del usuario
        /// </summary>
        private ProcessedMenuData CreateEmptyMenuData(string? userName)
        {
            return new ProcessedMenuData
            {
                MenuItems = new List<MenuDto>(),
                UserInfo = new MenuUserInfo
                {
                    Name = userName ?? "Usuario",
                    Role = "Sin rol",
                    Initials = GetInitials(userName ?? "Usuario")
                }
            };
        }

        /// <summary>
        /// Obtiene las iniciales de un nombre
        /// </summary>
        private string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "??";

            var parts = name.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length >= 2)
            {
                return $"{parts[0][0]}{parts[1][0]}".ToUpper();
            }
            else if (parts.Length == 1 && parts[0].Length >= 2)
            {
                return parts[0].Substring(0, 2).ToUpper();
            }
            else if (parts.Length == 1)
            {
                return parts[0].ToUpper();
            }

            return "??";
        }

        /// <summary>
        /// Obtiene el icono para un módulo basado en su nombre
        /// </summary>
        public static string GetModuleIcon(string moduleName)
        {
            var key = moduleName?.ToLower() ?? "";
            return ModuleIconMap.ContainsKey(key) ? ModuleIconMap[key] : "\uf425";
        }
    }
}
