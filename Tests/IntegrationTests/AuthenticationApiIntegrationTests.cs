using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Xunit;
using FluentAssertions;

namespace AutogestionSena.Tests.IntegrationTests
{
    /// <summary>
    /// Pruebas de integración para el API de Autenticación
    /// Estas pruebas consumen endpoints reales del backend
    /// Total: 5 pruebas de integración
    /// </summary>
    [Collection("Integration")]
    public class AuthenticationApiIntegrationTests : IDisposable
    {
        private readonly HttpClient _httpClient;
        private const string BASE_URL = "http://167.114.98.199/api/";
        
        // Credenciales de prueba (usuario existente en el sistema)
        private const string TEST_EMAIL = "daniela_ramos@soy.sena.edu.co";
        private const string TEST_PASSWORD = "1032679504Y3";

        public AuthenticationApiIntegrationTests()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BASE_URL),
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        #region Test 1: Verificar conectividad con el servidor
        [Fact]
        public async Task Server_ShouldBeReachable()
        {
            // Arrange
            var endpoint = "security/document-types/";

            // Act
            var response = await _httpClient.GetAsync(endpoint);

            // Assert
            response.Should().NotBeNull();
            response.IsSuccessStatusCode.Should().BeTrue($"El servidor debería responder correctamente. Status: {response.StatusCode}");
        }
        #endregion

        #region Test 2: Obtener tipos de documento
        [Fact]
        public async Task GetDocumentTypes_ShouldReturnList()
        {
            // Arrange
            var endpoint = "security/document-types/";

            // Act
            var response = await _httpClient.GetAsync(endpoint);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            content.Should().NotBeNullOrEmpty();
            content.Should().Contain("id");
            content.Should().Contain("name");
            
            // Verificar que sea un array JSON
            content.TrimStart().Should().StartWith("[");
        }
        #endregion

        #region Test 3: Login institucional - Envío de código 2FA
        [Fact]
        public async Task ValidateInstitutionalLogin_WithValidCredentials_ShouldSend2FACode()
        {
            // Arrange
            var endpoint = "security/users/validate-institutional-login/";
            var payload = new
            {
                email = TEST_EMAIL,
                password = TEST_PASSWORD
            };
            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _httpClient.PostAsync(endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue($"Status: {response.StatusCode}, Content: {responseContent}");
            responseContent.Should().Contain("success");
            // Verificar que contiene mensaje de verificación (en español con ó)
            responseContent.ToLower().Should().Contain("verificaci");
        }
        #endregion

        #region Test 4: Login institucional - Credenciales inválidas
        [Fact]
        public async Task ValidateInstitutionalLogin_WithInvalidCredentials_ShouldReturnError()
        {
            // Arrange
            var endpoint = "security/users/validate-institutional-login/";
            var payload = new
            {
                email = "usuario_inexistente@soy.sena.edu.co",
                password = "contraseña_incorrecta"
            };
            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _httpClient.PostAsync(endpoint, content);

            // Assert
            // Puede ser 400 (Bad Request) o 401 (Unauthorized) o 404 (Not Found)
            response.IsSuccessStatusCode.Should().BeFalse("Credenciales inválidas no deberían ser aceptadas");
        }
        #endregion

        #region Test 5: Validación de código 2FA - Código inválido
        [Fact]
        public async Task Validate2FACode_WithInvalidCode_ShouldReturnError()
        {
            // Arrange
            var endpoint = "security/users/validate-2fa-code/";
            var payload = new
            {
                email = TEST_EMAIL,
                code = "000000" // Código inválido
            };
            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _httpClient.PostAsync(endpoint, content);

            // Assert
            // Código incorrecto debería retornar error
            response.IsSuccessStatusCode.Should().BeFalse("Código 2FA inválido no debería ser aceptado");
        }
        #endregion
    }
}
