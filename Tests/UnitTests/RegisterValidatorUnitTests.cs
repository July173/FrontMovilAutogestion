using Xunit;
using FluentAssertions;
using AutogestionSena.MAUI.Validators;

namespace AutogestionSena.Tests.UnitTests
{
    /// <summary>
    /// Pruebas unitarias para RegisterValidator
    /// Total: 5 pruebas unitarias mockeadas
    /// </summary>
    public class RegisterValidatorUnitTests
    {
        #region Test 1: Validación de email requerido
        [Theory]
        [InlineData("usuario@soy.sena.edu.co", true)]
        [InlineData("test@gmail.com", true)]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData("   ", false)]
        public void IsEmailValid_ValidatesCorrectly(string? email, bool expected)
        {
            // Act
            var result = RegisterValidator.IsEmailValid(email);

            // Assert
            result.Should().Be(expected);
        }
        #endregion

        #region Test 2: Validación de nombres
        [Theory]
        [InlineData("Juan", true)]
        [InlineData("María José", true)]
        [InlineData("Carlos Andrés", true)]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData("   ", false)]
        public void IsNameValid_ValidatesCorrectly(string? name, bool expected)
        {
            // Act
            var result = RegisterValidator.IsNameValid(name);

            // Assert
            result.Should().Be(expected);
        }
        #endregion

        #region Test 3: Validación de número de documento numérico
        [Theory]
        [InlineData("1234567890", true)]
        [InlineData("12345678", true)]
        [InlineData("1234567ABC", false)] // Contiene letras
        [InlineData("12-345-678", false)] // Contiene guiones
        [InlineData("", false)]
        [InlineData(null, false)]
        public void IsDocumentNumberNumeric_ValidatesCorrectly(string? document, bool expected)
        {
            // Act
            var result = RegisterValidator.IsDocumentNumberNumeric(document);

            // Assert
            result.Should().Be(expected);
        }
        #endregion

        #region Test 4: Validación de teléfono colombiano
        [Theory]
        [InlineData("3001234567", true)]  // 10 dígitos - válido
        [InlineData("3125647896", true)]  // 10 dígitos - válido
        [InlineData("1234567", true)]     // 7 dígitos - fijo válido
        [InlineData("123456", false)]     // 6 dígitos - muy corto
        [InlineData("12345678901", false)] // 11 dígitos - muy largo
        [InlineData("", false)]
        [InlineData(null, false)]
        public void IsPhoneLengthValid_ValidatesCorrectly(string? phone, bool expected)
        {
            // Act
            var result = RegisterValidator.IsPhoneLengthValid(phone);

            // Assert
            result.Should().Be(expected);
        }
        #endregion

        #region Test 5: Validación de todos los campos del registro
        [Theory]
        [InlineData("usuario@soy.sena.edu.co", "Juan", "Perez", "1234567890", "3001234567", true, true)]
        [InlineData("", "Juan", "Perez", "1234567890", "3001234567", true, false)] // Email vacío
        [InlineData("usuario@soy.sena.edu.co", "", "Perez", "1234567890", "3001234567", true, false)] // Nombre vacío
        [InlineData("usuario@soy.sena.edu.co", "Juan", "", "1234567890", "3001234567", true, false)] // Apellido vacío
        [InlineData("usuario@soy.sena.edu.co", "Juan", "Perez", "", "3001234567", true, false)] // Documento vacío
        [InlineData("usuario@soy.sena.edu.co", "Juan", "Perez", "1234567890", "", true, false)] // Teléfono vacío
        [InlineData("usuario@soy.sena.edu.co", "Juan", "Perez", "1234567890", "3001234567", false, false)] // Sin tipo documento
        public void AreAllFieldsValid_ValidatesCorrectly(
            string email, string firstName, string lastName, 
            string doc, string phone, bool hasDocumentType, bool expected)
        {
            // Act
            var result = RegisterValidator.AreAllFieldsValid(email, firstName, lastName, doc, phone, hasDocumentType);

            // Assert
            result.Should().Be(expected);
        }
        #endregion
    }
}
