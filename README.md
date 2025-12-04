# Aplicación MAUI

Repositorio de ejemplo que contiene una aplicación multiplataforma creada con .NET MAUI.

## Resumen

Proyecto MAUI que incluye vistas para autenticación, perfil y manejo de experiencias. Está preparado para compilar en Android, iOS, MacCatalyst y Windows (net8.0).

---

## Requisitos previos

- .NET SDK 8.0 (verifica con `dotnet --version` — debe comenzar con `8.`).
- Herramienta de MAUI: instala las workloads necesarias:

  ```powershell
  dotnet workload install maui
  dotnet workload restore
  ```

- IDE recomendado: Visual Studio con la carga de trabajo ".NET MAUI" instalada (Visual Studio para Windows o Visual Studio para Mac). Alternativamente puedes usar la CLI `dotnet`.
- Para Android: Android SDK + emulador o un dispositivo físico con depuración habilitada.
- Para iOS/MacCatalyst: macOS con Xcode (necesario para compilar/desplegar en iOS/Mac).
- Para Windows: Windows 10/11 con SDK mínimo 10.0.17763.0 (este proyecto incluye `net8.0-windows10.0.19041.0` como target).

## Estructura importante

- `Services/ApiService.cs` — contiene el cliente HTTP hacia el backend. Actualmente usa la URL base:

  ```csharp
  http://10.0.2.2:5062/api/
  ```

  - Nota: `10.0.2.2` es la IP que permite al emulador Android acceder al host (máquina local) donde corre un backend en `localhost`.
  - Si ejecutas el backend en otra máquina o en producción, cambia la `BaseAddress` en `ApiService` o crea una configuración que lea la URL desde un archivo/variable de entorno.

## Pasos para ejecutar (modo seguro, sin reescribir historial)

1. Clonar el repositorio:

	```powershell
	git clone 
	```

2. Comprobar versión de .NET y restaurar paquetes:

	```powershell
	dotnet --version
	dotnet restore
	```

3. (Opcional, recomendado) Instalar workloads MAUI si aún no lo has hecho:

	```powershell
	dotnet workload install maui
	```

4. Ejecutar en Windows (si estás en Windows y tienes Visual Studio o los workloads instalados):

	```powershell
	dotnet build -f net8.0-windows10.0.19041.0 -c Debug
	dotnet run -f net8.0-windows10.0.19041.0
	```

	O abre la solución `.sln` en Visual Studio y selecciona `Start` con `Windows Machine`.

5. Ejecutar en Android:

	### Opción A: Usando Visual Studio (recomendado)

	1. Abre el archivo `.sln` en Visual Studio
	2. En la barra de herramientas superior, selecciona:
	   - **Framework**: `net8.0-android`
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
	   dotnet build -f net8.0-android -c Debug
	   ```

	3. **Ejecutar en el dispositivo/emulador:**
	   ```powershell
	   dotnet build -t:Run -f net8.0-android

	   dotnet watch run -f net8.0-android
	   ```
	   O usando la tarea configurada:
	   ```powershell
	   dotnet build -t:Run -f net8.0-android "c:\Users\braya\Desktop\Front-end-Mobile-Autogestion-Sena\AutogestionSenaMaui.csproj"
	   ```

	4. **Generar APK para distribución (Release):**
	   ```powershell
	   # El proyecto ya está configurado para generar APK por defecto
	   dotnet publish -f net8.0-android -c Release
	   ```
	   El APK estará en: `bin/Release/net8.0-android/publish/com.companyname.autogestionsena.maui-Signed.apk`

	   **Comandos alternativos:**
	   ```powershell
	   # Forzar formato APK (si el .csproj no tiene la configuración)
	   dotnet publish -f net8.0-android -c Release /p:AndroidPackageFormat=apk

	   # Generar AAB para Google Play Store
	   dotnet publish -f net8.0-android -c Release /p:AndroidPackageFormat=aab
	   ```

	5. **Instalar APK manualmente (opcional):**
	   ```powershell
	   adb install -r ./publish/android/com.companyname.autogestionsenamaui-Signed.apk
	   ```

	### Opción C: Usando tareas de VS Code

	Si tienes VS Code con las tareas configuradas:
	```powershell
	# Ejecutar la tarea run-android
	```
	Presiona `Ctrl+Shift+P` → `Tasks: Run Task` → `run-android`

	### Solución de problemas Android:

	- **Error "No se encuentra adb"**: Instala Android SDK y agrega `platform-tools` al PATH
	- **Error "No devices/emulators found"**: Inicia un emulador AVD desde Android Studio o conecta un dispositivo físico
	- **Error de firma**: Asegúrate de tener configurado el keystore en `AutogestionSenaMaui.csproj`
	- **App no se conecta al backend**: Cambia la URL en `ApiService.cs` de `10.0.2.2` a la IP de tu máquina local (ej: `192.168.1.100`) si usas dispositivo físico

6. iOS / MacCatalyst: requiere macOS y Xcode; abre la solución en Visual Studio para Mac o usa `dotnet` desde macOS con los workloads instalados.

## Notas sobre `ApiService` y backend

- Si pruebas en el emulador Android y tu backend corre en tu máquina local en el puerto `5062`, deja `http://10.0.2.2:5062/api/`.
- Para ejecutar con un dispositivo físico o en Windows, cambia la `BaseAddress` a la URL o IP accesible desde el dispositivo.
- Asegúrate de que el backend acepte CORS desde la app si haces llamadas desde un origen diferente.

## Problemas comunes y soluciones rápidas

- Error al pushear a `main`: verifica si la rama está protegida en GitHub; en ese caso crea un Pull Request o desprotege temporalmente la rama.
- Errores en la compilación de Android: instala y actualiza el Android SDK y crea un AVD con una API >= la versión mínima especificada en el `.csproj`.
- iOS sólo en macOS: necesitarás un Mac con Xcode y permisos adecuados.

## Contacto

Si necesitas ayuda adicional, deja una issue en el repositorio o contacta al mantenedor.
---

Archivo generado y actualizado con instrucciones básicas de ejecución y notas específicas del proyecto.

