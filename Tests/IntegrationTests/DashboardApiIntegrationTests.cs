using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Xunit;
using FluentAssertions;

namespace AutogestionSena.Tests.IntegrationTests
{
    /// <summary>
    /// Pruebas de integración para el API de Dashboard del Aprendiz
    /// Estas pruebas consumen endpoints reales del backend
    /// Total: 5 pruebas de integración
    /// </summary>
    [Collection("Integration")]
    public class DashboardApiIntegrationTests : IDisposable
    {
        private readonly HttpClient _httpClient;
        private const string BASE_URL = "http://167.114.98.199/api/";
        
        // IDs de prueba (existentes en el sistema)
        private const int TEST_APPRENTICE_ID = 1;
        private const int TEST_ENTERPRISE_ID = 1;
        private const int TEST_USER_ID = 1;

        public DashboardApiIntegrationTests()
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

        #region Test 1: Obtener dashboard del aprendiz
        [Fact]
        public async Task GetApprenticeDashboard_WithValidId_ShouldReturnData()
        {
            // Arrange
            var endpoint = $"assign/request_asignation/aprendiz-dashboard/?aprendiz_id={TEST_APPRENTICE_ID}";

            // Act
            var response = await _httpClient.GetAsync(endpoint);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue($"Status: {response.StatusCode}");
            content.Should().NotBeNullOrEmpty();
            
            // Puede ser array o objeto según el estado
            var trimmedContent = content.Trim();
            (trimmedContent.StartsWith("[") || trimmedContent.StartsWith("{")).Should().BeTrue();
        }
        #endregion

        #region Test 2: Obtener datos de empresa
        [Fact]
        public async Task GetEnterprise_WithValidId_ShouldReturnEnterpriseData()
        {
            // Arrange
            var endpoint = $"assign/enterprise/{TEST_ENTERPRISE_ID}/";

            // Act
            var response = await _httpClient.GetAsync(endpoint);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue($"Status: {response.StatusCode}");
            content.Should().NotBeNullOrEmpty();
            content.Should().Contain("id");
            
            // Verificar campos de empresa (puede tener name_enterprise o name)
            if (response.IsSuccessStatusCode)
            {
                var hasName = content.Contains("name_enterprise") || content.Contains("name");
                hasName.Should().BeTrue("Debería contener información de nombre de empresa");
            }
        }
        #endregion

        #region Test 3: Obtener modalidades de etapa productiva
        [Fact]
        public async Task GetModalityProductiveStage_ShouldReturnList()
        {
            // Arrange
            var endpoint = "assign/modality_productive_stage/";

            // Act
            var response = await _httpClient.GetAsync(endpoint);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue($"Status: {response.StatusCode}");
            content.Should().NotBeNullOrEmpty();
            content.TrimStart().Should().StartWith("[", "Debería retornar un array de modalidades");
        }
        #endregion

        #region Test 4: Obtener datos completos del usuario
        [Fact]
        public async Task GetUserDetails_WithValidId_ShouldReturnUserData()
        {
            // Arrange
            var endpoint = $"security/users/{TEST_USER_ID}/";

            // Act
            var response = await _httpClient.GetAsync(endpoint);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue($"Status: {response.StatusCode}");
            content.Should().NotBeNullOrEmpty();
            content.Should().Contain("email");
            content.Should().Contain("person");
            content.Should().Contain("role");
        }
        #endregion

        #region Test 5: Obtener menú por rol de usuario
        [Fact]
        public async Task GetMenuByUserId_ShouldReturnMenuStructure()
        {
            // Arrange
            var endpoint = $"security/rol-form-permissions/{TEST_USER_ID}/get-menu/";

            // Act
            var response = await _httpClient.GetAsync(endpoint);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue($"Status: {response.StatusCode}");
            content.Should().NotBeNullOrEmpty();
            content.TrimStart().Should().StartWith("[", "Debería retornar un array de menú");
            content.Should().Contain("moduleForm");
        }
        #endregion
    }
}
