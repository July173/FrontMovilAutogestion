using Xunit;
using FluentAssertions;
using AutogestionSena.MAUI.Validators;

namespace AutogestionSena.Tests.UnitTests
{
    /// <summary>
    /// Pruebas unitarias para PasswordResetValidator
    /// Total: 4 pruebas unitarias mockeadas
    /// </summary>
    public class PasswordResetValidatorUnitTests
    {
        #region Test 1: Validación de contraseña no vacía
        [Theory]
        [InlineData("Password123", true)]
        [InlineData("miContraseña", true)]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData("   ", false)]
        public void IsPasswordValid_ValidatesNotEmpty(string? password, bool expected)
        {
            // Act
            var result = PasswordResetValidator.IsPasswordValid(password);

            // Assert
            result.Should().Be(expected);
        }
        #endregion

        #region Test 2: Validación de longitud mínima (8 caracteres)
        [Theory]
        [InlineData("12345678", true)]   // Exactamente 8
        [InlineData("123456789", true)]  // Más de 8
        [InlineData("contraseñaLarga123", true)]
        [InlineData("1234567", false)]   // Solo 7
        [InlineData("abc", false)]       // Solo 3
        [InlineData("", false)]
        [InlineData(null, false)]
        public void IsPasswordLengthValid_Validates8CharactersMinimum(string? password, bool expected)
        {
            // Act
            var result = PasswordResetValidator.IsPasswordLengthValid(password);

            // Assert
            result.Should().Be(expected);
        }
        #endregion

        #region Test 3: Validación de coincidencia de contraseñas
        [Theory]
        [InlineData("Password123", "Password123", true)]
        [InlineData("miContraseña", "miContraseña", true)]
        [InlineData("Password123", "password123", false)] // Diferente case
        [InlineData("Password123", "Password124", false)] // Diferente número
        [InlineData("Password123", "", false)]
        [InlineData("", "Password123", false)]
        [InlineData(null, null, false)]
        public void DoPasswordsMatch_ValidatesEquality(string? password, string? confirmPassword, bool expected)
        {
            // Act
            var result = PasswordResetValidator.DoPasswordsMatch(password, confirmPassword);

            // Assert
            result.Should().Be(expected);
        }
        #endregion

        #region Test 4: Validación de fortaleza (letra + número)
        [Theory]
        [InlineData("Password1", true)]   // Letra y número
        [InlineData("abc123", true)]      // Letra y número
        [InlineData("A1", true)]          // Mínimo: letra y número
        [InlineData("password", false)]   // Solo letras
        [InlineData("12345678", false)]   // Solo números
        [InlineData("!!@@##$$", false)]   // Solo símbolos
        [InlineData("", false)]
        [InlineData(null, false)]
        public void IsPasswordStrong_ValidatesLetterAndDigit(string? password, bool expected)
        {
            // Act
            var result = PasswordResetValidator.IsPasswordStrong(password);

            // Assert
            result.Should().Be(expected);
        }
        #endregion
    }
}
