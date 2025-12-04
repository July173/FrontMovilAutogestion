using Xunit;
using FluentAssertions;
using AutogestionSena.MAUI.Validators;

namespace AutogestionSena.Tests.UnitTests
{
    /// <summary>
    /// Pruebas unitarias para LoginValidator
    /// Total: 5 pruebas unitarias mockeadas
    /// </summary>
    public class LoginValidatorUnitTests
    {
        #region Test 1: Validación de email SENA válido
        [Theory]
        [InlineData("usuario@soy.sena.edu.co", true)]
        [InlineData("admin@sena.edu.co", true)]
        [InlineData("test.user@soy.sena.edu.co", true)]
        public void IsSenaEmail_WithValidSenaEmails_ReturnsTrue(string email, bool expected)
        {
            // Act
            var result = LoginValidator.IsSenaEmail(email);

            // Assert
            result.Should().Be(expected);
        }
        #endregion

        #region Test 2: Validación de email inválido (vacío, null o no SENA)
        [Theory]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData("   ", false)]
        [InlineData("user@gmail.com", false)] // No es SENA
        [InlineData("user", false)]           // No es email válido
        public void IsSenaEmail_WithInvalidEmails_ReturnsFalse(string? email, bool expected)
        {
            // Act
            var result = LoginValidator.IsSenaEmail(email);

            // Assert
            result.Should().Be(expected);
        }
        #endregion

        #region Test 3: Validación de contraseña válida
        [Theory]
        [InlineData("Password123!", true)]
        [InlineData("miContrasena", true)]
        [InlineData("123456", true)]
        public void IsPasswordValid_WithValidPassword_ReturnsTrue(string password, bool expected)
        {
            // Act
            var result = LoginValidator.IsPasswordValid(password);

            // Assert
            result.Should().Be(expected);
        }
        #endregion

        #region Test 4: Validación de contraseña inválida
        [Theory]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData("   ", false)]
        public void IsPasswordValid_WithEmptyOrNull_ReturnsFalse(string? password, bool expected)
        {
            // Act
            var result = LoginValidator.IsPasswordValid(password);

            // Assert
            result.Should().Be(expected);
        }
        #endregion

        #region Test 5: Validación de código 2FA
        [Theory]
        [InlineData("123456", true)]  // 6 dígitos exactos
        [InlineData("000000", true)]  // Solo ceros
        [InlineData("999999", true)]  // Solo nueves
        [InlineData("12345", false)]  // 5 dígitos - inválido
        [InlineData("1234567", false)] // 7 dígitos - inválido
        [InlineData("abcdef", false)] // Letras - inválido
        [InlineData("", false)]       // Vacío - inválido
        [InlineData(null, false)]     // Null - inválido
        public void Is2FACodeValid_ValidatesCorrectly(string? code, bool expected)
        {
            // Act
            var result = LoginValidator.Is2FACodeValid(code);

            // Assert
            result.Should().Be(expected);
        }
        #endregion
    }
}
