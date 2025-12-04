# 🧪 Suite de Pruebas - AutoGestión SENA

## 📊 Resumen de Pruebas

| Tipo | Cantidad de Métodos | Casos de Test | Descripción |
|------|---------------------|---------------|-------------|
| **Unitarias (Mockeadas)** | 22 métodos | 112 casos | Pruebas de validadores sin dependencias externas |
| **Integración (API Real)** | 10 métodos | 10 casos | Pruebas que consumen endpoints del backend |
| **Total** | **32 métodos** | **122 casos** | - |

---

## 🏗️ Framework de Pruebas: xUnit

Este proyecto utiliza **xUnit** como framework de pruebas unitarias. xUnit es el framework de testing más popular para .NET moderno.

### ¿Por qué xUnit?
- ✅ Framework moderno y activamente mantenido
- ✅ Integración nativa con .NET CLI y Visual Studio
- ✅ Soporte para pruebas paralelas
- ✅ Sintaxis limpia y expresiva
- ✅ Extensible con assertions personalizadas (FluentAssertions)

---

## 🏷️ Etiquetas y Atributos de xUnit Usados

### `[Fact]` - Prueba Simple
Marca un método como una prueba unitaria **sin parámetros**. Se ejecuta una sola vez.

```csharp
[Fact]
public void Server_ShouldBeReachable()
{
    // Arrange, Act, Assert
    var response = await httpClient.GetAsync("/api/health");
    response.IsSuccessStatusCode.Should().BeTrue();
}
```

**Uso en el proyecto:** Pruebas de integración que verifican conectividad o comportamiento específico.

---

### `[Theory]` - Prueba Parametrizada
Marca un método como una prueba que se ejecuta **múltiples veces** con diferentes datos. Requiere un atributo de datos como `[InlineData]`.

```csharp
[Theory]
[InlineData("usuario@soy.sena.edu.co", true)]
[InlineData("", false)]
[InlineData(null, false)]
public void IsSenaEmail_ValidatesCorrectly(string? email, bool expected)
{
    var result = LoginValidator.IsSenaEmail(email);
    result.Should().Be(expected);
}
```

**Uso en el proyecto:** Mayoría de las pruebas unitarias para probar múltiples escenarios con un solo método.

---

### `[InlineData]` - Datos en Línea
Proporciona datos de prueba directamente en el atributo. Cada `[InlineData]` genera un caso de prueba separado.

```csharp
[Theory]
[InlineData("123456", true)]   // Caso 1: 6 dígitos válidos
[InlineData("12345", false)]   // Caso 2: 5 dígitos inválido
[InlineData("abcdef", false)]  // Caso 3: letras inválido
public void IsCodeValid(string code, bool expected) { ... }
```

**Uso en el proyecto:** Todas las pruebas `[Theory]` usan `[InlineData]` para definir casos de prueba.

---

### Patrón AAA (Arrange-Act-Assert)
Todas las pruebas siguen el patrón estándar:

```csharp
[Fact]
public void Example_Test()
{
    // Arrange - Preparar datos
    var email = "test@sena.edu.co";
    
    // Act - Ejecutar acción
    var result = LoginValidator.IsSenaEmail(email);
    
    // Assert - Verificar resultado
    result.Should().BeTrue();
}
```

---

### `#region` / `#endregion` - Organización de Código
Usamos regiones para agrupar y documentar las pruebas:

```csharp
#region Test 1: Validación de email SENA válido
[Theory]
[InlineData("usuario@soy.sena.edu.co", true)]
public void IsSenaEmail_WithValidSenaEmails_ReturnsTrue(...) { ... }
#endregion
```

---

## 📚 Librería FluentAssertions

Usamos **FluentAssertions** para hacer las aserciones más legibles:

| xUnit Tradicional | FluentAssertions |
|-------------------|------------------|
| `Assert.True(result)` | `result.Should().BeTrue()` |
| `Assert.False(result)` | `result.Should().BeFalse()` |
| `Assert.Equal(expected, actual)` | `actual.Should().Be(expected)` |
| `Assert.NotNull(obj)` | `obj.Should().NotBeNull()` |
| `Assert.Null(obj)` | `obj.Should().BeNull()` |
| `Assert.Contains(item, list)` | `list.Should().Contain(item)` |
| `Assert.NotEmpty(list)` | `list.Should().NotBeEmpty()` |
| `Assert.IsType<T>(obj)` | `obj.Should().BeOfType<T>()` |

---

## 🔧 Pruebas Unitarias (22 Métodos / 112 Casos)

