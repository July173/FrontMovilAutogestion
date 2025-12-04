# 🏗️ Arquitectura del Proyecto - Autogestion SENA MAUI

## 📋 Índice

1. [Visión General](#visión-general)
2. [Estructura de Carpetas](#estructura-de-carpetas)
3. [Comparación con React](#comparación-con-react)
4. [Patrones de Diseño](#patrones-de-diseño)
5. [Flujo de Datos](#flujo-de-datos)
6. [Navegación](#navegación)
7. [Ciclo de Vida](#ciclo-de-vida)

---

## 🌐 Visión General

Este proyecto está construido con **.NET MAUI (Multi-platform App UI)** y sigue una arquitectura **MVVM (Model-View-ViewModel)**, similar a cómo React utiliza componentes con estado y props, pero adaptado al ecosistema .NET.

### Tecnologías Principales

| .NET MAUI | React (Web) |
|-----------|-------------|
| C# + XAML | JavaScript/TypeScript + JSX |
| .NET 8.0 | Node.js + npm |
| MVVM Pattern | Component-based Architecture |
| Shell Navigation | React Router |
| Binding System | useState + useEffect |
| ContentPage | Functional Components |
| ViewModel | Custom Hooks + Context |

---

## 📁 Estructura de Carpetas

```
Front-end-Mobile-Autogestion-Sena/
│
├── 📂 Api/                          # ≈ React: src/api/ o src/services/
│   ├── 📂 config/                   # Configuración de endpoints
│   │   └── Endpoints.cs             # URLs de la API (≈ apiConfig.ts)
│   │
│   ├── 📂 Dtos/                     # Data Transfer Objects
│   │   ├── AdminCountsDto.cs        # ≈ TypeScript interfaces/types
│   │   ├── UserDto.cs               # ≈ User interface
│   │   ├── MenuDto.cs               # ≈ MenuItem interface
│   │   └── ...                      # Más DTOs
│   │
│   └── 📂 Services/                 # Servicios de API
│       ├── ApiService.cs            # Cliente HTTP base (≈ axios instance)
│       ├── UserService.cs           # ≈ userApi.ts
│       ├── MenuService.cs           # ≈ menuApi.ts
│       ├── AdminService.cs          # ≈ adminApi.ts
│       └── ...                      # Más servicios
│
├── 📂 Views/                        # ≈ React: src/pages/ o src/views/
│   ├── HomePage.xaml                # ≈ Home.tsx (main view)
│   ├── HomePage.xaml.cs             # Lógica de HomePage
│   ├── MainLayoutPage.xaml          # ≈ MainLayout.tsx (wrapper)
│   │
│   ├── 📂 login/                    # Vistas de autenticación
│   │   ├── LoginPage.xaml           # ≈ Login.tsx
│   │   ├── LoginPage.xaml.cs        # Lógica del login
│   │   ├── RegisterPage.xaml        # ≈ Register.tsx
│   │   └── ...
│   │
│   ├── 📂 dashboard/                # Dashboards por rol
│   │   ├── AdminDashboardPage.xaml  # ≈ AdminDashboard.tsx
│   │   ├── ApprenticeDashboardPage.xaml
│   │   └── ...
│   │
│   └── 📂 Security/                 # Módulo de seguridad
│       ├── SecurityMainPage.xaml    # ≈ Security/Index.tsx
│       └── ...
│
├── 📂 ContentViews/                 # ≈ React: src/components/
│   ├── TopBar.xaml                  # ≈ Header.tsx (reusable component)
│   ├── Footer.xaml                  # ≈ Footer.tsx
│   ├── DynamicSideMenu.xaml         # ≈ Sidebar.tsx o Menu.tsx
│   ├── BottomMenu.xaml              # ≈ BottomNavigation.tsx
│   ├── DashboardCards.xaml          # ≈ DashboardCards.tsx
│   ├── DashboardCharts.xaml         # ≈ Charts.tsx
│   └── ...                          # Componentes reutilizables
│
├── 📂 ViewModels/                   # ≈ React: Custom Hooks + Context
│   ├── MainLayoutViewModel.cs       # ≈ useLayout() hook
│   ├── DynamicSideMenuViewModel.cs  # ≈ useMenu() hook
│   ├── LoginViewModel.cs            # ≈ useAuth() hook
│   ├── AdminDashboardViewModel.cs   # ≈ useDashboard() hook
│   │
│   └── 📂 Security/                 # ViewModels del módulo Security
│       └── SecurityMainViewModel.cs
│
├── 📂 Helpers/                      # ≈ React: src/utils/ o src/helpers/
│   ├── NavigationHelper.cs          # ≈ navigationUtils.ts
│   ├── MainLayoutHelper.cs          # ≈ layoutUtils.ts
│   ├── RouteMap.cs                  # ≈ routeConfig.ts
│   ├── BootstrapIcons.cs            # ≈ iconMapping.ts
│   └── AuthEvents.cs                # ≈ eventEmitter.ts o EventBus
│
├── 📂 Converters/                   # ≈ React: Custom formatters/utils
│   ├── BooleanConverters.cs         # Conversores de datos para binding
│   ├── TabConverters.cs             # ≈ formatters o computed values
│   └── ...
│
├── 📂 Examples/                     # Ejemplos de uso
│   └── MainLayoutUsageExamples.cs   # Ejemplos de código
│
├── 📂 Resources/                    # ≈ React: public/ o src/assets/
│   ├── 📂 Images/                   # ≈ assets/images/
│   ├── 📂 Fonts/                    # ≈ assets/fonts/
│   ├── 📂 Styles/                   # ≈ styles/ o theme/
│   │   ├── Colors.xaml              # ≈ colors.css o theme.ts
│   │   └── Styles.xaml              # ≈ global.css
│   └── 📂 Raw/                      # Archivos raw
│
├── 📂 Platforms/                    # Código específico de plataforma
│   ├── 📂 Android/                  # Configuración Android
│   ├── 📂 iOS/                      # Configuración iOS
│   └── 📂 Windows/                  # Configuración Windows
│
├── 📂 readmes/                      # Documentación
│   ├── MAINLAYOUT.md
│   ├── PROTECTED_NAVIGATION.md
│   ├── MENU_IMPLEMENTATION.md
│   └── ...
│
├── App.xaml                         # ≈ App.tsx (entry point)
├── App.xaml.cs                      # Lógica de App (≈ App.tsx)
├── AppShell.xaml                    # ≈ Router config (routes.tsx)
├── AppShell.xaml.cs                 # Navegación y rutas
├── MauiProgram.cs                   # ≈ index.tsx (startup)
└── AutogestionSenaMaui.csproj       # ≈ package.json

```

---

## 📂 Detalle de Cada Carpeta

### 🌐 Api/ - Capa de Comunicación

**Propósito:** Maneja toda la comunicación con el backend, similar a la capa de servicios en React.

#### Subcarpetas:

**📁 config/**
```csharp
// Endpoints.cs - Similar a apiConfig.ts en React
public static class Endpoints
{
    public const string BASE_URL = "https://api.example.com";
    public const string LOGIN = "/security/login";
    public const string GET_MENU = "/security/rol-form-permissions/{id}/get-menu";
}
```

**Equivalente React:**
```typescript
// apiConfig.ts
export const API_CONFIG = {
  BASE_URL: "https://api.example.com",
  ENDPOINTS: {
    LOGIN: "/security/login",
    GET_MENU: "/security/rol-form-permissions/{id}/get-menu"
  }
};
```

**📁 Dtos/** - Data Transfer Objects
```csharp
// UserDto.cs - Define la estructura de datos
public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; }
    public int Role { get; set; }
}
```

**Equivalente React:**
```typescript
// types/User.ts
export interface User {
  id: number;
  email: string;
  role: number;
}
```

**📁 Services/** - Servicios de API
```csharp
// UserService.cs - Lógica de llamadas API
public class UserService
{
    private readonly ApiService _apiService;
    
    public async Task<UserDto> GetUserByIdAsync(int id)
    {
        return await _apiService.GetAsync<UserDto>($"users/{id}");
    }
}
```

**Equivalente React:**
```typescript
// services/userService.ts
export const userService = {
  getUserById: async (id: number): Promise<User> => {
    const response = await axios.get(`/users/${id}`);
    return response.data;
  }
};
```

---

### 📱 Views/ - Páginas de la Aplicación

**Propósito:** Contiene las páginas principales, similar a `src/pages/` en React.

#### Estructura:

**Archivos .xaml** → Definen la UI (como JSX en React)
**Archivos .xaml.cs** → Code-behind con lógica (como funciones en componentes React)

**Ejemplo:**
```xaml
<!-- HomePage.xaml - Define la estructura visual -->
<ContentPage xmlns="...">
    <Grid RowDefinitions="Auto,*,Auto">
        <TopBar Grid.Row="0"/>
        <ContentView x:Name="DashboardContainer" Grid.Row="1"/>
        <Footer Grid.Row="2"/>
    </Grid>
</ContentPage>
```

```csharp
// HomePage.xaml.cs - Lógica de la página
public partial class HomePage : ContentPage
{
    protected override async void OnAppearing()
    {
        await LoadUserDataAndDashboard();
    }
}
```

**Equivalente React:**
```tsx
// Home.tsx
export const Home: React.FC = () => {
  useEffect(() => {
    loadUserDataAndDashboard();
  }, []);
  
  return (
    <div className="home">
      <TopBar />
      <DashboardContainer />
      <Footer />
    </div>
  );
};
```

**Subcarpetas:**

- **login/** → Páginas de autenticación (Login, Register, Recovery)
- **dashboard/** → Dashboards por rol (Admin, Apprentice, Instructor, etc.)
- **Security/** → Módulo de seguridad (Users, Roles, Permissions)

---

### 🧩 ContentViews/ - Componentes Reutilizables

**Propósito:** Componentes UI reutilizables, equivalente a `src/components/` en React.

**Características:**
- Encapsulan lógica y UI específica
- Pueden recibir datos via `BindingContext` (≈ props en React)
- Reutilizables en múltiples páginas

**Ejemplo:**
```xaml
<!-- TopBar.xaml - Componente reutilizable -->
<ContentView xmlns="...">
    <Border BackgroundColor="White">
        <Grid ColumnDefinitions="Auto,*,Auto">
            <!-- Breadcrumb -->
            <Label Text="{Binding ActiveModule}"/>
            <!-- Notifications -->
            <Label Text="🔔" Grid.Column="2">
                <Label.GestureRecognizers>
                    <TapGestureRecognizer Command="{Binding NotificationsCommand}"/>
                </Label.GestureRecognizers>
            </Label>
        </Grid>
    </Border>
</ContentView>
```

**Equivalente React:**
```tsx
// TopBar.tsx
interface TopBarProps {
  activeModule: string;
  onNotificationClick: () => void;
}

export const TopBar: React.FC<TopBarProps> = ({ 
  activeModule, 
  onNotificationClick 
}) => {
  return (
    <div className="topbar">
      <span>{activeModule}</span>
      <button onClick={onNotificationClick}>🔔</button>
    </div>
  );
};
```

**Componentes Principales:**

| MAUI ContentView | React Component |
|-----------------|-----------------|
| TopBar | Header |
| Footer | Footer |
| DynamicSideMenu | Sidebar/Menu |
| BottomMenu | BottomNavigation |
| DashboardCards | DashboardCards |
| DashboardCharts | Charts |

---

### 🎯 ViewModels/ - Lógica de Negocio

**Propósito:** Contienen la lógica de negocio y estado, similar a **custom hooks** + **Context API** en React.

**Patrón MVVM:**
```
View (XAML) ←→ ViewModel (C#) ←→ Model (DTOs/Services)
```

**Ejemplo:**
```csharp
// MainLayoutViewModel.cs
public class MainLayoutViewModel : INotifyPropertyChanged
{
    private string _activeModule;
    
    // Property con notificación de cambios (≈ useState)
    public string ActiveModule
    {
        get => _activeModule;
        set
        {
            _activeModule = value;
            OnPropertyChanged(); // Notifica a la UI
        }
    }
    
    // Command (≈ event handler)
    public ICommand NotificationsCommand { get; }
    
    public MainLayoutViewModel()
    {
        NotificationsCommand = new Command(OnNotificationsTapped);
    }
    
    private void OnNotificationsTapped()
    {
        // Lógica del comando
    }
}
```

**Equivalente React:**
```tsx
// useLayout.ts (custom hook)
export const useLayout = () => {
  const [activeModule, setActiveModule] = useState('');
  
  const handleNotificationsTapped = useCallback(() => {
    // Lógica del handler
  }, []);
  
  return {
    activeModule,
    setActiveModule,
    handleNotificationsTapped
  };
};

// Uso en componente
const MyComponent = () => {
  const { activeModule, handleNotificationsTapped } = useLayout();
  return <TopBar module={activeModule} onNotify={handleNotificationsTapped} />;
};
```

**Comparación Detallada:**

| MAUI ViewModel | React Pattern |
|----------------|---------------|
| `INotifyPropertyChanged` | `useState` + re-render |
| Property with setter | `const [value, setValue]` |
| `ICommand` | Event handler function |
| Constructor initialization | `useEffect(() => {}, [])` |
| Private methods | Private functions in component |
| Async methods | `async/await` functions |

---

### 🛠️ Helpers/ - Utilidades y Funciones Auxiliares

**Propósito:** Funciones auxiliares y utilidades, equivalente a `src/utils/` en React.

**Ejemplos:**

**NavigationHelper.cs** ≈ `navigationUtils.ts`
```csharp
public static class NavigationHelper
{
    public static bool IsUserAuthenticated()
    {
        return !string.IsNullOrEmpty(Preferences.Get("AuthToken", ""));
    }
    
    public static async Task NavigateToAsync(string route)
    {
        await Shell.Current.GoToAsync($"///{route}");
    }
}
```

**Equivalente React:**
```typescript
// navigationUtils.ts
export const navigationUtils = {
  isUserAuthenticated: (): boolean => {
    return !!localStorage.getItem('authToken');
  },
  
  navigateTo: (route: string) => {
    navigate(`/${route}`);
  }
};
```

**AuthEvents.cs** ≈ `EventEmitter` o `EventBus`
```csharp
// Sistema de eventos para comunicación entre componentes
public static class AuthEvents
{
    public static event EventHandler<LoginEventArgs> UserLoggedIn;
    
    public static void NotifyUserLoggedIn(int roleId, string email)
    {
        UserLoggedIn?.Invoke(null, new LoginEventArgs(roleId, email));
    }
}
```

**Equivalente React:**
```typescript
// EventBus.ts
class EventBus {
  private listeners: Map<string, Function[]> = new Map();
  
  on(event: string, callback: Function) {
    if (!this.listeners.has(event)) {
      this.listeners.set(event, []);
    }
    this.listeners.get(event)!.push(callback);
  }
  
  emit(event: string, data: any) {
    this.listeners.get(event)?.forEach(cb => cb(data));
  }
}

export const eventBus = new EventBus();
```

---

### 🎨 Converters/ - Transformadores de Datos

**Propósito:** Convierten datos entre diferentes formatos para el binding, similar a **formatters** o **computed values** en React.

**Ejemplo:**
```csharp
// BooleanConverters.cs
public class InvertBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, 
                         object parameter, CultureInfo culture)
    {
        return value is bool b ? !b : true;
    }
}
```

**Uso en XAML:**
```xaml
<ContentView.Resources>
    <converters:InvertBoolConverter x:Key="InvertBool"/>
</ContentView.Resources>

<Label IsVisible="{Binding IsLoading, Converter={StaticResource InvertBool}}"/>
```

**Equivalente React:**
```tsx
// No hay concepto directo, pero se hace inline o con useMemo
const Dashboard = () => {
  const { isLoading } = useData();
  const isNotLoading = useMemo(() => !isLoading, [isLoading]);
  
  return <div style={{ display: isNotLoading ? 'block' : 'none' }}>...</div>;
};
```

---

### 📦 Resources/ - Recursos Estáticos

**Propósito:** Assets, estilos, fuentes e imágenes, equivalente a `public/` y `src/assets/` en React.

**Estructura:**

**Images/** → Imágenes de la aplicación
**Fonts/** → Fuentes personalizadas (BootstrapIcons, etc.)
**Styles/** → Estilos globales y temas
**Raw/** → Archivos raw (JSON, XML, etc.)

**Ejemplo de Estilos:**
```xaml
<!-- Styles/Colors.xaml -->
<ResourceDictionary xmlns="...">
    <Color x:Key="Primary">#39A900</Color>
    <Color x:Key="Secondary">#2D7430</Color>
    <Color x:Key="Background">#D9D9D9</Color>
</ResourceDictionary>
```

**Equivalente React:**
```typescript
// theme/colors.ts
export const colors = {
  primary: '#39A900',
  secondary: '#2D7430',
  background: '#D9D9D9'
};
```

---

## 🔄 Comparación: Patrones MAUI vs React

### 1. **Componentes y Reutilización**

#### MAUI:
```xaml
<!-- ContentView = React Component -->
<ContentView xmlns="...">
    <Label Text="{Binding Title}"/>
    <Button Command="{Binding ClickCommand}"/>
</ContentView>
```

```csharp
public partial class MyComponent : ContentView
{
    public MyComponent()
    {
        InitializeComponent();
    }
}
```

#### React:
```tsx
interface MyComponentProps {
  title: string;
  onClick: () => void;
}

const MyComponent: React.FC<MyComponentProps> = ({ title, onClick }) => {
  return (
    <div>
      <h1>{title}</h1>
      <button onClick={onClick}>Click</button>
    </div>
  );
};
```

---

### 2. **Estado y Reactividad**

#### MAUI (ViewModel):
```csharp
public class MyViewModel : INotifyPropertyChanged
{
    private string _message;
    
    public string Message
    {
        get => _message;
        set
        {
            _message = value;
            OnPropertyChanged(); // Actualiza UI
        }
    }
    
    public event PropertyChangedEventHandler PropertyChanged;
    
    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
```

#### React (Hook):
```tsx
const MyComponent = () => {
  const [message, setMessage] = useState('');
  
  // Cambiar mensaje actualiza UI automáticamente
  const updateMessage = (newMessage: string) => {
    setMessage(newMessage);
  };
  
  return <p>{message}</p>;
};
```

**Similitud:** Ambos usan sistemas de reactividad para actualizar la UI cuando cambian los datos.

---

### 3. **Navegación**

#### MAUI (Shell Navigation):
```csharp
// AppShell.xaml - Define rutas
<ShellContent Route="HomePage" ContentTemplate="{DataTemplate views:HomePage}" />
<ShellContent Route="LoginPage" ContentTemplate="{DataTemplate views:LoginPage}" />

// Navegar
await Shell.Current.GoToAsync("///HomePage");
```

#### React (React Router):
```tsx
// routes.tsx
<BrowserRouter>
  <Routes>
    <Route path="/home" element={<HomePage />} />
    <Route path="/login" element={<LoginPage />} />
  </Routes>
</BrowserRouter>

// Navegar
navigate('/home');
```

---

### 4. **Ciclo de Vida**

#### MAUI:
```csharp
public partial class MyPage : ContentPage
{
    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Se ejecuta cuando la página aparece (≈ useEffect mount)
    }
    
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Se ejecuta cuando la página desaparece (≈ useEffect cleanup)
    }
}
```

#### React:
```tsx
const MyComponent = () => {
  useEffect(() => {
    // Equivalente a OnAppearing
    console.log('Component mounted');
    
    return () => {
      // Equivalente a OnDisappearing
      console.log('Component unmounted');
    };
  }, []); // [] = solo en mount/unmount
  
  return <div>Content</div>;
};
```

---

### 5. **Inyección de Dependencias**

#### MAUI:
```csharp
// MauiProgram.cs
builder.Services.AddSingleton<ApiService>();
builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<LoginViewModel>();

// Uso en constructor
public LoginPage(LoginViewModel viewModel)
{
    InitializeComponent();
    BindingContext = viewModel;
}
```

#### React (Context API):
```tsx
// ApiContext.tsx
const ApiContext = createContext<ApiService>(null!);

export const ApiProvider = ({ children }) => {
  const apiService = useMemo(() => new ApiService(), []);
  return <ApiContext.Provider value={apiService}>{children}</ApiContext.Provider>;
};

// Uso
const MyComponent = () => {
  const apiService = useContext(ApiContext);
  return <div>...</div>;
};
```

---

### 6. **Binding de Datos**

#### MAUI (Two-Way Binding):
```xaml
<!-- El Entry actualiza automáticamente la propiedad Email -->
<Entry Text="{Binding Email, Mode=TwoWay}" />
```

```csharp
public string Email { get; set; } // Se actualiza automáticamente
```

#### React (Controlled Component):
```tsx
const LoginForm = () => {
  const [email, setEmail] = useState('');
  
  return (
    <input 
      value={email} 
      onChange={(e) => setEmail(e.target.value)} 
    />
  );
};
```

---

## 🔀 Flujo de Datos en el Proyecto

### Arquitectura General

```
┌─────────────────────────────────────────────────────┐
│                   Usuario Interactúa                 │
└────────────────────┬────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────┐
│              View (XAML Page/ContentView)            │
│  - Define UI                                         │
│  - Binding a ViewModel                               │
│  - Event handlers                                    │
└────────────────────┬────────────────────────────────┘
                     │ Binding / Commands
                     ▼
┌─────────────────────────────────────────────────────┐
│           ViewModel (Lógica de Presentación)         │
│  - Propiedades con INotifyPropertyChanged            │
│  - Commands (ICommand)                               │
│  - Validaciones                                      │
│  - Llama a Services                                  │
└────────────────────┬────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────┐
│              Services (Lógica de Negocio)            │
│  - UserService, MenuService, etc.                    │
│  - Llama a ApiService                                │
│  - Procesa datos                                     │
└────────────────────┬────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────┐
│              ApiService (HTTP Client)                │
│  - HttpClient configurado                            │
│  - GET, POST, PUT, DELETE                            │
│  - Manejo de tokens                                  │
└────────────────────┬────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────┐
│                  Backend API                         │
│  - Endpoints REST                                    │
│  - Autenticación JWT                                 │
│  - Base de datos                                     │
└─────────────────────────────────────────────────────┘
```

### Ejemplo Concreto: Login Flow

```
1. Usuario hace tap en "Iniciar Sesión"
   └─> LoginPage.xaml (Button con Command)

2. Command ejecuta LoginCommand en ViewModel
   └─> LoginViewModel.OnLoginAsync()

3. ViewModel valida datos y llama al servicio
   └─> UserService.LoginAsync(email, password)

4. Service hace la llamada HTTP
   └─> ApiService.PostAsync("/security/login", credentials)

5. Backend responde con token JWT
   └─> { "access": "token...", "user": {...} }

6. Service procesa respuesta y retorna
   └─> LoginResponse con datos del usuario

7. ViewModel guarda token y notifica éxito
   └─> Preferences.Set("AuthToken", token)
   └─> AuthEvents.NotifyUserLoggedIn(...)

8. Navigation Helper navega a HomePage
   └─> await Shell.Current.GoToAsync("///HomePage")

9. HomePage detecta rol y carga dashboard
   └─> LoadDashboardForRole(userRole)

10. UI se actualiza automáticamente
    └─> Gracias a INotifyPropertyChanged
```

---

## 🎯 Patrones de Diseño Implementados

### 1. **MVVM (Model-View-ViewModel)**
- **Separación de responsabilidades**
- View se comunica con ViewModel via Binding
- ViewModel no conoce la View
- Similar a React con hooks y Context

### 2. **Repository Pattern**
- Services actúan como repositorios
- Abstracción de acceso a datos
- Similar a servicios en React

### 3. **Singleton (ApiService)**
```csharp
builder.Services.AddSingleton<ApiService>();
```
- Una sola instancia del cliente HTTP
- Similar a instancia única de axios en React

### 4. **Command Pattern (ICommand)**
```csharp
public ICommand LoginCommand { get; }
```
- Encapsula acciones en objetos
- Similar a event handlers en React

### 5. **Observer Pattern (INotifyPropertyChanged)**
```csharp
public event PropertyChangedEventHandler PropertyChanged;
```
- La UI observa cambios en ViewModel
- Similar a re-render en React cuando cambia state

### 6. **Dependency Injection**
- Constructor injection para servicios
- Similar a Context API en React

---

## 🚀 Ventajas de esta Arquitectura

### ✅ Separación de Responsabilidades
- **View:** Solo UI (XAML)
- **ViewModel:** Lógica de presentación
- **Services:** Lógica de negocio
- **API:** Comunicación con backend

### ✅ Testeable
```csharp
// Podemos testear ViewModel sin UI
[Test]
public void LoginCommand_WithValidData_ShouldSucceed()
{
    var viewModel = new LoginViewModel(mockUserService);
    viewModel.Email = "test@example.com";
    viewModel.Password = "password123";
    
    viewModel.LoginCommand.Execute(null);
    
    Assert.IsTrue(viewModel.IsLoggedIn);
}
```

### ✅ Reutilizable
- ContentViews se usan en múltiples páginas
- Services compartidos entre ViewModels
- Helpers utilizables en toda la app

### ✅ Mantenible
- Cambios en UI no afectan lógica
- Cambios en API solo afectan Services
- Fácil agregar nuevas features

### ✅ Escalable
- Estructura modular
- Fácil agregar nuevos módulos
- Patrones consistentes

---

## 📊 Tabla Comparativa Final

| Concepto | .NET MAUI | React |
|----------|-----------|-------|
| **UI Definition** | XAML | JSX/TSX |
| **Language** | C# | JavaScript/TypeScript |
| **State Management** | INotifyPropertyChanged | useState, useReducer |
| **Side Effects** | OnAppearing/OnDisappearing | useEffect |
| **Components** | ContentView, ContentPage | Functional Components |
| **Props** | BindingContext | props |
| **Event Handling** | ICommand | Event handlers (onClick, etc.) |
| **Navigation** | Shell Navigation | React Router |
| **HTTP Client** | HttpClient | fetch/axios |
| **Storage** | Preferences, SecureStorage | localStorage, sessionStorage |
| **Dependency Injection** | Built-in DI Container | Context API, Custom Providers |
| **Styling** | XAML Styles, ResourceDictionary | CSS, CSS-in-JS |
| **Type Safety** | Strong typing (C#) | TypeScript (optional) |
| **Build Tool** | .NET CLI | npm/yarn + webpack/vite |
| **Package Manager** | NuGet | npm/yarn |

---

## 🎓 Conceptos Clave para Desarrolladores React

Si vienes de React, estos son los conceptos equivalentes:

### 1. **Component → ContentView/ContentPage**
```csharp
// MAUI ContentView ≈ React Component
public partial class MyComponent : ContentView { }
```

### 2. **useState → Property with INotifyPropertyChanged**
```csharp
// MAUI
private string _value;
public string Value
{
    get => _value;
    set { _value = value; OnPropertyChanged(); }
}

// React
const [value, setValue] = useState('');
```

### 3. **useEffect → OnAppearing/OnDisappearing**
```csharp
// MAUI
protected override void OnAppearing()
{
    // Código al aparecer
}

// React
useEffect(() => {
    // Código al montar
}, []);
```

### 4. **props → BindingContext**
```csharp
// MAUI
myComponent.BindingContext = viewModel;

// React
<MyComponent viewModel={viewModel} />
```

### 5. **onClick → Command**
```csharp
// MAUI
<Button Command="{Binding MyCommand}" />

// React
<button onClick={handleClick}>Click</button>
```

### 6. **Context API → Dependency Injection**
```csharp
// MAUI
builder.Services.AddSingleton<IMyService, MyService>();

// React
const MyContext = createContext<IMyService>(null);
```

---

## 🎯 Flujo Típico de Desarrollo

### Crear una Nueva Feature

#### 1. **Crear DTO (si es necesario)**
```csharp
// Api/Dtos/ProductDto.cs
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

#### 2. **Crear Service**
```csharp
// Api/Services/ProductService.cs
public class ProductService
{
    public async Task<List<ProductDto>> GetProductsAsync()
    {
        return await _apiService.GetAsync<List<ProductDto>>("products");
    }
}
```

#### 3. **Crear ViewModel**
```csharp
// ViewModels/ProductListViewModel.cs
public class ProductListViewModel : INotifyPropertyChanged
{
    public ObservableCollection<ProductDto> Products { get; set; }
    public ICommand LoadCommand { get; }
    
    private async Task LoadProductsAsync()
    {
        var products = await _productService.GetProductsAsync();
        Products = new ObservableCollection<ProductDto>(products);
    }
}
```

#### 4. **Crear View**
```xaml
<!-- Views/ProductListPage.xaml -->
<ContentPage>
    <CollectionView ItemsSource="{Binding Products}">
        <CollectionView.ItemTemplate>
            <DataTemplate>
                <Label Text="{Binding Name}" />
            </DataTemplate>
        </CollectionView.ItemTemplate>
    </CollectionView>
</ContentPage>
```

#### 5. **Registrar en DI y Shell**
```csharp
// MauiProgram.cs
builder.Services.AddTransient<ProductService>();
builder.Services.AddTransient<ProductListViewModel>();
builder.Services.AddTransient<ProductListPage>();

// AppShell.xaml
<ShellContent Route="ProductList" ContentTemplate="{DataTemplate views:ProductListPage}" />
```

---

## 📚 Recursos Adicionales

### Documentación del Proyecto
- `MAINLAYOUT.md` - Sistema de layout
- `PROTECTED_NAVIGATION.md` - Navegación protegida
- `MENU_IMPLEMENTATION.md` - Menú dinámico
- `HOMEPAGE_IMPLEMENTATION.md` - HomePage con dashboards

### Documentación Externa
- [.NET MAUI Docs](https://learn.microsoft.com/dotnet/maui/)
- [MVVM Pattern](https://learn.microsoft.com/dotnet/architecture/maui/mvvm)
- [Shell Navigation](https://learn.microsoft.com/dotnet/maui/fundamentals/shell/)

---

## 🎉 Conclusión

Este proyecto implementa una arquitectura sólida y escalable usando **.NET MAUI con el patrón MVVM**, muy similar a cómo React estructura aplicaciones con **componentes, hooks y Context API**. 

**Principales similitudes:**
- ✅ Componentes reutilizables (ContentViews ≈ Components)
- ✅ Estado reactivo (INotifyPropertyChanged ≈ useState)
- ✅ Ciclo de vida (OnAppearing ≈ useEffect)
- ✅ Navegación declarativa (Shell ≈ React Router)
- ✅ Servicios/API layer
- ✅ Helpers y utilidades
- ✅ Separación de responsabilidades

**Diferencias clave:**
- MAUI usa **XAML** en lugar de JSX
- MAUI usa **C#** en lugar de JavaScript/TypeScript
- MAUI usa **MVVM** en lugar de hooks directamente
- MAUI tiene **tipos fuertes nativos** (C#)
- MAUI es **multiplataforma nativo** (Android, iOS, Windows, macOS)

---

**Autor:** GitHub Copilot  
**Fecha:** 20 de noviembre de 2025  
**Proyecto:** Autogestion SENA - MAUI  
**Versión:** 1.0.0
