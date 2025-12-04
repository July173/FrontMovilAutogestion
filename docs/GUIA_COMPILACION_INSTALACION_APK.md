# 📱 Guía de Compilación e Instalación de APK

## AutoGestión SENA - .NET MAUI Android

---

## Índice

1. [Compilación APK de Pruebas](#-1-compilación-apk-de-pruebas)
2. [Instalación de APK en Dispositivo](#-2-instalación-de-apk-en-dispositivo)

---

##  1. Compilación APK de Pruebas

Esta sección documenta el **proceso exacto para generar el archivo APK (Android Package Kit)** de la aplicación que se utilizará para realizar pruebas funcionales y de calidad.

### 1.1 Entorno y Herramientas

| Componente | Versión/Especificación | Propósito |
|------------|------------------------|-----------|
| **IDE Principal** | Visual Studio 2022 (v17.8+) | Desarrollo y compilación |
| **IDE Alternativo** | Visual Studio Code + extensión C# | Edición de código |
| **Framework** | .NET MAUI (.NET 8.0) | Framework multiplataforma |
| **.NET SDK** | 8.0.x | Compilación y ejecución |
| **Android SDK** | API 21+ (mínimo Android 5.0) | Plataforma objetivo |
| **Android Build-Tools** | 34.0.0 | Herramientas de compilación |
| **Java JDK** | 11 o superior | Requerido por Android SDK |

### 1.2 Procedimiento de Compilación - Visual Studio 2022

#### Opción A: Compilación desde la Interfaz Gráfica

1. **Abrir la solución:**
   - Doble clic en `AutogestionSenaMaui.sln`
   - Esperar a que se carguen todos los proyectos

2. **Seleccionar configuración de Build:**
   - En la barra de herramientas superior:
     - **Configuración:** Seleccionar `Debug`
     - **Plataforma:** Seleccionar `Any CPU`
     - **Framework:** Seleccionar `net8.0-android`

3. **Compilar la solución:**
   - Menú: `Build > Build Solution` (o presionar `Ctrl+Shift+B`)
   - Esperar a que finalice la compilación
   - Verificar en la ventana de salida: `Build succeeded`

4. **Generar APK:**
   - Menú: `Build > Publish Selection`
   - O hacer clic derecho en el proyecto > `Publish`

#### Opción B: Compilación desde Terminal (PowerShell) - RECOMENDADO

```powershell
# 1. Navegar al directorio del proyecto
cd "C:\Users\braya\Desktop\Front-end-Mobile-Autogestion-Sena"

# 2. Limpiar compilaciones anteriores (evita conflictos)
dotnet clean -f net8.0-android -c Debug

# 3. Restaurar paquetes NuGet
dotnet restore

# 4. Compilar APK de Debug (Pruebas)
dotnet build -f net8.0-android -c Debug

# 5. Verificar que la APK se generó correctamente
Get-ChildItem -Path "bin\Debug\net8.0-android\" -Filter "*.apk" -Recurse | Format-Table Name, Length, LastWriteTime
```

### 1.3 Ubicación del Archivo APK Generado

Después de la compilación exitosa, el archivo APK se encuentra en:

```
 Front-end-Mobile-Autogestion-Sena/
└──  bin/
    └──  Debug/
        └──  net8.0-android/
            ├──  com.companyname.autogestionsena.maui.apk          (APK sin firmar)
            └──  com.companyname.autogestionsena.maui-Signed.apk   (APK firmada )
```

**Ruta completa:**

```
C:\Users\braya\Desktop\Front-end-Mobile-Autogestion-Sena\bin\Debug\net8.0-android\com.companyname.autogestionsena.maui-Signed.apk
```

### 1.4 Nomenclatura y Versionamiento de APK

Para mantener un control adecuado de las versiones de prueba, se recomienda seguir esta convención de nomenclatura:

```
AutoGestion-SENA-[version]-[fecha]-[entorno].apk

Ejemplos:
- AutoGestion-SENA-1.0.0-2025.11.27-debug.apk
- AutoGestion-SENA-1.0.0-2025.11.27-qa.apk
```

**Script para renombrar APK automáticamente:**

```powershell
# Variables
$version = "1.0.0"
$fecha = Get-Date -Format "yyyy.MM.dd"
$entorno = "debug"
$origen = "bin\Debug\net8.0-android\com.companyname.autogestionsena.maui-Signed.apk"
$destino = "releases\AutoGestion-SENA-$version-$fecha-$entorno.apk"

# Crear carpeta releases si no existe
New-Item -ItemType Directory -Path "releases" -Force | Out-Null

# Copiar y renombrar
Copy-Item $origen $destino
Write-Host " APK generada: $destino" -ForegroundColor Green
```

### 1.5 Características de la APK de Pruebas (Debug)

| Característica | APK Debug (Pruebas) | APK Release (Producción) |
|----------------|---------------------|--------------------------|
| **Optimización de código** |  Deshabilitada |  Habilitada |
| **Símbolos de depuración** |  Incluidos |  Removidos |
| **Logging/Debug.WriteLine** | ✅ Activo | ⚠️ Limitado |
| **Debuggeable** | ✅ Sí | ❌ No |
| **Tamaño aproximado** | ~80-120 MB | ~40-60 MB |
| **Rendimiento** | Más lenta | Más rápida |
| **Firma** | Debug keystore | Keystore de producción |
| **Uso recomendado** | Testing interno, QA | Google Play, distribución |

### 1.6 Verificar Integridad de la APK

```powershell
# Ver información de la APK generada
$apk = Get-Item "bin\Debug\net8.0-android\com.companyname.autogestionsena.maui-Signed.apk"

Write-Host "📦 Información de la APK:" -ForegroundColor Cyan
Write-Host "   Nombre: $($apk.Name)"
Write-Host "   Tamaño: $([math]::Round($apk.Length / 1MB, 2)) MB"
Write-Host "   Fecha de creación: $($apk.CreationTime)"
Write-Host "   Ruta: $($apk.FullName)"

# Verificar firma (requiere Java/keytool)
jarsigner -verify -verbose "bin\Debug\net8.0-android\com.companyname.autogestionsena.maui-Signed.apk"
```

### 1.7 Compilación con Parámetros Adicionales

```powershell
# Compilación con información detallada (verbose)
dotnet build -f net8.0-android -c Debug -v detailed

# Compilación forzando reconstrucción completa
dotnet build -f net8.0-android -c Debug --no-incremental

# Compilación especificando versión de Android
dotnet build -f net8.0-android -c Debug /p:AndroidTargetSdkVersion=34
```

---

## 📱 2. Instalación de APK en Dispositivo

Esta sección detalla **cómo el archivo APK generado se transfiere e instala** en el dispositivo móvil (físico o emulador) para que los testers puedan ejecutar y verificar la aplicación.

### 2.1 Requisitos Previos del Dispositivo

#### Para Dispositivo Físico Android

| Requisito | Cómo Habilitarlo |
|-----------|------------------|
| **Android 5.0+** (API 21+) | Verificar en: `Configuración > Acerca del teléfono > Versión de Android` |
| **Opciones de Desarrollador** | `Configuración > Acerca del teléfono` → Tocar 7 veces en `Número de compilación` |
| **Depuración USB** | `Configuración > Opciones de desarrollador` → Activar `Depuración USB` |
| **Orígenes Desconocidos** | Permitir instalación desde fuentes externas (ver sección 2.6) |
| **Espacio disponible** | Mínimo 150 MB libres |

#### Pasos para Habilitar Opciones de Desarrollador

1. Ir a: `Configuración > Acerca del teléfono`
2. Buscar: "Número de compilación" o "Build number"
3. Tocar 7 veces consecutivas
4. Mensaje: "¡Ahora eres un desarrollador!"
5. Volver a: `Configuración > Sistema > Opciones de desarrollador`
6. Activar: "Depuración USB"
7. Conectar el dispositivo por USB al PC
8. Aceptar el prompt de depuración en el teléfono (marcar "Siempre permitir")

### 2.2 Método 1: Instalación por ADB (Android Debug Bridge) - RECOMENDADO

Este es el método más rápido y confiable para desarrollo y testing.

#### Prerequisitos

- ADB instalado (viene con Android SDK)
- Dispositivo conectado por USB con depuración habilitada

#### Comandos de Instalación

```powershell
# 1. Verificar que el dispositivo está conectado y reconocido
adb devices
# Salida esperada:
# List of devices attached
# XXXXXXXX    device

# 2. Instalar la APK (reemplaza si ya existe)
adb install -r "C:\Users\braya\Desktop\Front-end-Mobile-Autogestion-Sena\bin\Debug\net8.0-android\com.companyname.autogestionsena.maui-Signed.apk"

# 3. Instalar con permisos automáticos (Android 6.0+)
adb install -r -g "bin\Debug\net8.0-android\com.companyname.autogestionsena.maui-Signed.apk"

# 4. Si hay error de versión/firma, desinstalar primero:
adb uninstall com.companyname.autogestionsena.maui
adb install "bin\Debug\net8.0-android\com.companyname.autogestionsena.maui-Signed.apk"
```

#### Opciones de ADB Install

| Opción | Descripción |
|--------|-------------|
| `-r` | Reinstalar la app (reemplazar versión existente) |
| `-g` | Otorgar todos los permisos automáticamente |
| `-d` | Permitir downgrade (instalar versión anterior) |
| `-t` | Permitir APKs de test |

### 2.3 Método 2: Transferencia Manual por USB

Si no tienes ADB configurado o prefieres un método manual:

#### Paso 1: Copiar APK al dispositivo

```powershell
# Opción A: Usando ADB push
adb push "bin\Debug\net8.0-android\com.companyname.autogestionsena.maui-Signed.apk" /sdcard/Download/AutoGestion-SENA.apk

# Opción B: Usando Windows Explorer
# 1. Conectar el dispositivo por USB
# 2. Seleccionar "Transferir archivos" en el teléfono
# 3. Abrir el dispositivo en Windows Explorer
# 4. Copiar la APK a: Almacenamiento interno > Download
```

#### Paso 2: Instalar desde el dispositivo

1. Abrir la app **"Archivos"** o **"Gestor de archivos"** en el teléfono
2. Navegar a la carpeta **"Descargas"** o **"Download"**
3. Tocar el archivo **`AutoGestion-SENA.apk`**
4. Si aparece advertencia de fuentes desconocidas, tocar **"Configuración"**
5. Activar **"Permitir desde esta fuente"**
6. Volver y tocar **"Instalar"**
7. Una vez instalada, tocar **"Abrir"**

### 2.4 Método 3: Compartir por Nube o Red

Útil para distribuir a múltiples testers:

#### Opción A: Google Drive / OneDrive

```powershell
# 1. Subir la APK a Google Drive
# 2. Obtener enlace de compartir
# 3. Enviar enlace por correo/chat a los testers

# Los testers deben:
# - Abrir el enlace en el navegador del celular
# - Descargar la APK
# - Instalar desde Descargas
```

#### Opción B: Servidor interno / Red local

```powershell
# Iniciar servidor HTTP simple en Python
cd "bin\Debug\net8.0-android"
python -m http.server 8080

# Los testers acceden desde el navegador del celular:
# http://[IP_DEL_PC]:8080/com.companyname.autogestionsena.maui-Signed.apk
```

#### Opción C: Envío por correo electrónico

> ⚠️ **Nota:** Gmail y otros proveedores pueden bloquear archivos APK por seguridad. Se recomienda usar un archivo ZIP o servicios de almacenamiento en la nube.

### 2.5 Método 4: Instalación en Emulador Android

```powershell
# 1. Listar emuladores disponibles
emulator -list-avds

# 2. Iniciar el emulador
emulator -avd Pixel_5_API_34

# 3. Esperar a que el emulador inicie completamente

# 4. Instalar APK en el emulador
adb install "bin\Debug\net8.0-android\com.companyname.autogestionsena.maui-Signed.apk"

# Alternativa: Arrastrar y soltar
# - Simplemente arrastrar el archivo APK sobre la ventana del emulador
```

### 2.6 Habilitar "Orígenes Desconocidos" (Android 8.0+)

En Android 8.0 (Oreo) y versiones posteriores, los permisos de instalación se otorgan por aplicación:

1. Al intentar instalar la APK, aparecerá un mensaje de bloqueo
2. Tocar "Configuración" en el mensaje
3. Activar "Permitir desde esta fuente" para:
   - Chrome (si descargaste desde navegador)
   - Archivos (si instalas desde gestor de archivos)
   - Gmail (si recibiste por correo)
4. Volver y completar la instalación

### 2.7 Verificación de Instalación Exitosa

Una vez instalada la APK, verificar:

```powershell
# 1. Verificar que la app está instalada
adb shell pm list packages | Select-String "autogestionsena"
# Salida esperada: package:com.companyname.autogestionsena.maui

# 2. Obtener información de la app instalada
adb shell dumpsys package com.companyname.autogestionsena.maui | Select-String "versionName|versionCode"

# 3. Iniciar la aplicación desde ADB
adb shell am start -n com.companyname.autogestionsena.maui/crc64e1fb321c08285b90.MainActivity
```

#### Verificación Manual en el Dispositivo

1. ✅ Buscar el ícono **"AutoGestión SENA"** en el menú de aplicaciones
2. ✅ Abrir la aplicación
3. ✅ Verificar que carga la pantalla de login
4. ✅ Probar las funcionalidades básicas
5. ✅ Verificar conexión con el backend (si aplica)

### 2.8 Solución de Errores Comunes de Instalación

| Código de Error | Causa | Solución |
|-----------------|-------|----------|
| `INSTALL_FAILED_UPDATE_INCOMPATIBLE` | La APK tiene firma diferente a la versión instalada | `adb uninstall com.companyname.autogestionsena.maui` y reinstalar |
| `INSTALL_FAILED_VERSION_DOWNGRADE` | Intentas instalar versión anterior | Agregar flag `-d`: `adb install -r -d archivo.apk` |
| `INSTALL_PARSE_FAILED_NO_CERTIFICATES` | APK no está firmada | Usar el archivo `*-Signed.apk` en lugar del sin firmar |
| `INSTALL_FAILED_INSUFFICIENT_STORAGE` | No hay espacio en el dispositivo | Liberar espacio o usar `adb install` con almacenamiento externo |
| `INSTALL_FAILED_OLDER_SDK` | Versión de Android no compatible | Verificar que el dispositivo tenga Android 5.0+ |
| `INSTALL_FAILED_USER_RESTRICTED` | Restricciones de usuario/MDM | Verificar políticas de seguridad del dispositivo |

### 2.9 Script Completo de Instalación

```powershell
# ============================================
# Script: install-apk.ps1
# Propósito: Automatizar la instalación de APK
# ============================================

param(
    [string]$ApkPath = "bin\Debug\net8.0-android\com.companyname.autogestionsena.maui-Signed.apk",
    [switch]$Reinstall,
    [switch]$GrantPermissions
)

# Verificar que ADB está disponible
if (-not (Get-Command adb -ErrorAction SilentlyContinue)) {
    Write-Host "❌ ADB no encontrado. Verifica que Android SDK está en el PATH" -ForegroundColor Red
    exit 1
}

# Verificar dispositivo conectado
$devices = adb devices | Select-String "device$"
if (-not $devices) {
    Write-Host "❌ No hay dispositivos conectados" -ForegroundColor Red
    Write-Host "   Conecta un dispositivo con Depuración USB habilitada" -ForegroundColor Yellow
    exit 1
}

Write-Host "📱 Dispositivo detectado: $($devices -replace '\s+device','')" -ForegroundColor Green

# Verificar APK existe
if (-not (Test-Path $ApkPath)) {
    Write-Host "❌ APK no encontrada: $ApkPath" -ForegroundColor Red
    exit 1
}

$apkInfo = Get-Item $ApkPath
Write-Host "📦 APK: $($apkInfo.Name) ($([math]::Round($apkInfo.Length/1MB, 2)) MB)" -ForegroundColor Cyan

# Construir comando de instalación
$installCmd = "adb install"
if ($Reinstall) { $installCmd += " -r" }
if ($GrantPermissions) { $installCmd += " -g" }
$installCmd += " `"$ApkPath`""

Write-Host "⏳ Instalando..." -ForegroundColor Yellow
$result = Invoke-Expression $installCmd

if ($result -match "Success") {
    Write-Host "✅ Instalación exitosa!" -ForegroundColor Green
    Write-Host "🚀 Iniciando aplicación..." -ForegroundColor Cyan
    adb shell am start -n com.companyname.autogestionsena.maui/crc64e1fb321c08285b90.MainActivity
} else {
    Write-Host "❌ Error en la instalación:" -ForegroundColor Red
    Write-Host $result -ForegroundColor Yellow
}
```

**Uso del script:**

```powershell
# Instalación básica
.\install-apk.ps1

# Reinstalar y otorgar permisos
.\install-apk.ps1 -Reinstall -GrantPermissions

# APK específica
.\install-apk.ps1 -ApkPath "releases\AutoGestion-SENA-1.0.0-debug.apk"
```

---

## 📋 Resumen Rápido

### Compilar APK de Pruebas

```powershell
cd "C:\Users\braya\Desktop\Front-end-Mobile-Autogestion-Sena"
dotnet clean -f net8.0-android -c Debug
dotnet restore
dotnet build -f net8.0-android -c Debug
```

### Instalar en Dispositivo

```powershell
adb devices
adb install -r -g "bin\Debug\net8.0-android\com.companyname.autogestionsena.maui-Signed.apk"
```

---

*Documento generado: 27 de noviembre de 2025*
*Proyecto: AutoGestión SENA - .NET MAUI*