### LoginValidatorUnitTests (5 métodos)
| # | Método de Test | Casos | Descripción |
|---|----------------|-------|-------------|
| 1 | `IsSenaEmail_WithValidSenaEmails_ReturnsTrue` | 3 | Valida emails institucionales SENA |
| 2 | `IsSenaEmail_WithInvalidEmails_ReturnsFalse` | 5 | Rechaza emails vacíos, null o no SENA |
| 3 | `IsPasswordValid_WithValidPassword_ReturnsTrue` | 3 | Acepta contraseñas válidas |
| 4 | `IsPasswordValid_WithEmptyOrNull_ReturnsFalse` | 3 | Rechaza contraseñas vacías |
| 5 | `Is2FACodeValid_ValidatesCorrectly` | 8 | Valida código 2FA de 6 dígitos |

### RegisterValidatorUnitTests (5 métodos)
| # | Método de Test | Casos | Descripción |
|---|----------------|-------|-------------|
| 6 | `IsEmailValid_ValidatesCorrectly` | 5 | Valida email requerido |
| 7 | `IsNameValid_ValidatesCorrectly` | 6 | Valida nombres requeridos |
| 8 | `IsDocumentNumberNumeric_ValidatesCorrectly` | 6 | Valida que documento sea numérico |
| 9 | `IsPhoneLengthValid_ValidatesCorrectly` | 7 | Valida longitud teléfono (7-10 dígitos) |
| 10 | `AreAllFieldsValid_ValidatesCorrectly` | 7 | Valida formulario completo de registro |

### CodeVerificationValidatorUnitTests (4 métodos)
| # | Método de Test | Casos | Descripción |
|---|----------------|-------|-------------|
| 11 | `IsCodeValid_ValidatesNotEmpty` | 5 | Valida código no vacío |
| 12 | `IsCode6DigitsValid_ValidatesExactly6Digits` | 8 | Valida exactamente 6 dígitos |
| 13 | `GetErrorMessages_ReturnsCorrectMessages` | 1 | Verifica mensajes de error |

### PasswordResetValidatorUnitTests (4 métodos)
| # | Método de Test | Casos | Descripción |
|---|----------------|-------|-------------|
| 14 | `IsPasswordValid_ValidatesNotEmpty` | 5 | Valida contraseña no vacía |
| 15 | `IsPasswordLengthValid_Validates8CharactersMinimum` | 7 | Valida mínimo 8 caracteres |
| 16 | `DoPasswordsMatch_ValidatesEquality` | 7 | Valida coincidencia de contraseñas |
| 17 | `IsPasswordStrong_ValidatesLetterAndDigit` | 8 | Valida fortaleza (letra + número) |

### TwoFactorValidatorUnitTests (4 métodos)
| # | Método de Test | Casos | Descripción |
|---|----------------|-------|-------------|
| 18 | `IsCodeComplete_ValidatesAll6Digits` | 6 | Valida 6 dígitos separados completos |
| 19 | `IsCodeComplete_WithNullDigit_ReturnsFalse` | 1 | Maneja dígitos null |
| 20 | `CombineCode_CreatesCorrectString` | 4 | Combina dígitos en string |
| 21 | `IsCodeNumeric_ValidatesAllAreDigits` | 7 | Valida que todos sean numéricos |

---

## 🌐 Pruebas de Integración (10 Tests)

### AuthenticationApiIntegrationTests (5 tests)
| # | Test | Endpoint | Descripción |
|---|------|----------|-------------|
| 1 | `Server_ShouldBeReachable` | `GET /security/document-types/` | Verifica conectividad |
| 2 | `GetDocumentTypes_ShouldReturnList` | `GET /security/document-types/` | Obtiene tipos de documento |
| 3 | `ValidateInstitutionalLogin_WithValidCredentials_ShouldSend2FACode` | `POST /security/users/validate-institutional-login/` | Login con credenciales válidas |
| 4 | `ValidateInstitutionalLogin_WithInvalidCredentials_ShouldReturnError` | `POST /security/users/validate-institutional-login/` | Login con credenciales inválidas |
| 5 | `Validate2FACode_WithInvalidCode_ShouldReturnError` | `POST /security/users/validate-2fa-code/` | Código 2FA inválido |

### DashboardApiIntegrationTests (5 tests)
| # | Test | Endpoint | Descripción |
|---|------|----------|-------------|
| 6 | `GetApprenticeDashboard_WithValidId_ShouldReturnData` | `GET /assign/request_asignation/aprendiz-dashboard/` | Dashboard aprendiz |
| 7 | `GetEnterprise_WithValidId_ShouldReturnEnterpriseData` | `GET /assign/enterprise/{id}/` | Datos de empresa |
| 8 | `GetModalityProductiveStage_ShouldReturnList` | `GET /assign/modality_productive_stage/` | Lista modalidades |
| 9 | `GetUserDetails_WithValidId_ShouldReturnUserData` | `GET /security/users/{id}/` | Datos de usuario |
| 10 | `GetMenuByUserId_ShouldReturnMenuStructure` | `GET /security/rol-form-permissions/{id}/get-menu/` | Menú por rol |

