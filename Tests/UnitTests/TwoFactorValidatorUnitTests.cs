using Xunit;
using FluentAssertions;
using AutogestionSena.MAUI.Validators;

namespace AutogestionSena.Tests.UnitTests
{
    /// <summary>
    /// Pruebas unitarias para TwoFactorValidator
    /// Total: 3 pruebas unitarias mockeadas
    /// </summary>
    public class TwoFactorValidatorUnitTests
    {
        #region Test 1: Validación de código completo (6 dígitos separados)
        [Theory]
        [InlineData("1", "2", "3", "4", "5", "6", true)]
        [InlineData("0", "0", "0", "0", "0", "0", true)]
        [InlineData("9", "9", "9", "9", "9", "9", true)]
        [InlineData("", "2", "3", "4", "5", "6", false)]  // Primer dígito vacío
        [InlineData("1", "", "3", "4", "5", "6", false)]  // Segundo dígito vacío
        [InlineData("1", "2", "3", "4", "5", "", false)]  // Último dígito vacío
        public void IsCodeComplete_ValidatesAll6Digits(
            string c1, string c2, string c3, string c4, string c5, string c6, bool expected)
        {
            // Act
            var result = TwoFactorValidator.IsCodeComplete(c1, c2, c3, c4, c5, c6);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void IsCodeComplete_WithNullDigit_ReturnsFalse()
        {
            // Act
            var result = TwoFactorValidator.IsCodeComplete("1", "2", "3", "4", "5", null!);

            // Assert
            result.Should().BeFalse();
        }
        #endregion

        #region Test 2: Combinación de dígitos en código completo
        [Theory]
        [InlineData("1", "2", "3", "4", "5", "6", "123456")]
        [InlineData("0", "0", "0", "0", "0", "0", "000000")]
        [InlineData("9", "8", "7", "6", "5", "4", "987654")]
        [InlineData("3", "2", "9", "6", "4", "1", "329641")]
        public void CombineCode_CreatesCorrectString(
            string c1, string c2, string c3, string c4, string c5, string c6, string expected)
        {
            // Act
            var result = TwoFactorValidator.CombineCode(c1, c2, c3, c4, c5, c6);

            // Assert
            result.Should().Be(expected);
        }
        #endregion

        #region Test 3: Validación de dígitos numéricos
        [Theory]
        [InlineData("1", "2", "3", "4", "5", "6", true)]
        [InlineData("0", "0", "0", "0", "0", "0", true)]
        [InlineData("a", "2", "3", "4", "5", "6", false)]  // Letra en primer dígito
        [InlineData("1", "b", "3", "4", "5", "6", false)]  // Letra en segundo dígito
        [InlineData("1", "2", "3", "4", "5", "z", false)]  // Letra en último dígito
        [InlineData("!", "2", "3", "4", "5", "6", false)]  // Símbolo
        [InlineData("12", "2", "3", "4", "5", "6", false)] // Dos dígitos en una posición
        public void IsCodeNumeric_ValidatesAllAreDigits(
            string c1, string c2, string c3, string c4, string c5, string c6, bool expected)
        {
            // Act
            var result = TwoFactorValidator.IsCodeNumeric(c1, c2, c3, c4, c5, c6);

            // Assert
            result.Should().Be(expected);
        }
        #endregion
    }
}
