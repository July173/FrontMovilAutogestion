using AutogestionSenaMaui.Api.Dtos;
using Microsoft.Maui.Storage;
using AutogestionSenaMaui.Helpers;
using AutogestionSena.MAUI.Api.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Linq;
using Microsoft.Maui.Controls;

namespace AutogestionSenaMaui.ViewModels;

/// <summary>
/// ViewModel para un item individual del menú
/// </summary>
public class MenuItemViewModel : INotifyPropertyChanged
{
    private bool _isExpanded;
    private bool _isSelected;

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public string BackendRoute { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public int Order { get; set; }

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            _isExpanded = value;
            OnPropertyChanged();
        }
    }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<MenuItemViewModel> _subMenus;

    public MenuItemViewModel()
    {
        _subMenus = new ObservableCollection<MenuItemViewModel>();
        _subMenus.CollectionChanged += SubMenus_CollectionChanged;
    }

    public ObservableCollection<MenuItemViewModel> SubMenus
    {
        get => _subMenus;
        set
        {
            if (_subMenus != null)
            {
                _subMenus.CollectionChanged -= SubMenus_CollectionChanged;
            }
            _subMenus = value ?? new ObservableCollection<MenuItemViewModel>();
            if (_subMenus != null)
            {
                _subMenus.CollectionChanged += SubMenus_CollectionChanged;
            }
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasSubMenus));
        }
    }

    public bool HasSubMenus => SubMenus != null && SubMenus.Count > 0;

    private void SubMenus_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(HasSubMenus));
    }

    public ICommand ToggleCommand { get; set; } = new Command(() => { });
    public ICommand NavigateCommand { get; set; } = new Command(() => { });

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

/// <summary>
/// ViewModel principal para el menú lateral dinámico
/// </summary>
public class DynamicSideMenuViewModel : INotifyPropertyChanged
{
    private readonly ApiService _apiService;
    private readonly MenuService _menuService;
    private string _userName = string.Empty;
    private string _userInitials = string.Empty;
    private string _roleName = string.Empty;
    private int _roleId;
    private bool _isLoading;
    private string _userEmail = string.Empty;

    public DynamicSideMenuViewModel()
    {
        _apiService = new ApiService();
        _menuService = new MenuService(_apiService);
        MenuItems = new ObservableCollection<MenuItemViewModel>();
        
        NavigateCommand = new Command<string>(OnNavigate);
        OpenProfileCommand = new Command(OnOpenProfile);
        LogoutCommand = new Command(async () => await OnLogoutAsync());
        
        // Cargar datos del usuario y menú
        Task.Run(async () => await InitializeAsync());

        // Suscribirse a eventos de login para recargar menú cuando se inicie sesión
        AuthEvents.UserLoggedIn += async (s, e) =>
        {
            try
            {
                if (!string.IsNullOrEmpty(e.AccessToken))
                {
                    _apiService.SetAuthToken(e.AccessToken);
                }

                RoleId = e.RoleId;
                UserName = e.FirstName;

                await LoadMenuAsync();
            }
            catch (Exception)
            {
                // Menu reload error handled silently
            }
        };
    }

    #region Properties

    public ObservableCollection<MenuItemViewModel> MenuItems { get; set; }

    public string UserName
    {
        get => _userName;
        set
        {
            _userName = value;
            OnPropertyChanged();
            UpdateUserInitials();
        }
    }

    public string UserInitials
    {
        get => _userInitials;
        set
        {
            _userInitials = value;
            OnPropertyChanged();
        }
    }

    public string RoleName
    {
        get => _roleName;
        set
        {
            _roleName = value;
            OnPropertyChanged();
        }
    }

    public int RoleId
    {
        get => _roleId;
        set
        {
            _roleId = value;
            OnPropertyChanged();
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged();
        }
    }