---

## 🚀 Comandos de Ejecución

### Ejecutar TODAS las pruebas
```powershell
cd Tests
dotnet test
```

### Ejecutar con detalle verbose
```powershell
dotnet test --verbosity detailed
```

### Ejecutar con información de resumen
```powershell
dotnet test --verbosity normal
```

---

## 🔍 Filtrar Pruebas Específicas

### Solo pruebas UNITARIAS (mockeadas)
```powershell
dotnet test --filter "FullyQualifiedName~UnitTests"
```

### Solo pruebas de INTEGRACIÓN (API real)
```powershell
dotnet test --filter "FullyQualifiedName~IntegrationTests"
```

### Por clase específica
```powershell
# Login
dotnet test --filter "FullyQualifiedName~LoginValidatorUnitTests"

# Registro
dotnet test --filter "FullyQualifiedName~RegisterValidatorUnitTests"

# Verificación de código
dotnet test --filter "FullyQualifiedName~CodeVerificationValidatorUnitTests"

# Reseteo de contraseña
dotnet test --filter "FullyQualifiedName~PasswordResetValidatorUnitTests"

# Two-Factor
dotnet test --filter "FullyQualifiedName~TwoFactorValidatorUnitTests"

# API de autenticación
dotnet test --filter "FullyQualifiedName~AuthenticationApiIntegrationTests"

# API de dashboard
dotnet test --filter "FullyQualifiedName~DashboardApiIntegrationTests"
```

### Por nombre de método
```powershell
# Todos los tests de email
dotnet test --filter "Name~Email"

# Todos los tests de password
dotnet test --filter "Name~Password"

# Todos los tests de código 2FA
dotnet test --filter "Name~Code"
```

---

## 📈 Reportes y Cobertura

### Ejecutar con reporte de cobertura
```powershell
dotnet test --collect:"XPlat Code Coverage"
```

### Listar todas las pruebas disponibles
```powershell
dotnet test --list-tests
```

### Generar resultados en formato TRX
```powershell
dotnet test --logger "trx;LogFileName=test-results.trx"
```

---

## ⚙️ Configuración

### URL Base del API
```
http://10.3.234.91:8001/api/
```

### Credenciales de Prueba
| Parámetro | Valor |
|-----------|-------|
| **Email** | `daniela_ramos@soy.sena.edu.co` |
| **Password** | `1032679504Y3` |
| **Apprentice ID** | `1` |
| **User ID** | `1` |
| **Enterprise ID** | `1` |

---

## 📦 Dependencias

```xml
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.9.0" />
<PackageReference Include="FluentAssertions" Version="8.8.0" />
<PackageReference Include="Moq" Version="4.20.70" />
<PackageReference Include="xunit" Version="2.6.6" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.5.6" />
<PackageReference Include="coverlet.collector" Version="6.0.0" />
```

---

## 📁 Estructura de Archivos

```
Tests/
├── AutogestionSena.Tests.csproj    # Proyecto de pruebas
├── README.md                        # Esta documentación
├── UnitTests/                       # Pruebas unitarias mockeadas
│   ├── LoginValidatorUnitTests.cs           (5 métodos / 22 casos)
│   ├── RegisterValidatorUnitTests.cs        (5 métodos / 31 casos)
│   ├── CodeVerificationValidatorUnitTests.cs (4 métodos / 14 casos)
│   ├── PasswordResetValidatorUnitTests.cs   (4 métodos / 27 casos)
│   └── TwoFactorValidatorUnitTests.cs       (4 métodos / 18 casos)
└── IntegrationTests/                # Pruebas de integración con API
    ├── AuthenticationApiIntegrationTests.cs (5 tests)
    └── DashboardApiIntegrationTests.cs      (5 tests)
```

---

## ✅ Resultados Esperados

Al ejecutar `dotnet test` deberías ver:

```
Pruebas totales: 122
     Correcto: 122
      Errores: 0
     Omitidas: 0

Compilación correcta. 0 Advertencia(s), 0 Errores
```

---

## ⚠️ Notas Importantes

1. **Pruebas de Integración**: Requieren conectividad con el servidor backend en `http://10.3.234.91:8001/api/`

2. **Pruebas Unitarias**: Son completamente **mockeadas** y no requieren conexión a internet ni backend.

3. **Encoding**: El proyecto usa UTF-8 para soportar caracteres especiales en español.

4. **Theory vs Fact**: 
   - Usa `[Fact]` cuando solo necesitas un caso de prueba
   - Usa `[Theory]` + `[InlineData]` cuando quieres probar múltiples escenarios

---

## 🔗 Referencias

- [Documentación oficial de xUnit](https://xunit.net/)
- [FluentAssertions](https://fluentassertions.com/)
- [.NET CLI - dotnet test](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-test)
