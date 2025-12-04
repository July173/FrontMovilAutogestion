#  Manual Técnico de Despliegue - AutoGestión SENA

## .NET MAUI - Generación de APK para Android

---

## Índice

1. [Requisitos Previos](#1-requisitos-previos)
2. [Configuración del Entorno](#2-configuración-del-entorno)
3. [Estructura del Proyecto](#3-estructura-del-proyecto)
4. [Entornos de Despliegue](#121-entornos-de-despliegue)
5. [Ejecución en Modo Desarrollo](#122-ejecución-en-modo-desarrollo)
6. [Compilación APK de Pruebas](#123-compilación-apk-de-pruebas)
7. [Compilación APK de Producción](#124-compilación-apk-de-producción)
8. [Firma de la Aplicación](#125-firma-de-la-aplicación)
9. [Instalación de APK en Dispositivo](#126-instalación-de-apk-en-dispositivo)
10. [Versionamiento](#127-versionamiento)
11. [Solución de Problemas Comunes](#11-solución-de-problemas-comunes)
12. [Comandos de Referencia Rápida](#12-comandos-de-referencia-rápida)

>  **Nota:** Para una guía detallada de compilación e instalación de APK, consultar: [GUIA_COMPILACION_INSTALACION_APK.md](./GUIA_COMPILACION_INSTALACION_APK.md)

---

## 1. Requisitos Previos

### 1.1 Software Necesario

| Software | Versión Mínima | Descarga |
|----------|----------------|----------|
| **.NET SDK** | 8.0 o superior | [Descargar](https://dotnet.microsoft.com/download/dotnet/8.0) |
| **Visual Studio 2022** | 17.8 o superior | [Descargar](https://visualstudio.microsoft.com/) |
| **Android SDK** | API 21+ (Android 5.0+) | Incluido en VS |
| **Java JDK** | 11 o superior | [Descargar](https://www.oracle.com/java/technologies/downloads/) |
| **Android Studio** (opcional) | Última versión | [Descargar](https://developer.android.com/studio) |

### 1.2 Cargas de Trabajo de Visual Studio

Al instalar Visual Studio 2022, asegúrate de seleccionar:

```
 Desarrollo de .NET Multi-platform App UI
 Desarrollo móvil con .NET
 SDK de Android (API 33, 34)
 Android SDK Build-Tools
 Android Emulator
 Intel HAXM (para emuladores x86)
```

### 1.3 Verificar Instalación

Ejecuta estos comandos en PowerShell para verificar:

```powershell
# Verificar .NET SDK
dotnet --version
# Esperado: 8.0.xxx

# Verificar workloads de MAUI
dotnet workload list
# Debe incluir: maui, maui-android, android

# Si no está instalado MAUI:
dotnet workload install maui
dotnet workload install android
```

### 1.4 Variables de Entorno

Configura estas variables de entorno en Windows:

```powershell
# Agregar al PATH del sistema (Panel de Control > Sistema > Variables de Entorno)

ANDROID_HOME = C:\Program Files (x86)\Android\android-sdk
JAVA_HOME = C:\Program Files\Java\jdk-11

# Agregar al PATH:
%ANDROID_HOME%\platform-tools
%ANDROID_HOME%\tools
%JAVA_HOME%\bin
```

---

## 2. Configuración del Entorno

### 2.1 Clonar el Repositorio

```powershell
# Clonar el repositorio
git clone https://github.com/July173/Front-end-Mobile-Autogestion-Sena.git

# Entrar al directorio
cd Front-end-Mobile-Autogestion-Sena

# Cambiar a la rama de desarrollo
git checkout HU-14-dev
```

### 2.2 Restaurar Dependencias

```powershell
# Restaurar paquetes NuGet
dotnet restore

# Verificar que compila correctamente
dotnet build
```

### 2.3 Estructura de Configuración de API

El archivo de configuración de API se encuentra en:

```
Api/
└── config/
    └── ApiConfig.cs
```

Contenido típico:

```csharp
namespace AutogestionSenaMaui.Api.Config
{
    public static class ApiConfig
    {
        // URL base del API según el entorno
        public static string BaseUrl => "http://10.3.234.91:8001/api/";
        
        // Timeout de conexión en segundos
        public static int TimeoutSeconds => 30;
    }
}
```

---

## 3. Estructura del Proyecto

```
Front-end-Mobile-Autogestion-Sena/
├──  Api/                          # Servicios y configuración de API
│   ├── config/                      # Configuración de endpoints
│   ├── Dtos/                        # Data Transfer Objects
│   └── Services/                    # Servicios de consumo API
├──  ContentViews/                 # Componentes reutilizables
├──  Converters/                   # Value Converters para XAML
├──  Helpers/                      # Clases de utilidad
├──  Platforms/                    # Código específico por plataforma
│   ├── Android/                     # Configuración Android
│   │   ├── AndroidManifest.xml      # Permisos y configuración
│   │   ├── MainActivity.cs          # Actividad principal
│   │   └── MainApplication.cs       # Aplicación Android
│   ├── iOS/                         # Configuración iOS
│   ├── MacCatalyst/                 # Configuración Mac
│   └── Windows/                     # Configuración Windows
├──  Resources/                    # Recursos de la aplicación
│   ├── AppIcon/                     # Íconos de la app
│   ├── Fonts/                       # Fuentes personalizadas
│   ├── Images/                      # Imágenes
│   ├── Splash/                      # Pantalla de inicio
│   └── Styles/                      # Estilos XAML
├──  Services/                     # Servicios locales
├──  Validators/                   # Validadores de formularios
├──  ViewModels/                   # ViewModels (MVVM)
├──  Views/                        # Vistas XAML
├──  Tests/                        # Proyecto de pruebas
├──  App.xaml                      # Recursos globales
├──  App.xaml.cs                   # Clase Application
├──  AppShell.xaml                 # Navegación Shell
├──  MauiProgram.cs                # Punto de entrada
└──  AutogestionSenaMaui.csproj    # Archivo de proyecto
```

---

## 12. Compilación y Despliegue

## 12.1 Entornos de Despliegue

La aplicación AutoGestión SENA maneja **cuatro entornos** de despliegue:

| Entorno | Rama Git | Propósito | API URL |
|---------|----------|-----------|---------|
| **Dev** | `HU-*-dev`, `feature/*` | Desarrollo y nuevas funcionalidades | `http://10.3.234.91:8001/api/` |
| **QA** | `qa`, `test` | Pruebas de calidad y testing | `http://10.3.234.91:8002/api/` |
| **Staging** | `staging`, `pre-prod` | Pre-producción, pruebas finales | `http://10.3.234.91:8003/api/` |
| **Producción** | `main`, `master` | Versión en producción | `https://api.autogestion.sena.edu.co/api/` |

### 12.1.1 Configurar URL por Entorno

Para cambiar el entorno, modifica el archivo `Api/config/ApiConfig.cs`:

```csharp
namespace AutogestionSenaMaui.Api.Config
{
    public static class ApiConfig
    {
        // ============================================
        // CONFIGURACIÓN DE ENTORNO
        // Descomentar la línea del entorno deseado
        // ============================================
        
        //  DESARROLLO (Dev)
        public static string BaseUrl => "http://10.3.234.91:8001/api/";
        
        //  QA (Testing)
        // public static string BaseUrl => "http://10.3.234.91:8002/api/";
        
        //  STAGING (Pre-producción)
        // public static string BaseUrl => "http://10.3.234.91:8003/api/";
        
        //  PRODUCCIÓN
        // public static string BaseUrl => "https://api.autogestion.sena.edu.co/api/";
        
        public static int TimeoutSeconds => 30;
    }
}
```

### 12.1.2 Uso de Compilación Condicional (Avanzado)

Para automatizar el cambio de entorno según la configuración de build:

```csharp
namespace AutogestionSenaMaui.Api.Config
{
    public static class ApiConfig
    {
        public static string BaseUrl
        {
            get
            {
#if DEBUG
                return "http://10.3.234.91:8001/api/";  // Dev
#elif QA
                return "http://10.3.234.91:8002/api/";  // QA
#elif STAGING
                return "http://10.3.234.91:8003/api/";  // Staging
#else
                return "https://api.autogestion.sena.edu.co/api/"; // Producción
#endif
            }
        }
    }
}
```

---

## 12.2 Ejecución en Modo Desarrollo

### 12.2.1 Usando Visual Studio 2022

1. **Abrir la solución:**
   - Doble clic en `AutogestionSenaMaui.sln`

2. **Seleccionar dispositivo de destino:**
   - En la barra de herramientas, seleccionar:
     - `Android Emulator` → Para emulador
     - `Pixel 5 - API 34` → Emulador específico
     - `Samsung Galaxy...` → Dispositivo físico conectado

3. **Iniciar depuración:**
   - Presionar `F5` o clic en ▶️ (Iniciar depuración)
   - Esperar a que compile y despliegue

### 12.2.2 Usando Terminal (PowerShell)

```powershell
# Navegar al proyecto
cd "C:\Users\braya\Desktop\Front-end-Mobile-Autogestion-Sena"

# Compilar en modo Debug
dotnet build -f net8.0-android -c Debug

# Ejecutar en emulador (debe estar corriendo)
dotnet build -f net8.0-android -c Debug -t:Run
```

### 12.2.3 Configurar Emulador Android

```powershell
# Listar emuladores disponibles
emulator -list-avds

# Iniciar un emulador específico
emulator -avd Pixel_5_API_34

# O desde Android Studio:
# Tools > Device Manager > Create Device
```

### 12.2.4 Conectar Dispositivo Físico

1. **Habilitar Opciones de Desarrollador en el teléfono:**
   - Ir a `Configuración > Acerca del teléfono`
   - Tocar 7 veces en `Número de compilación`
   - Mensaje: "Ahora eres desarrollador"

2. **Habilitar Depuración USB:**
   - Ir a `Configuración > Opciones de desarrollador`
   - Activar `Depuración USB`

3. **Conectar por USB:**
   - Conectar el dispositivo al PC con cable USB
   - Aceptar el prompt de depuración en el teléfono

4. **Verificar conexión:**
   ```powershell
   adb devices
   # Debe mostrar el dispositivo:
   # XXXXXXXX    device
   ```

---

## 12.3 Compilación APK de Pruebas

### 12.3.1 APK de Debug (Desarrollo)

Esta APK es para pruebas internas y desarrollo. **NO está optimizada**.

```powershell
# Navegar al proyecto
cd "C:\Users\braya\Desktop\Front-end-Mobile-Autogestion-Sena"

# Limpiar compilaciones anteriores
dotnet clean -f net8.0-android

# Compilar APK de Debug
dotnet build -f net8.0-android -c Debug

# La APK se genera en:
# bin\Debug\net8.0-android\com.companyname.autogestionsena.maui-Signed.apk
```

### 12.3.2 Verificar APK Generada

```powershell
# Buscar la APK generada
Get-ChildItem -Path "bin\Debug\net8.0-android\" -Filter "*.apk" -Recurse

# Ver tamaño del archivo
$apk = Get-Item "bin\Debug\net8.0-android\com.companyname.autogestionsena.maui-Signed.apk"
Write-Host "Tamaño: $([math]::Round($apk.Length / 1MB, 2)) MB"
```

### 12.3.3 Características de APK Debug

| Característica | Debug | Release |
|----------------|-------|---------|
| Optimización |  No |  Sí |
| Símbolos de depuración |  Incluidos |  Removidos |
| Logging |  Habilitado |  Reducido |
| Tamaño | ~80-120 MB | ~40-60 MB |
| Velocidad | Más lenta | Más rápida |
| Uso | Desarrollo/Testing | Producción |

> 📘 **Para guía detallada de compilación:** Ver [GUIA_COMPILACION_INSTALACION_APK.md](./GUIA_COMPILACION_INSTALACION_APK.md)

---

## 12.4 Instalación de APK en Dispositivo

### 12.4.1 Método 1: Usando ADB (Recomendado)

```powershell
# Verificar dispositivo conectado
adb devices

# Instalar APK (reemplaza si existe)
adb install -r "bin\Debug\net8.0-android\com.companyname.autogestionsena.maui-Signed.apk"

# Instalar y otorgar todos los permisos
adb install -r -g "bin\Debug\net8.0-android\com.companyname.autogestionsena.maui-Signed.apk"

# Desinstalar aplicación anterior (si hay conflictos)
adb uninstall com.companyname.autogestionsena.maui
```

### 12.4.2 Método 2: Transferencia Manual

1. **Copiar APK al dispositivo:**
   ```powershell
   adb push "bin\Debug\net8.0-android\com.companyname.autogestionsena.maui-Signed.apk" /sdcard/Download/
   ```

2. **En el dispositivo:**
   - Abrir `Archivos` o `Gestor de archivos`
   - Navegar a `Descargas`
   - Tocar el archivo `.apk`
   - Permitir instalación de fuentes desconocidas (si se solicita)
   - Instalar

### 12.4.3 Habilitar "Orígenes Desconocidos"

En Android 8.0+:

1. Ir a `Configuración > Aplicaciones`
2. Buscar la app desde donde instalas (Chrome, Archivos, etc.)
3. Activar `Instalar aplicaciones desconocidas`

### 12.4.4 Solución a Errores de Instalación

| Error | Causa | Solución |
|-------|-------|----------|
| `INSTALL_FAILED_UPDATE_INCOMPATIBLE` | Firma diferente | Desinstalar app anterior: `adb uninstall com.companyname.autogestionsena.maui` |
| `INSTALL_FAILED_VERSION_DOWNGRADE` | Versión menor | Incrementar `ApplicationVersion` en `.csproj` |
| `INSTALL_PARSE_FAILED_NO_CERTIFICATES` | APK no firmada | Usar APK `-Signed.apk` |
| `INSTALL_FAILED_INSUFFICIENT_STORAGE` | Sin espacio | Liberar espacio en dispositivo |

> 📘 **Para guía detallada de instalación:** Ver [GUIA_COMPILACION_INSTALACION_APK.md](./GUIA_COMPILACION_INSTALACION_APK.md)

---

## 12.5 Compilación APK de Producción (Release)

Esta sección cubre la compilación de la APK optimizada para distribución.

### 12.5.1 APK de Release (Producción)

```powershell
# Navegar al proyecto
cd "C:\Users\braya\Desktop\Front-end-Mobile-Autogestion-Sena"

# Limpiar compilaciones anteriores
dotnet clean -f net8.0-android -c Release

# Restaurar paquetes
dotnet restore

# Compilar y publicar APK de Release
dotnet publish -f net8.0-android -c Release

# La APK se genera en:
# bin\Release\net8.0-android\publish\com.companyname.autogestionsena.maui-Signed.apk
```

### 12.5.2 Opciones Adicionales de Compilación

```powershell
# Compilar con optimizaciones específicas
dotnet publish -f net8.0-android -c Release `
    /p:AndroidPackageFormat=apk `
    /p:AndroidLinkMode=SdkOnly `
    /p:EnableLLVM=true

# Para generar AAB (Android App Bundle) para Google Play:
dotnet publish -f net8.0-android -c Release `
    /p:AndroidPackageFormat=aab
```

### 12.5.3 Tabla de Parámetros de Compilación

| Parámetro | Valores | Descripción |
|-----------|---------|-------------|
| `-f` | `net8.0-android` | Framework objetivo |
| `-c` | `Debug`, `Release` | Configuración de build |
| `/p:AndroidPackageFormat` | `apk`, `aab` | Formato de salida |
| `/p:AndroidLinkMode` | `None`, `SdkOnly`, `Full` | Nivel de linking |
| `/p:EnableLLVM` | `true`, `false` | Compilador LLVM |
| `/p:RunAOTCompilation` | `true`, `false` | Compilación AOT |

---

## 12.6 Firma de la Aplicación

### 12.6.1 ¿Por qué Firmar la APK?

-  **Requerido** para instalar en dispositivos sin depuración
-  **Requerido** para publicar en Google Play Store
-  Garantiza la **integridad** de la aplicación
-  Identifica al **desarrollador/organización**

### 12.6.2 Crear Keystore (Solo una vez)

```powershell
# Crear directorio para el keystore
New-Item -ItemType Directory -Path "Platforms\Android\Keystore" -Force

# Generar keystore con keytool
keytool -genkey -v `
    -keystore "Platforms\Android\Keystore\autogestion-sena.keystore" `
    -alias autogestion `
    -keyalg RSA `
    -keysize 2048 `
    -validity 10000

# Se te pedirá:
# - Contraseña del keystore (mínimo 6 caracteres)
# - Nombre y apellido
# - Unidad organizacional
# - Organización (SENA)
# - Ciudad
# - Estado/Provincia
# - Código de país (CO)
```

### 12.6.3 Configurar Firma en el Proyecto

Agrega al archivo `AutogestionSenaMaui.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
    
    <!-- ...existing code... -->
    
    <!-- ============================================ -->
    <!-- CONFIGURACIÓN DE FIRMA PARA ANDROID RELEASE -->
    <!-- ============================================ -->
    <PropertyGroup Condition="$(TargetFramework.Contains('-android')) and '$(Configuration)' == 'Release'">
        <AndroidKeyStore>true</AndroidKeyStore>
        <AndroidSigningKeyStore>Platforms\Android\Keystore\autogestion-sena.keystore</AndroidSigningKeyStore>
        <AndroidSigningKeyAlias>autogestion</AndroidSigningKeyAlias>
        <AndroidSigningKeyPass>TU_PASSWORD_AQUI</AndroidSigningKeyPass>
        <AndroidSigningStorePass>TU_PASSWORD_AQUI</AndroidSigningStorePass>
    </PropertyGroup>
    
    <!-- ...existing code... -->
    
</Project>
```

### 12.6.4 Compilar con Firma

```powershell
# Opción 1: Usar configuración del .csproj
dotnet publish -f net8.0-android -c Release

# Opción 2: Pasar credenciales por línea de comandos (más seguro para CI/CD)
dotnet publish -f net8.0-android -c Release `
    /p:AndroidKeyStore=true `
    /p:AndroidSigningKeyStore="Platforms\Android\Keystore\autogestion-sena.keystore" `
    /p:AndroidSigningKeyAlias=autogestion `
    /p:AndroidSigningKeyPass=TU_PASSWORD `
    /p:AndroidSigningStorePass=TU_PASSWORD
```

### 12.6.5 Verificar Firma de la APK

```powershell
# Verificar que la APK está firmada correctamente
jarsigner -verify -verbose -certs "bin\Release\net8.0-android\publish\com.companyname.autogestionsena.maui-Signed.apk"

# Debería mostrar:
# jar verified.
```

### 12.6.6  Seguridad del Keystore

> **IMPORTANTE:** El keystore es como una contraseña maestra. Si lo pierdes, **no podrás actualizar** tu aplicación en Google Play.

**Buenas prácticas:**

1.  **Respalda** el keystore en un lugar seguro (no en el repositorio)
2.  **No incluyas** el keystore en Git (añadir a `.gitignore`)
3.  **Guarda** las contraseñas en un gestor de contraseñas
4.  **Documenta** la información del keystore en un lugar seguro

Agregar a `.gitignore`:

```gitignore
# Keystore de Android - NO subir al repositorio
*.keystore
*.jks
Platforms/Android/Keystore/
```

---

## 12.7 Versionamiento

### 12.7.1 Esquema de Versiones

El proyecto sigue **Semantic Versioning (SemVer)**:

```
MAJOR.MINOR.PATCH (ej: 1.2.3)

MAJOR: Cambios incompatibles con versiones anteriores
MINOR: Nuevas funcionalidades compatibles
PATCH: Corrección de errores
```

### 12.7.2 Configurar Versión en el Proyecto

Editar `AutogestionSenaMaui.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
    
    <PropertyGroup>
        <!-- ...existing code... -->
        
        <!-- ============================================ -->
        <!-- CONFIGURACIÓN DE VERSIÓN DE LA APLICACIÓN -->
        <!-- ============================================ -->
        
        <!-- Versión mostrada al usuario (Semantic Versioning) -->
        <ApplicationDisplayVersion>1.0.0</ApplicationDisplayVersion>
        
        <!-- Código de versión interno (debe incrementarse en cada release) -->
        <!-- Google Play requiere que este número sea mayor en cada actualización -->
        <ApplicationVersion>1</ApplicationVersion>
        
        <!-- Título de la aplicación -->
        <ApplicationTitle>AutoGestión SENA</ApplicationTitle>
        
        <!-- Identificador único de la aplicación -->
        <ApplicationId>com.sena.autogestion</ApplicationId>
        
        <!-- ...existing code... -->
    </PropertyGroup>
    
</Project>
```

### 12.7.3 Historial de Versiones

| Versión | Código | Fecha | Cambios Principales |
|---------|--------|-------|---------------------|
| 1.0.0 | 1 | 2025-11-26 | Versión inicial |
| 1.1.0 | 2 | YYYY-MM-DD | Nuevas funcionalidades |
| 1.1.1 | 3 | YYYY-MM-DD | Corrección de errores |
| 1.2.0 | 4 | YYYY-MM-DD | Dashboard mejorado |

### 12.7.4 Proceso de Release

```powershell
# 1. Actualizar versión en .csproj
#    ApplicationDisplayVersion: 1.1.0
#    ApplicationVersion: 2

# 2. Commit de la versión
git add AutogestionSenaMaui.csproj
git commit -m "chore: bump version to 1.1.0"

# 3. Crear tag de versión
git tag -a v1.1.0 -m "Release version 1.1.0"

# 4. Push con tags
git push origin HU-14-dev --tags

# 5. Compilar APK de Release
dotnet publish -f net8.0-android -c Release

# 6. Renombrar APK con versión
$version = "1.1.0"
Copy-Item "bin\Release\net8.0-android\publish\com.companyname.autogestionsena.maui-Signed.apk" `
          "releases\AutoGestion-SENA-v$version.apk"
```

### 12.7.5 Script de Versionamiento Automático

Crear archivo `scripts\bump-version.ps1`:

```powershell
param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("major", "minor", "patch")]
    [string]$Type
)

# Leer versión actual del .csproj
$csprojPath = "AutogestionSenaMaui.csproj"
$content = Get-Content $csprojPath -Raw

# Extraer versión actual
$versionMatch = [regex]::Match($content, '<ApplicationDisplayVersion>(\d+)\.(\d+)\.(\d+)</ApplicationDisplayVersion>')
$major = [int]$versionMatch.Groups[1].Value
$minor = [int]$versionMatch.Groups[2].Value
$patch = [int]$versionMatch.Groups[3].Value

$codeMatch = [regex]::Match($content, '<ApplicationVersion>(\d+)</ApplicationVersion>')
$code = [int]$codeMatch.Groups[1].Value

# Incrementar según tipo
switch ($Type) {
    "major" { $major++; $minor = 0; $patch = 0 }
    "minor" { $minor++; $patch = 0 }
    "patch" { $patch++ }
}
$code++

$newVersion = "$major.$minor.$patch"

Write-Host "Versión anterior: $($versionMatch.Groups[0].Value)" -ForegroundColor Yellow
Write-Host "Nueva versión: $newVersion (código: $code)" -ForegroundColor Green

# Actualizar .csproj
$content = $content -replace '<ApplicationDisplayVersion>\d+\.\d+\.\d+</ApplicationDisplayVersion>', "<ApplicationDisplayVersion>$newVersion</ApplicationDisplayVersion>"
$content = $content -replace '<ApplicationVersion>\d+</ApplicationVersion>', "<ApplicationVersion>$code</ApplicationVersion>"
$content | Set-Content $csprojPath -NoNewline

Write-Host " Versión actualizada en $csprojPath" -ForegroundColor Cyan
```

Uso:

```powershell
# Incrementar versión patch (1.0.0 → 1.0.1)
.\scripts\bump-version.ps1 -Type patch

# Incrementar versión minor (1.0.1 → 1.1.0)
.\scripts\bump-version.ps1 -Type minor

# Incrementar versión major (1.1.0 → 2.0.0)
.\scripts\bump-version.ps1 -Type major
```

---

## 11. Solución de Problemas Comunes

### 11.1 Error: "Android SDK not found"

```powershell
# Verificar ubicación del SDK
$env:ANDROID_HOME

# Si no existe, configurar:
[Environment]::SetEnvironmentVariable("ANDROID_HOME", "C:\Program Files (x86)\Android\android-sdk", "User")

# Reiniciar terminal y verificar
```

### 11.2 Error: "Java not found" o "JAVA_HOME not set"

```powershell
# Verificar Java instalado
java -version

# Configurar JAVA_HOME
[Environment]::SetEnvironmentVariable("JAVA_HOME", "C:\Program Files\Java\jdk-11", "User")
```

### 11.3 Error: "License not accepted"

```powershell
# Aceptar licencias de Android SDK
cd "$env:ANDROID_HOME\cmdline-tools\latest\bin"
.\sdkmanager --licenses

# Aceptar todas con 'y'
```

### 11.4 Error: "Build failed" con errores de recursos

```powershell
# Limpiar completamente
dotnet clean
Remove-Item -Recurse -Force bin, obj -ErrorAction SilentlyContinue

# Restaurar y compilar
dotnet restore
dotnet build -f net8.0-android
```

### 11.5 Error: "INSTALL_FAILED_UPDATE_INCOMPATIBLE"

```powershell
# Desinstalar versión anterior
adb uninstall com.companyname.autogestionsenaMaui

# Reinstalar
adb install "ruta\a\la\apk.apk"
```

### 11.6 La aplicación se cierra inmediatamente

```powershell
# Ver logs del dispositivo en tiempo real
adb logcat -s "DOTNET" "mono" "AndroidRuntime"

# Filtrar solo errores
adb logcat *:E | Select-String "AutogestionSena|MAUI|Mono"
```

---

## 12. Comandos de Referencia Rápida

### Compilación

```powershel
# Debug (desarrollo)
dotnet build -f net8.0-android -c Debug

# Release (producción)
dotnet publish -f net8.0-android -c Release

# Limpiar
dotnet clean -f net8.0-android
```

### Dispositivos

```powershell
# Listar dispositivos conectados
adb devices

# Instalar APK
adb install -r "ruta/a/apk.apk"

# Desinstalar app
adb uninstall com.companyname.autogestionsenaMaui

# Ver logs
adb logcat -s "DOTNET"
```

### Emulador

```powershell
# Listar emuladores
emulator -list-avds

# Iniciar emulador
emulator -avd Pixel_5_API_34
```

### Pruebas

```powershell
# Ejecutar todas las pruebas
cd Tests
dotnet test

# Solo unitarias
dotnet test --filter "FullyQualifiedName~UnitTests"

# Solo integración
dotnet test --filter "FullyQualifiedName~IntegrationTests"
```

---

## 📋 Checklist de Despliegue

### Pre-Release

- [ ] Actualizar versión en `.csproj`
- [ ] Verificar URL de API según entorno
- [ ] Ejecutar todas las pruebas
- [ ] Probar en dispositivo físico
- [ ] Revisar permisos en `AndroidManifest.xml`

### Build

- [ ] Limpiar compilaciones anteriores
- [ ] Compilar en modo Release
- [ ] Firmar APK con keystore
- [ ] Verificar tamaño de APK

### Post-Release

- [ ] Crear tag en Git
- [ ] Documentar cambios en CHANGELOG
- [ ] Archivar APK con nombre versionado
- [ ] Notificar al equipo

---

## Soporte

Para dudas o problemas con el despliegue:

- **Repositorio:** https://github.com/July173/Front-end-Mobile-Autogestion-Sena
- **Documentación .NET MAUI:** https://learn.microsoft.com/dotnet/maui/

---

*Última actualización: 26 de noviembre de 2025*