    public string UserEmail
    {
        get => _userEmail;
        set
        {
            _userEmail = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Commands

    public ICommand NavigateCommand { get; }
    public ICommand OpenProfileCommand { get; }
    public ICommand LogoutCommand { get; }

    #endregion

    #region Methods

    private async Task InitializeAsync()
    {
        try
        {
            IsLoading = true;

            // Obtener datos del usuario desde Preferences (más robusto)
            var userJson = Preferences.Get("user_data", string.Empty);
            
            if (string.IsNullOrEmpty(userJson))
            {
                // Fallback a SecureStorage
                userJson = await SecureStorage.GetAsync("user_data") ?? string.Empty;
            }

            if (!string.IsNullOrEmpty(userJson))
            {
                try
                {
                    var user = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(userJson);
                    
                    // Extraer firstName
                    if (user.TryGetProperty("firstName", out var firstNameProp))
                        UserName = firstNameProp.GetString() ?? "Usuario";
                    
                    // Extraer lastName y combinar
                    if (user.TryGetProperty("lastName", out var lastNameProp))
                    {
                        var lastName = lastNameProp.GetString();
                        if (!string.IsNullOrEmpty(lastName))
                            UserName = $"{UserName} {lastName}".Trim();
                    }
                    
                    // Extraer roleId
                    if (user.TryGetProperty("roleId", out var roleIdProp))
                        RoleId = roleIdProp.GetInt32();
                    
                    // Extraer email
                    if (user.TryGetProperty("email", out var emailProp))
                        UserEmail = emailProp.GetString() ?? string.Empty;
                }
                catch (Exception)
                {
                    // User data parsing error handled silently
                }
            }

            // Configurar token si existe (para que las peticiones al backend incluyan la cabecera)
            try
            {
                var token = Preferences.Get("AuthToken", string.Empty);
                if (!string.IsNullOrEmpty(token))
                    _apiService.SetAuthToken(token);
            }
            catch { /* ignore */ }

            // Cargar menú según el usuario
            await LoadMenuAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al inicializar menú: {ex.Message}");
            // Cargar menú por defecto en caso de error
            LoadDefaultMenu();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadMenuAsync()
    {
        try
        {
            var userId = Preferences.Get("UserId", 0);

            if (userId <= 0)
            {
                LoadDefaultMenu();
                return;
            }

            var processedData = await _menuService.GetMenuItemsAsync(userId.ToString(), UserName);

            if (processedData != null && processedData.MenuItems != null)
            {
                if (!string.IsNullOrEmpty(processedData.UserInfo.Role))
                    RoleName = processedData.UserInfo.Role;

                if (string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(processedData.UserInfo.Name))
                    UserName = processedData.UserInfo.Name;

                MenuItems.Clear();

                foreach (var menuItem in processedData.MenuItems.OrderBy(m => m.Order))
                {
                    var menuItemVm = CreateMenuItemViewModel(menuItem);
                    MenuItems.Add(menuItemVm);
                }
            }
            else
            {
                LoadDefaultMenu();
            }
        }
        catch (Exception)
        {
            LoadDefaultMenu();
        }
    }

    private MenuItemViewModel CreateMenuItemViewModel(MenuDto dto)
    {
        MenuItemViewModel menuItem = null!;
        
        // Determinar la ruta según el módulo
        string route = "HomePage";
        var moduleNameLower = (dto.ModuleName ?? dto.Name ?? "").ToLower();
        var nameLower = (dto.Name ?? "").ToLower();
        
        // Si es el módulo de Seguridad, navegar a SecurityMainPage
        if (moduleNameLower.Contains("seguridad") || moduleNameLower.Contains("security") || 
            nameLower.Contains("seguridad") || nameLower.Contains("security"))
        {
            route = "SecurityMainPage";
        }
        
        menuItem = new MenuItemViewModel
        {
            Id = dto.Id,
            Name = dto.Name,
            Icon = !string.IsNullOrEmpty(dto.Icon) ? dto.Icon : MenuService.GetModuleIcon(dto.ModuleName),
            Route = route,
            ParentId = dto.ParentId,
            Order = dto.Order,
            IsExpanded = dto.IsExpanded,
            IsSelected = dto.IsActive,
            ToggleCommand = new Command(() => ToggleMenuItem(menuItem)),
            // Store backend route/path para referencia futura
            BackendRoute = dto.BackendPath ?? dto.Route,
            NavigateCommand = new Command(() => NavigateToRoute(
                route, 
                dto.BackendPath ?? dto.Route))
        };

        // Cargar submenús recursivamente (soportar tanto SubMenus como Children)
        var subMenus = dto.SubMenus ?? dto.Children ?? new List<MenuDto>();
        if (subMenus.Any())
        {
            foreach (var subMenu in subMenus.OrderBy(s => s.Order))
            {
                var subMenuItem = CreateMenuItemViewModel(subMenu);
                menuItem.SubMenus.Add(subMenuItem);
            }
        }

        return menuItem;
    }

    private void ToggleMenuItem(MenuItemViewModel menuItem)
    {
        menuItem.IsExpanded = !menuItem.IsExpanded;
    }

    private async void NavigateToRoute(string route, string? backendRoute = null)
    {
        if (string.IsNullOrEmpty(route))
            return;

        try
        {
            // Desmarcar todos los items
            foreach (var item in MenuItems)
            {
                item.IsSelected = false;
                foreach (var subItem in item.SubMenus)
                {
                    subItem.IsSelected = false;
                }
            }

            // Marcar el item actual
            var selectedItem = FindMenuItemByRoute(route);
            if (selectedItem != null)
            {
                selectedItem.IsSelected = true;
            }

            // Usar la ruta determinada (puede ser HomePage o SecurityMainPage según el módulo)
            var shellRoute = route;

            // Navegar usando NavigationHelper para validar permisos
            if (Shell.Current != null)
            {
                await NavigationHelper.NavigateToAsync(shellRoute);
            }
            else if (Application.Current?.MainPage?.Navigation != null)
            {
                // Fallback when Shell.Current is null
            }
        }
        catch (Exception)
        {
            // Navigation error handled silently
        }
    }

    private MenuItemViewModel? FindMenuItemByRoute(string route)
    {
        foreach (var item in MenuItems)
        {
            if (item.Route == route)
                return item;

            var subItem = item.SubMenus.FirstOrDefault(s => s.Route == route);
            if (subItem != null)
                return subItem;
        }
        return null;
    }

    private void OnNavigate(string route)
    {
        NavigateToRoute(route);
    }

    private async void OnOpenProfile()
    {
        // Request to close the side menu first (if any)
        try
        {
            // Inform any listeners (eg. DashboardLayout) that they should close the side menu
            MessagingCenter.Send(this, "CloseSideMenu");
        }
        catch { }

        
    }

    private async Task OnLogoutAsync()
    {
        try
        {
            bool confirm = false;
            
            // Mostrar confirmación
            if (Application.Current?.MainPage != null)
            {
                confirm = await Application.Current.MainPage.DisplayAlert(
                    "Cerrar Sesión",
                    "¿Estás seguro que deseas cerrar sesión?",
                    "Sí",
                    "No"
                );
            }

            if (confirm)
            {
                await NavigationHelper.LogoutAsync();
            }
        }
        catch (Exception)
        {
            // Logout error handled silently
        }
    }

    private void UpdateUserInitials()
    {
        if (string.IsNullOrEmpty(UserName))
        {
            UserInitials = "??";
            return;
        }

        var parts = UserName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2)
        {
            UserInitials = $"{parts[0][0]}{parts[1][0]}".ToUpper();
        }
        else if (parts.Length == 1)
        {
            UserInitials = parts[0].Length >= 2 
                ? parts[0].Substring(0, 2).ToUpper() 
                : parts[0].ToUpper();
        }
        else
        {
            UserInitials = "??";
        }
    }

    private void LoadDefaultMenu()
    {
        // Menú básico por defecto (por si falla la carga desde API)
        // Usando Bootstrap Icons Unicode
        MenuItems.Clear();
        
        var defaultMenu = new MenuItemViewModel
        {
            Id = 1,
            Name = "Seguridad",
            Icon = "\uf59d", // bi-shield-fill
            Route = "security",
            IsExpanded = false,
            ToggleCommand = new Command(() => { }),
            NavigateCommand = new Command(() => NavigateToRoute("security"))
        };

        var subMenu1 = new MenuItemViewModel
        {
            Id = 2,
            Name = "Asignar",
            Route = "security/assign",
            ParentId = 1,
            NavigateCommand = new Command(() => NavigateToRoute("security/assign"))
        };

        defaultMenu.SubMenus.Add(subMenu1);
        MenuItems.Add(defaultMenu);

        // Add Admin menu as fallback in default menu
        var adminMenu = new MenuItemViewModel
        {
            Id = 100,
            Name = "Administración",
            Icon = "\uf002",
            Route = "admin",
            IsExpanded = false,
            ToggleCommand = new Command(() => { }),
            NavigateCommand = new Command(() => NavigateToRoute("admin"))
        };
        MenuItems.Add(adminMenu);

        RoleName = "Usuario";
    }

    #endregion

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
}
