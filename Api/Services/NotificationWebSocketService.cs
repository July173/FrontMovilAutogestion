using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutogestionSena.MAUI.Api.Dtos;
using Microsoft.Maui.ApplicationModel;

namespace AutogestionSena.MAUI.Api.Services
{
    public class NotificationWebSocketService : IDisposable
    {
        private ClientWebSocket? _socket;
        private CancellationTokenSource? _cts;
        private Task? _receivingTask;
        private readonly TimeSpan _reconnectDelay = TimeSpan.FromSeconds(5);

        public event EventHandler<NotificationDto>? NotificationReceived;
        public event EventHandler<string>? ErrorOccurred;

        public bool IsConnected => _socket != null && _socket.State == WebSocketState.Open;

        public NotificationWebSocketService()
        {
        }

        /// <summary>
        /// Construye la URL websocket a partir de la API base (convierte http(s) a ws(s) y /api/ a /ws/notifications/)
        /// </summary>
        private string BuildWebSocketUrl()
        {
            var apiBase = Endpoints.API_BASE_URL; // e.g. http://host:8000/api/
            if (string.IsNullOrEmpty(apiBase)) return string.Empty;

            // ensure trailing slash
            if (!apiBase.EndsWith("/")) apiBase += "/";

            // replace scheme
            string wsBase;
            if (apiBase.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                wsBase = "wss://" + apiBase.Substring("https://".Length);
            else if (apiBase.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                wsBase = "ws://" + apiBase.Substring("http://".Length);
            else
                wsBase = apiBase;

            // replace /api/ with /ws/notifications/ (best-effort)
            wsBase = wsBase.Replace("/api/", "/ws/notifications/", StringComparison.OrdinalIgnoreCase);

            return wsBase;
        }

        /// <summary>
        /// Inicia la conexi�n websocket. Se toma el token de Preferences("AuthToken") y se lo env�a como query param token.
        /// </summary>
        public async Task StartAsync()
        {
            if (IsConnected) return;

            _cts = new CancellationTokenSource();

            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    _socket = new ClientWebSocket();

                    // Agregar headers si es necesario (no todos los servidores aceptan headers en ws)
                    var token = Preferences.Get("AuthToken", string.Empty);
                    var wsUrl = BuildWebSocketUrl();
                    if (!string.IsNullOrEmpty(token))
                    {
                        // agregar token en query string
                        var separator = wsUrl.Contains("?") ? "&" : "?";
                        wsUrl = wsUrl + separator + "token=" + Uri.EscapeDataString(token);
                    }

                    await _socket.ConnectAsync(new Uri(wsUrl), _cts.Token);

                    if (_socket.State == WebSocketState.Open)
                    {
                        _receivingTask = Task.Run(() => ReceiveLoopAsync(_socket, _cts.Token));
                        return;
                    }
                }
                catch (Exception ex)
                {
                    ErrorOccurred?.Invoke(this, ex.Message);
                }

                // esperar antes de reintentar
                await Task.Delay(_reconnectDelay);
            }
        }

        private async Task ReceiveLoopAsync(ClientWebSocket socket, CancellationToken ct)
        {
            var buffer = new byte[8192];
            try
            {
                while (!ct.IsCancellationRequested && socket.State == WebSocketState.Open)
                {
                    var segment = new ArraySegment<byte>(buffer);
                    WebSocketReceiveResult? result = null;
                    using var ms = new System.IO.MemoryStream();
                    do
                    {
                        result = await socket.ReceiveAsync(segment, ct);
                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", ct);
                            break;
                        }

                        ms.Write(segment.Array!, segment.Offset, result.Count);
                    } while (!result.EndOfMessage);

                    ms.Seek(0, System.IO.SeekOrigin.Begin);
                    string message = Encoding.UTF8.GetString(ms.ToArray());
                    System.Diagnostics.Debug.WriteLine($"[WS] Mensaje recibido: {message}");

                    // Intentar deserializar a NotificationDto (si el backend env�a otro formato, ajustar)
                    try
                    {
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        NotificationDto? dto = null;

                        // si el mensaje viene envuelto, intentar obtener propiedad 'notification'
                        using var doc = JsonDocument.Parse(message);
                        var root = doc.RootElement;
                        if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("notification", out var notifProp))
                        {
                            dto = JsonSerializer.Deserialize<NotificationDto>(notifProp.GetRawText(), options);
                        }
                        else if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("data", out var dataProp) && dataProp.ValueKind == JsonValueKind.Object && dataProp.TryGetProperty("notification", out var notif2))
                        {
                            dto = JsonSerializer.Deserialize<NotificationDto>(notif2.GetRawText(), options);
                        }
                        else
                        {
                            // intentar deserializar directamente
                            dto = JsonSerializer.Deserialize<NotificationDto>(message, options);
                        }

                        if (dto != null)
                        {
                            NotificationReceived?.Invoke(this, dto);
                        }
                    }
                    catch (JsonException jex)
                    {
                        ErrorOccurred?.Invoke(this, jex.Message);
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, ex.Message);
            }
            finally
            {
                try
                {
                    if (socket.State != WebSocketState.Closed && socket.State != WebSocketState.Aborted)
                        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                }
                catch { }

                // intentar reconectar autom�ticamente si no se cancel� expl�citamente
if (_cts != null && !_cts.IsCancellationRequested)
                {
                    await Task.Delay(_reconnectDelay);
                    await StartAsync();
                }
            }
        }

        public async Task StopAsync()
        {
            try
            {
                _cts?.Cancel();
                if (_socket != null)
                {
                    if (_socket.State == WebSocketState.Open)
                        await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closing", CancellationToken.None);

                    _socket.Dispose();
                    _socket = null;
                }
            }
            catch (Exception)
            {
                // Ignore stop errors
            }
        }

        public void Dispose()
        {
            try
            {
                _cts?.Cancel();
                _socket?.Dispose();
                _cts?.Dispose();
            }
            catch { }
        }
    }
}
