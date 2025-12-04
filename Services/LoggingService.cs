using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace AutogestionSena.MAUI.Services
{
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error,
        Critical
    }

    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public LogLevel Level { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public Exception Exception { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] ");
            sb.Append($"[{Level}] ");
            if (!string.IsNullOrEmpty(Source))
                sb.Append($"[{Source}] ");
            sb.Append(Message);
            
            if (Exception != null)
            {
                sb.AppendLine();
                sb.Append($"Exception: {Exception.GetType().Name}: {Exception.Message}");
                sb.AppendLine();
                sb.Append($"StackTrace: {Exception.StackTrace}");
            }
            
            return sb.ToString();
        }

        public string GetColorCode()
        {
            return Level switch
            {
                LogLevel.Debug => "#6C757D",
                LogLevel.Info => "#0D6EFD",
                LogLevel.Warning => "#FFC107",
                LogLevel.Error => "#DC3545",
                LogLevel.Critical => "#8B0000",
                _ => "#000000"
            };
        }
    }

    public class LoggingService
    {
        private static LoggingService _instance;
        private static readonly object _lock = new object();
        private readonly ObservableCollection<LogEntry> _logs;
        private readonly string _logFilePath;
        private const int MaxLogsInMemory = 500;

        public static LoggingService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new LoggingService();
                        }
                    }
                }
                return _instance;
            }
        }

        public ObservableCollection<LogEntry> Logs => _logs;

        private LoggingService()
        {
            _logs = new ObservableCollection<LogEntry>();
            
            // Configurar ruta del archivo de log
            var appDataPath = FileSystem.AppDataDirectory;
            _logFilePath = Path.Combine(appDataPath, "app_logs.txt");
            
            Log(LogLevel.Info, "Sistema de logging inicializado", "LoggingService");
        }

        public void Log(LogLevel level, string message, string source = null, Exception exception = null)
        {
            try
            {
                var logEntry = new LogEntry
                {
                    Timestamp = DateTime.Now,
                    Level = level,
                    Message = message,
                    Source = source ?? "App",
                    Exception = exception
                };

                // Agregar a colección en memoria
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    _logs.Insert(0, logEntry);
                    
                    // Mantener solo los últimos N logs en memoria
                    if (_logs.Count > MaxLogsInMemory)
                    {
                        _logs.RemoveAt(_logs.Count - 1);
                    }
                });

                // Escribir a archivo
                WriteToFile(logEntry);

                // Escribir a consola de debug
                System.Diagnostics.Debug.WriteLine(logEntry.ToString());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al escribir log: {ex.Message}");
            }
        }

        private void WriteToFile(LogEntry logEntry)
        {
            try
            {
                File.AppendAllText(_logFilePath, logEntry.ToString() + Environment.NewLine);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al escribir archivo de log: {ex.Message}");
            }
        }

        public void Debug(string message, string source = null)
        {
            Log(LogLevel.Debug, message, source);
        }

        public void Info(string message, string source = null)
        {
            Log(LogLevel.Info, message, source);
        }

        public void Warning(string message, string source = null, Exception exception = null)
        {
            Log(LogLevel.Warning, message, source, exception);
        }

        public void Error(string message, string source = null, Exception exception = null)
        {
            Log(LogLevel.Error, message, source, exception);
        }

        public void Critical(string message, string source = null, Exception exception = null)
        {
            Log(LogLevel.Critical, message, source, exception);
        }

        public async Task<string> ExportLogsAsync()
        {
            try
            {
                if (File.Exists(_logFilePath))
                {
                    var content = await File.ReadAllTextAsync(_logFilePath);
                    return content;
                }
                return "No hay logs disponibles";
            }
            catch (Exception ex)
            {
                return $"Error al exportar logs: {ex.Message}";
            }
        }

        public void ClearLogs()
        {
            try
            {
                MainThread.BeginInvokeOnMainThread(() => _logs.Clear());
                
                if (File.Exists(_logFilePath))
                {
                    File.Delete(_logFilePath);
                }
                
                Log(LogLevel.Info, "Logs limpiados", "LoggingService");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al limpiar logs: {ex.Message}");
            }
        }

        public string GetLogFilePath() => _logFilePath;
    }
}
