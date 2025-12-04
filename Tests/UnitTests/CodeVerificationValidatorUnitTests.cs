using Xunit;
using FluentAssertions;
using AutogestionSena.MAUI.Validators;

namespace AutogestionSena.Tests.UnitTests
{
    /// <summary>
    /// Pruebas unitarias para CodeVerificationValidator
    /// Total: 3 pruebas unitarias mockeadas
    /// </summary>
    public class CodeVerificationValidatorUnitTests
    {
        #region Test 1: Validación de código no vacío
        [Theory]
        [InlineData("123456", true)]
        [InlineData("abc", true)]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData("   ", false)]
        public void IsCodeValid_ValidatesNotEmpty(string? code, bool expected)
        {
            // Act
            var result = CodeVerificationValidator.IsCodeValid(code);

            // Assert
            result.Should().Be(expected);
        }
        #endregion

        #region Test 2: Validación de código con 6 dígitos exactos
        [Theory]
        [InlineData("123456", true)]
        [InlineData("000000", true)]
        [InlineData("999999", true)]
        [InlineData("12345", false)]  // 5 dígitos
        [InlineData("1234567", false)] // 7 dígitos
        [InlineData("12345a", false)] // Contiene letra
        [InlineData("", false)]
        [InlineData(null, false)]
        public void IsCode6DigitsValid_ValidatesExactly6Digits(string? code, bool expected)
        {
            // Act
            var result = CodeVerificationValidator.IsCode6DigitsValid(code);

            // Assert
            result.Should().Be(expected);
        }
        #endregion

        #region Test 3: Mensajes de error correctos
        [Fact]
        public void GetErrorMessages_ReturnsCorrectMessages()
        {
            // Act
            var emptyMessage = CodeVerificationValidator.GetCodeEmptyErrorMessage();
            var invalidMessage = CodeVerificationValidator.GetCodeInvalidErrorMessage();

            // Assert
            emptyMessage.Should().NotBeNullOrEmpty();
            emptyMessage.Should().Contain("código");
            invalidMessage.Should().NotBeNullOrEmpty();
            invalidMessage.Should().Contain("6 dígitos");
        }
        #endregion
    }
}
