using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace AutogestionSenaMaui.ViewModels
{
    /// <summary>
    /// ViewModel para MainLayoutPage y TopBar
    /// Maneja breadcrumb navigation, notificaciones y datos compartidos del layout
    /// Similar al estado en MainLayout.tsx de React
    /// </summary>
    public class MainLayoutViewModel : INotifyPropertyChanged
    {
        private string _activeModule = string.Empty;
        private string _activeFormName = string.Empty;
        private bool _hasUnreadNotifications = false;
        private int _unreadNotificationsCount = 0;
        private bool _showAppVersion = true;
        private string _appVersion = string.Empty;

        public MainLayoutViewModel()
        {
            NotificationsCommand = new Command(OnNotificationsTapped);
            LoadAppVersion();
        }

        #region Properties

        /// <summary>
        /// Nombre del módulo activo en el breadcrumb
        /// </summary>
        public string ActiveModule
        {
            get => _activeModule;
            set
            {
                if (_activeModule != value)
                {
                    _activeModule = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Nombre del formulario activo en el breadcrumb
        /// </summary>
        public string ActiveFormName
        {
            get => _activeFormName;
            set
            {
                if (_activeFormName != value)
                {
                    _activeFormName = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Indica si hay notificaciones sin leer
        /// </summary>
        public bool HasUnreadNotifications
        {
            get => _hasUnreadNotifications;
            set
            {
                if (_hasUnreadNotifications != value)
                {
                    _hasUnreadNotifications = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Cantidad de notificaciones sin leer
        /// </summary>
        public int UnreadNotificationsCount
        {
            get => _unreadNotificationsCount;
            set
            {
                if (_unreadNotificationsCount != value)
                {
                    _unreadNotificationsCount = value;
                    OnPropertyChanged();
                    HasUnreadNotifications = value > 0;
                }
            }
        }

        /// <summary>
        /// Indica si se muestra la versión de la app en el footer
        /// </summary>
        public bool ShowAppVersion
        {
            get => _showAppVersion;
            set
            {
                if (_showAppVersion != value)
                {
                    _showAppVersion = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Versión de la aplicación
        /// </summary>
        public string AppVersion
        {
            get => _appVersion;
            set
            {
                if (_appVersion != value)
                {
                    _appVersion = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Commands

        public ICommand NotificationsCommand { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Maneja el tap en el icono de notificaciones
        /// </summary>
        private async void OnNotificationsTapped()
        {
            try
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Notificaciones",
                        $"Tienes {UnreadNotificationsCount} notificación(es) pendiente(s)",
                        "OK");
                }
            }
            catch (Exception)
            {
                // Notifications display error handled silently
            }
        }

        /// <summary>
        /// Carga la versión de la aplicación desde AppInfo
        /// </summary>
        private void LoadAppVersion()
        {
            try
            {
                var version = AppInfo.Current.VersionString;
                AppVersion = $"v{version}";
            }
            catch (Exception)
            {
                AppVersion = "v1.0.0";
            }
        }

        /// <summary>
        /// Actualiza el breadcrumb con módulo y formulario activos
        /// Similar a handleMenuItemClick en MainLayout.tsx
        /// </summary>
        /// <param name="moduleName">Nombre del módulo</param>
        /// <param name="formName">Nombre del formulario</param>
        public void UpdateBreadcrumb(string moduleName, string formName)
        {
            ActiveModule = moduleName;
            ActiveFormName = formName;
        }

        /// <summary>
        /// Actualiza el contador de notificaciones
        /// Puede ser llamado desde servicios de notificaciones o eventos
        /// </summary>
        /// <param name="count">Cantidad de notificaciones sin leer</param>
        public void UpdateNotificationCount(int count)
        {
            UnreadNotificationsCount = count;
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
}
