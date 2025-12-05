# Aplicación MAUI - AutoGestión SENA

Repositorio de ejemplo que contiene una aplicación multiplataforma creada con .NET MAUI.

## Resumen

Proyecto MAUI que incluye vistas para autenticación, perfil y manejo de experiencias. Está preparado para compilar en Android y Windows.

---

## 🆕 Actualización a .NET 9 (diciembre 2025)

### ¿Por qué se actualizó de .NET 8 a .NET 9?

El proyecto fue actualizado de .NET 8 a .NET 9 por las siguientes razones:

1. **Compatibilidad con SDK .NET 10**: El SDK .NET 10.0.100 instalado marcaba las cargas de trabajo de .NET 8 MAUI como "End of Life" (EOL), lo que generaba errores de compilación (`NETSDK1202`).

2. **Mejor rendimiento**: .NET 9 incluye mejoras significativas de rendimiento en el runtime y en MAUI.

3. **Nuevas características**: Acceso a las últimas APIs y mejoras de .NET MAUI 9.

4. **Soporte activo**: .NET 9 es la versión con soporte activo (Standard Term Support).

### Cambios realizados

| Componente | Versión Anterior | Versión Nueva |
|:-----------|:----------------:|:-------------:|
| Target Framework (Android) | net8.0-android | net9.0-android |
| Target Framework (Windows) | net8.0-windows10.0.19041.0 | net9.0-windows10.0.19041.0 |
| Microsoft.Maui.Controls | 8.0.100 | 9.0.50 |
| SkiaSharp | 2.88.9-preview.2.2 | 3.116.1 |
| Microsoft.Extensions.Logging.Debug | 8.0.1 | 9.0.0 |

### Notas importantes post-actualización

- El control `Frame` está marcado como obsoleto en .NET 9. Se recomienda migrar a `Border` en futuras actualizaciones.
- Se agregó LiveChartsCore para gráficas (reemplazando Microcharts que no era compatible con .NET 8+).

---

## Requisitos previos

- **.NET SDK 9.0 o superior** (verifica con `dotnet --version` — debe comenzar con `9.` o superior).
- Herramienta de MAUI: instala las workloads necesarias:

  ```powershell
  dotnet workload install maui
  dotnet workload restore
  ```

- IDE recomendado: Visual Studio 2022 (17.12+) con la carga de trabajo ".NET MAUI" instalada. Alternativamente puedes usar la CLI `dotnet`.
- Para Android: Android SDK + emulador o un dispositivo físico con depuración habilitada.
- Para iOS/MacCatalyst: macOS con Xcode (necesario para compilar/desplegar en iOS/Mac).
- Para Windows: Windows 10/11 con SDK mínimo 10.0.19041.0.

## Estructura importante

- `Services/ApiService.cs` — contiene el cliente HTTP hacia el backend. Actualmente usa la URL base:

  ```csharp
  http://10.0.2.2:5062/api/
  ```

  - Nota: `10.0.2.2` es la IP que permite al emulador Android acceder al host (máquina local) donde corre un backend en `localhost`.
  - Si ejecutas el backend en otra máquina o en producción, cambia la `BaseAddress` en `ApiService` o crea una configuración que lea la URL desde un archivo/variable de entorno.

## Pasos para ejecutar

1. Clonar el repositorio:

	```powershell
	git clone https://github.com/July173/FrontMovilAutogestion.git
	cd FrontMovilAutogestion
	```

2. Comprobar versión de .NET y restaurar paquetes:

	```powershell
	dotnet --version   # Debe ser 9.0 o superior
	dotnet restore
	```

3. (Opcional, recomendado) Instalar workloads MAUI si aún no lo has hecho:

	```powershell
	dotnet workload install maui
	```

4. **Ejecutar en Windows**:

	```powershell
	dotnet build -f net9.0-windows10.0.19041.0 -c Debug
	dotnet run -f net9.0-windows10.0.19041.0
	```

	O abre la solución `.sln` en Visual Studio y selecciona `Start` con `Windows Machine`.

