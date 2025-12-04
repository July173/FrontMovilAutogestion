# Arquitectura y Estructura del Proyecto Autogestión SENA (MAUI)

## Visión General
- **Framework**: .NET MAUI
- **Lenguaje**: C#
- **Patrón de Diseño**: MVVM (Model-View-ViewModel)
- **Propósito**: Aplicación móvil para la autogestión de servicios SENA.

## Estructura de Carpetas

### 📂 Api
- **Propósito**: Capa de integración y comunicación de datos.
- **Función**: Centraliza toda la interacción con el backend y servicios externos. Aquí se definen los contratos de datos (DTOs), la configuración de endpoints y la implementación de los servicios HTTP, aislando la lógica de conexión del resto de la aplicación.
    - **📂 config/**: Almacena configuraciones globales de la API, como URLs base, endpoints específicos y constantes de conexión.
    - **📂 Dtos/**: (Data Transfer Objects) Define las estructuras de datos simples utilizadas para transportar información entre la aplicación móvil y la API, sin lógica de negocio.
    - **📂 Services/**: Contiene la implementación concreta de los servicios que realizan las peticiones HTTP (GET, POST, etc.), manejando la serialización y deserialización de datos.

### 📂 ContentViews
- **Propósito**: Modularización de la Interfaz de Usuario.
- **Función**: Contiene controles de usuario (UserControls) y fragmentos de UI reutilizables. Su objetivo es encapsular elementos visuales complejos (como menús, tarjetas o encabezados) para evitar la duplicación de código XAML y facilitar el mantenimiento del diseño en múltiples vistas.

### 📂 Converters
- **Propósito**: Adaptación de datos para la Vista.
- **Función**: Alberga la lógica de transformación necesaria para el Data Binding. Convierte tipos de datos del backend o lógica de negocio (como booleanos o estados) en propiedades visuales comprensibles para la UI (como visibilidad, colores o texto), actuando como puente entre el ViewModel y la Vista.

### 📂 Helpers
- **Propósito**: Utilidades transversales y soporte.
- **Función**: Agrupa clases estáticas y servicios auxiliares que son utilizados por múltiples capas de la aplicación. Incluye lógica para la navegación centralizada, manejo de eventos globales, definiciones de iconos y otras herramientas que no pertenecen a un dominio de negocio específico.

### 📂 Validators
- **Propósito**: Aseguramiento de integridad de datos.
- **Función**: Contiene las reglas de negocio y lógica de validación. Se encarga de verificar que la información ingresada por el usuario cumpla con los requisitos del sistema (formatos, obligatoriedad, reglas institucionales) antes de ser procesada o enviada al servidor.

### 📂 ViewModels
- **Propósito**: Lógica de presentación y gestión de estado (MVVM).
- **Función**: Actúa como el cerebro de las vistas. Desacopla la interfaz gráfica de la lógica de negocio, gestionando el estado de la pantalla, manejando los comandos del usuario y orquestando la comunicación entre la Vista y los servicios de datos (Api).
    - **📂 dasboard/**: Contiene los ViewModels que gestionan la lógica y el estado de las vistas del dashboard.
    - **📂 Security/**: Agrupa los ViewModels encargados de la lógica de autenticación y seguridad.

### 📂 Views
- **Propósito**: Estructura visual y experiencia de usuario.
- **Función**: Define las pantallas completas de la aplicación. Es la capa responsable de la renderización gráfica (XAML) y la interacción directa con el usuario, sirviendo como contenedor para los ContentViews y punto de enlace con los ViewModels.
    - **📂 dashboard/**: Agrupa las vistas relacionadas con el panel de control principal y sus funcionalidades específicas.
    - **📂 login/**: Contiene las pantallas dedicadas al proceso de autenticación y recuperación de acceso.
    - **📂 Security/**: Vistas relacionadas con la seguridad, gestión de claves o configuraciones sensibles.

### 📂 Tests
- **Propósito**: Garantía de calidad y verificación.
- **Función**: Aloja el proyecto de pruebas automatizadas. Su función es validar de manera aislada el correcto funcionamiento de los componentes lógicos, validadores y servicios, asegurando que los cambios en el código no introduzcan regresiones o errores.

### 📂 Resources
- **Propósito**: Gestión de activos y estilos visuales.
- **Función**: Centraliza los recursos estáticos de la aplicación. Organiza imágenes, fuentes, iconos y diccionarios de recursos (estilos, colores, temas) que definen la identidad visual, permitiendo una gestión uniforme de la apariencia en toda la app.
    - **📂 AppIcon/**: Contiene los archivos de icono de la aplicación para las diferentes plataformas y resoluciones.
    - **📂 Images/**: Almacena imágenes generales y activos gráficos utilizados en la interfaz de usuario (logos, ilustraciones).
    - **📂 Raw/**: Carpeta para archivos "crudos" (como JSON, texto, o bases de datos locales iniciales) que se necesitan acceder tal cual.
    - **📂 Splash/**: Recursos específicos para la pantalla de inicio (Splash Screen) que se muestra al cargar la aplicación.
    - **📂 Styles/**: Define los diccionarios de recursos XAML globales, incluyendo paletas de colores, estilos de controles y tipografías para mantener la consistencia visual.

### 📂 Platforms
- **Propósito**: Abstracción y especificidad de plataforma.
- **Función**: Contiene el código y configuración nativa para cada sistema operativo soportado (Android, iOS, Windows, Mac). Permite implementar funcionalidades que requieren acceso directo a APIs del dispositivo o ajustes específicos para cada entorno de ejecución.
- **Android/**: Configuraciones y código nativo para Android.
- **iOS/**: Configuraciones y código nativo para iOS.
- **Windows/**: Configuraciones para Windows (WinUI).
- **MacCatalyst/**: Soporte para macOS.

## Flujo de Arquitectura (MVVM)

### 1. Vista (View)
- El usuario interactúa con la UI (XAML).
- Realiza Data Binding con el ViewModel.

### 2. Modelo de Vista (ViewModel)
- Recibe comandos de la Vista.
- Ejecuta validaciones usando **Validators**.
- Llama a **Api/Services** para obtener o enviar datos.
- Actualiza propiedades observables que refrescan la Vista.

### 3. Modelo / Servicios (Model)
- **Dtos** definen la estructura de los datos.
- **Services** se comunican con el Backend.
