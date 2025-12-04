using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AutogestionSena.MAUI.Services;

namespace AutogestionSena.MAUI.ViewModels
{
    public class LogItemViewModel : INotifyPropertyChanged
    {
        public DateTime Timestamp { get; set; }
        public LogLevel Level { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public Exception Exception { get; set; }

        public string ColorCode => Level switch
        {
            LogLevel.Debug => "#6C757D",
            LogLevel.Info => "#0D6EFD",
            LogLevel.Warning => "#FFC107",
            LogLevel.Error => "#DC3545",
            LogLevel.Critical => "#8B0000",
            _ => "#000000"
        };

        public bool HasException => Exception != null;

        public string ExceptionText => Exception != null 
            ? $"{Exception.GetType().Name}: {Exception.Message}\n{Exception.StackTrace}" 
            : string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class LogsViewModel : INotifyPropertyChanged
    {
        private readonly LoggingService _loggingService;
        private ObservableCollection<LogItemViewModel> _logs;
        private string _logFilePath;
        private string _logCountText;

        public ObservableCollection<LogItemViewModel> Logs
        {
            get => _logs;
            set
            {
                _logs = value;
                OnPropertyChanged();
                UpdateLogCount();
            }
        }

        public string LogFilePath
        {
            get => _logFilePath;
            set
            {
                _logFilePath = value;
                OnPropertyChanged();
            }
        }

        public string LogCountText
        {
            get => _logCountText;
            set
            {
                _logCountText = value;
                OnPropertyChanged();
            }
        }

        public ICommand RefreshCommand { get; }
        public ICommand ExportLogsCommand { get; }
        public ICommand ClearLogsCommand { get; }

        public LogsViewModel()
        {
            _loggingService = LoggingService.Instance;
            _logs = new ObservableCollection<LogItemViewModel>();

            RefreshCommand = new Command(RefreshLogs);
            ExportLogsCommand = new Command(async () => await ExportLogsAsync());
            ClearLogsCommand = new Command(ClearLogs);

            LogFilePath = _loggingService.GetLogFilePath();
            
            RefreshLogs();
            
            // Suscribirse a cambios en la colección de logs
            _loggingService.Logs.CollectionChanged += (s, e) => RefreshLogs();
        }

        private void RefreshLogs()
        {
            try
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Logs.Clear();
                    foreach (var log in _loggingService.Logs)
                    {
                        Logs.Add(new LogItemViewModel
                        {
                            Timestamp = log.Timestamp,
                            Level = log.Level,
                            Message = log.Message,
                            Source = log.Source,
                            Exception = log.Exception
                        });
                    }
                    UpdateLogCount();
                });
            }
            catch (Exception ex)
            {
                _loggingService.Error("Error al refrescar logs en UI", "LogsViewModel", ex);
            }
        }

        private async Task ExportLogsAsync()
        {
            try
            {
                var logs = await _loggingService.ExportLogsAsync();
                
                // Copiar al clipboard
                await Clipboard.SetTextAsync(logs);
                
                await Application.Current.MainPage.DisplayAlert(
                    "Logs Exportados",
                    "Los logs se han copiado al portapapeles. También puedes encontrarlos en:\n" + LogFilePath,
                    "OK");
                
                _loggingService.Info("Logs exportados exitosamente", "LogsViewModel");
            }
            catch (Exception ex)
            {
                _loggingService.Error("Error al exportar logs", "LogsViewModel", ex);
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    $"No se pudieron exportar los logs: {ex.Message}",
                    "OK");
            }
        }

        private async void ClearLogs()
        {
            try
            {
                bool confirm = await Application.Current.MainPage.DisplayAlert(
                    "Confirmar",
                    "¿Estás seguro de que deseas limpiar todos los logs?",
                    "Sí",
                    "No");

                if (confirm)
                {
                    _loggingService.ClearLogs();
                    RefreshLogs();
                    
                    await Application.Current.MainPage.DisplayAlert(
                        "Logs Limpiados",
                        "Todos los logs han sido eliminados",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                _loggingService.Error("Error al limpiar logs", "LogsViewModel", ex);
            }
        }

        private void UpdateLogCount()
        {
            LogCountText = $"Total de logs: {Logs.Count}";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
