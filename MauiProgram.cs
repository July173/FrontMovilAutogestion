using AutogestionSena.MAUI;
using AutogestionSena.MAUI.Services;
using AutogestionSenaMaui.Helpers;
using Microsoft.Extensions.Logging;
using Microcharts.Maui;

namespace AutogestionSena.MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("bootstrap-icons.woff", "BootstrapIcons");
                })
                .UseMicrocharts();

            // Configurar logging
#if DEBUG
            builder.Logging.AddDebug();
            builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
#else
            builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Warning);
#endif

            // Registrar servicio de logging como singleton
            builder.Services.AddSingleton<LoggingService>(sp => LoggingService.Instance);

            // Configurar manejo de excepciones no controladas
            ConfigureExceptionHandling();

            // Nota: RouteMap ya no es necesario con la arquitectura simplificada
            // Ahora usamos solo LoginPage (pública) y HomePage (protegida)
            // HomePage carga dinámicamente el dashboard apropiado según el rol del usuario

            var app = builder.Build();
            
            LoggingService.Instance.Info("Aplicación MAUI iniciada", "MauiProgram");
            
            return app;
        }

        private static void ConfigureExceptionHandling()
        {
            // Capturar excepciones no controladas en el hilo principal
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                var exception = args.ExceptionObject as Exception;
                LoggingService.Instance.Critical(
                    "Excepción no controlada en AppDomain",
                    "AppDomain.UnhandledException",
                    exception
                );
            };

            // Capturar excepciones no controladas en tareas
            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                LoggingService.Instance.Critical(
                    "Excepción no observada en Task",
                    "TaskScheduler.UnobservedTaskException",
                    args.Exception
                );
                args.SetObserved(); // Prevenir que la app se cierre
            };
        }
    }
}
