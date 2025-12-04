using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutogestionSena.MAUI.Api;

namespace AutogestionSena.MAUI.Api.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(Endpoints.API_BASE_URL),
                Timeout = TimeSpan.FromSeconds(30)
            };

            }

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Realiza una petición GET y deserializa la respuesta al tipo T
        /// </summary>
        public async Task<T?> GetAsync<T>(string endpoint)
        {
            try
            {
                bool isFullUrl = Uri.IsWellFormedUriString(endpoint, UriKind.Absolute);
                string finalUrl = isFullUrl ? endpoint : $"{_httpClient.BaseAddress}{endpoint}";

                HttpResponseMessage response;
                if (isFullUrl)
                {
                    response = await _httpClient.GetAsync(new Uri(finalUrl));
                }
                else
                {
                    response = await _httpClient.GetAsync(endpoint);
                }

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        throw new Exception($"Endpoint no encontrado: {finalUrl}. Verifica la URL del servidor.");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        throw new Exception($"Error interno del servidor. Contacta al administrador.");
                    }
                    else
                    {
                        throw new Exception($"Error {(int)response.StatusCode}: {errorContent}");
                    }
                }

                var content = await response.Content.ReadAsStringAsync();

                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                };

                var result = System.Text.Json.JsonSerializer.Deserialize<T>(content, options);

                return result;
            }
            catch (TaskCanceledException ex) when (ex.InnerException is System.Net.Sockets.SocketException)
            {
                throw new Exception("No se pudo conectar al servidor. Verifica tu conexión a internet y que la IP del servidor sea accesible desde tu dispositivo.", ex);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error de conexión: {ex.Message}. Verifica que el servidor esté ejecutándose en {Endpoints.API_BASE_URL}", ex);
            }
            catch (System.Text.Json.JsonException ex)
            {
                throw new Exception($"Error al procesar la respuesta del servidor. La respuesta no es JSON válido.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inesperado: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Realiza una petición GET y retorna el JSON crudo como string
        /// Útil cuando el formato de respuesta puede variar (array vs objeto)
        /// </summary>
        public async Task<string?> GetRawAsync(string endpoint)
        {
            try
            {
                bool isFullUrl = Uri.IsWellFormedUriString(endpoint, UriKind.Absolute);
                string finalUrl = isFullUrl ? endpoint : $"{_httpClient.BaseAddress}{endpoint}";

                var response = await _httpClient.GetAsync(finalUrl);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error {(int)response.StatusCode}: {content}");
                }

                return content;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Diagnostica la conexión al servidor
        /// </summary>
        public async Task<(bool isConnected, string message)> DiagnoseConnectionAsync()
        {
            try
            {
                using var testClient = new HttpClient(new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                })
                {
                    Timeout = TimeSpan.FromSeconds(10)
                };

                var testUrl = $"{Endpoints.API_BASE_URL}security/document-types/";
                var response = await testClient.GetAsync(testUrl);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Conexión exitosa al servidor");
                }
                else
                {
                    return (false, $"Servidor respondió con error {response.StatusCode}");
                }
            }
            catch (TaskCanceledException)
            {
                return (false, "Timeout de conexión. Verifica la IP del servidor y tu red.");
            }
            catch (HttpRequestException ex)
            {
                return (false, $"Error de red: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"Error inesperado: {ex.Message}");
            }
        }

        /// <summary>
        /// Realiza una petición POST con el payload y devuelve la respuesta deserializada
        /// </summary>
        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest payload)
        {
            try
            {
                bool isFullUrl = Uri.IsWellFormedUriString(endpoint, UriKind.Absolute);
                string logEndpoint = isFullUrl ? endpoint : $"{_httpClient.BaseAddress}{endpoint}";

                // Serializar manualmente con opciones específicas
                var jsonOptions = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = false
                };

                var jsonContent = System.Text.Json.JsonSerializer.Serialize(payload, jsonOptions);

                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response;
                if (isFullUrl)
                {
                    // Para URLs completas, usar Uri
                    response = await _httpClient.PostAsync(new Uri(endpoint), content);
                }
                else
                {
                    // Para endpoints relativos, usar el método normal
                    response = await _httpClient.PostAsync(endpoint, content);
                }

                
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(
                     $"Error {(int)response.StatusCode} ({response.StatusCode}): {responseContent}"
               );
                }

                // Intentar deserializar con manejo de errores mejorado
                try
                {
                    return System.Text.Json.JsonSerializer.Deserialize<TResponse>(responseContent, jsonOptions);
                }
                catch (System.Text.Json.JsonException jsonEx)
                {
                    throw new Exception($"Error al procesar la respuesta del servidor. Respuesta: {responseContent}", jsonEx);
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error en la petición POST a {endpoint}: {ex.Message}", ex);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Realiza una petición POST sin esperar respuesta deserializada
        /// </summary>
        public async Task<HttpResponseMessage> PostAsync<TRequest>(string endpoint, TRequest payload)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(endpoint, payload);
                response.EnsureSuccessStatusCode();
                return response;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error en la petición POST a {endpoint}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Realiza una petición PUT
        /// </summary>
        public async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest payload)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(endpoint, payload);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<TResponse>();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error en la petición PUT a {endpoint}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Realiza una petición DELETE
        /// </summary>
        public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(endpoint);
                response.EnsureSuccessStatusCode();
                return response;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error en la petición DELETE a {endpoint}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Configura el token de autenticación en los headers
        /// </summary>
        public void SetAuthToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        /// <summary>
        /// Limpia el token de autenticación
        /// </summary>
        public void ClearAuthToken()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        /// <summary>
        /// Intenta parsear una respuesta HttpResponseMessage a un objeto que contenga
        /// la información de éxito y detalle, independientemente de cómo el servidor la envíe.
        /// </summary>
        public async Task<AutogestionSena.MAUI.Api.Dtos.ApiResponseParsedDto> ParseApiResponseAsync(HttpResponseMessage response)
        {
            var result = new AutogestionSena.MAUI.Api.Dtos.ApiResponseParsedDto
            {
                StatusCode = (int)response.StatusCode,
                IsSuccessStatusCode = response.IsSuccessStatusCode,
                Success = response.IsSuccessStatusCode
            };

            if (response.Content == null)
                return result;

            string content = string.Empty;
            try
            {
                content = await response.Content.ReadAsStringAsync();
            }
            catch
            {
                return result;
            }

            if (string.IsNullOrWhiteSpace(content))
                return result;

            try
            {
                using var doc = System.Text.Json.JsonDocument.Parse(content);
                var root = doc.RootElement;

                // Helper to find property case-insensitively
                bool TryGetPropertyIgnoreCase(System.Text.Json.JsonElement element, string propertyName, out System.Text.Json.JsonElement found)
                {
                    foreach (var p in element.EnumerateObject())
                    {
                        if (string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase))
                        {
                            found = p.Value;
                            return true;
                        }
                    }
                    found = default;
                    return false;
                }

                // Try 'success' field
                if (TryGetPropertyIgnoreCase(root, "success", out var successProp))
                {
                    switch (successProp.ValueKind)
                    {
                        case System.Text.Json.JsonValueKind.True:
                        case System.Text.Json.JsonValueKind.False:
                            result.Success = successProp.GetBoolean();
                            break;
                        case System.Text.Json.JsonValueKind.String:
                            var s = successProp.GetString();
                            if (bool.TryParse(s, out var b)) result.Success = b;
                            break;
                        case System.Text.Json.JsonValueKind.Number:
                            if (successProp.TryGetInt32(out var i)) result.Success = i != 0;
                            break;
                    }
                }

                // Try to obtain a detail/message
                string? GetFirstStringProperty(System.Text.Json.JsonElement element, params string[] names)
                {
                    foreach (var name in names)
                    {
                        if (TryGetPropertyIgnoreCase(element, name, out var prop))
                        {
                            if (prop.ValueKind == System.Text.Json.JsonValueKind.String)
                                return prop.GetString();
                            else
                                return prop.ToString();
                        }
                    }
                    return null;
                }

                var detail = GetFirstStringProperty(root, "detail", "message", "error", "errors", "description");
                if (!string.IsNullOrEmpty(detail)) result.Detail = detail;
                else
                {
                    // If not found but top-level is a string or primitive, take whole content
                    if (root.ValueKind == System.Text.Json.JsonValueKind.String)
                        result.Detail = root.GetString();
                    else
                        result.Detail = content;
                }
            }
            catch (System.Text.Json.JsonException)
            {
                // Not a JSON document; default to raw content
                result.Detail = content;
            }
            catch (Exception)
            {
                // Ignore parsing errors
            }

            return result;
        }
    }
}