5. **Ejecutar en Android**:

	### Opción A: Usando Visual Studio (recomendado)

	1. Abre el archivo `.sln` en Visual Studio
	2. En la barra de herramientas superior, selecciona:
	   - **Framework**: `net9.0-android`
	   - **Dispositivo**: Elige un emulador Android o dispositivo físico conectado
	3. Presiona `F5` o el botón "Play" verde para ejecutar

	### Opción B: Usando la CLI de .NET

	**Prerequisitos:**
	- Android SDK instalado (verifica con `adb --version`)
	- Emulador Android corriendo o dispositivo físico conectado con depuración USB habilitada
	- Variables de entorno configuradas: `ANDROID_HOME`, `JAVA_HOME`

	**Pasos:**

	1. **Listar dispositivos disponibles:**
	   ```powershell
	   adb devices
	   ```
	   Deberías ver tu emulador o dispositivo listado.

	2. **Compilar el proyecto para Android:**
	   ```powershell
	   dotnet build -f net9.0-android -c Debug
	   ```

	3. **Ejecutar en el dispositivo/emulador:**
	   ```powershell
	   dotnet build -t:Run -f net9.0-android
	   ```

	4. **Generar APK para distribución (Release):**
	   ```powershell
	   # El proyecto ya está configurado para generar APK por defecto
	   dotnet publish -f net9.0-android -c Release
	   ```
	   El APK estará en: `bin/Release/net9.0-android/publish/com.sena.autogestion-Signed.apk`

	   **Comandos alternativos:**
	   ```powershell
	   # Forzar formato APK (si el .csproj no tiene la configuración)
	   dotnet publish -f net9.0-android -c Release /p:AndroidPackageFormat=apk

	   # Generar AAB para Google Play Store
	   dotnet publish -f net9.0-android -c Release /p:AndroidPackageFormat=aab
	   ```

	5. **Instalar APK manualmente (opcional):**
	   ```powershell
	   adb install -r ./bin/Release/net9.0-android/publish/com.sena.autogestion-Signed.apk
	   ```

	### Opción C: Usando tareas de VS Code

	Si tienes VS Code con las tareas configuradas:
	Presiona `Ctrl+Shift+P` → `Tasks: Run Task` → `run-android`

	### Solución de problemas Android:

	- **Error "No se encuentra adb"**: Instala Android SDK y agrega `platform-tools` al PATH
	- **Error "No devices/emulators found"**: Inicia un emulador AVD desde Android Studio o conecta un dispositivo físico
	- **Error de firma**: Asegúrate de tener configurado el keystore en `AutogestionSenaMaui.csproj`
	- **App no se conecta al backend**: Cambia la URL en `ApiService.cs` de `10.0.2.2` a la IP de tu máquina local (ej: `192.168.1.100`) si usas dispositivo físico

6. **iOS / MacCatalyst**: requiere macOS y Xcode; abre la solución en Visual Studio para Mac o usa `dotnet` desde macOS con los workloads instalados.

## Notas sobre `ApiService` y backend

- Si pruebas en el emulador Android y tu backend corre en tu máquina local en el puerto `5062`, deja `http://10.0.2.2:5062/api/`.
- Para ejecutar con un dispositivo físico o en Windows, cambia la `BaseAddress` a la URL o IP accesible desde el dispositivo.
- Asegúrate de que el backend acepte CORS desde la app si haces llamadas desde un origen diferente.

## Problemas comunes y soluciones rápidas

| Problema | Solución |
|:---------|:---------|
| Error `NETSDK1202` (workload EOL) | Actualiza a .NET 9 o instala el SDK correspondiente |
| Error al pushear a `main` | Verifica si la rama está protegida; crea un Pull Request |
| Errores de compilación Android | Instala/actualiza Android SDK y crea un AVD con API >= 21 |
| iOS sólo en macOS | Necesitas un Mac con Xcode |
| `Frame` obsoleto warning | Es normal en .NET 9, considera migrar a `Border` |

## Historial de versiones

| Versión | Fecha | Cambios |
|:--------|:------|:--------|
| 2.0.0 | Junio 2025 | Actualización a .NET 9, nuevos paquetes NuGet, LiveChartsCore |
| 1.0.0 | - | Versión inicial con .NET 8 |

## Contacto

Si necesitas ayuda adicional, deja una issue en el repositorio o contacta al mantenedor.

---

*Documentación actualizada tras la migración a .NET 9 - diciembre 2025*

