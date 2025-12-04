using System;
using System.Linq;

namespace AutogestionSena.MAUI.Validators
{
    /// <summary>
    /// Validador para la página de verificación de código (CodeVerificationPage)
    /// Valida códigos de recuperación y verificación de 2FA
    /// </summary>
    public static class CodeVerificationValidator
    {
        /// <summary>
        /// Valida que el código no esté vacío
        /// </summary>
        /// <param name="code">El código de verificación a validar</param>
        /// <returns>true si el código no está vacío ni null, false en caso contrario</returns>
        public static bool IsCodeValid(string? code)
        {
            return !string.IsNullOrWhiteSpace(code);
        }

        /// <summary>
        /// Valida que el código tenga exactamente 6 dígitos numéricos
        /// </summary>
        /// <param name="code">El código de verificación a validar</param>
        /// <returns>true si el código tiene exactamente 6 dígitos numéricos, false en caso contrario</returns>
        public static bool IsCode6DigitsValid(string? code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return false;

            return code.Length == 6 && code.All(char.IsDigit);
        }

        /// <summary>
        /// Obtiene el mensaje de error para código vacío
        /// </summary>
        /// <returns>Cadena con el mensaje de error para código vacío</returns>
        public static string GetCodeEmptyErrorMessage()
        {
            return "Por favor ingresa el código de verificación.";
        }

        /// <summary>
        /// Obtiene el mensaje de error para código inválido
        /// </summary>
        /// <returns>Cadena con el mensaje de error para código inválido</returns>
        public static string GetCodeInvalidErrorMessage()
        {
            return "El código debe tener 6 dígitos.";
        }
    }

    /// <summary>
    /// Validador para la página de recuperación de contraseña (PasswordRecoveryPage)
    /// </summary>
    public static class PasswordRecoveryValidator
    {
        /// <summary>
        /// Valida que el email no esté vacío
        /// </summary>
        /// <param name="email">El email a validar</param>
        /// <returns>true si el email no está vacío ni null, false en caso contrario</returns>
        public static bool IsEmailValid(string? email)
        {
            return !string.IsNullOrWhiteSpace(email);
        }

        /// <summary>
        /// Valida el formato básico de email
        /// </summary>
        /// <param name="email">El email a validar</param>
        /// <returns>true si el email tiene formato básico válido (@ y .), false en caso contrario</returns>
        public static bool IsEmailFormatValid(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return email.Contains("@") && email.Contains(".");
        }

        /// <summary>
        /// Valida que sea un email institucional del SENA
        /// </summary>
        /// <param name="email">El email a validar</param>
        /// <returns>true si el email termina en @soy.sena.edu.co, false en caso contrario</returns>
        public static bool IsInstitutionalEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return email.EndsWith("@soy.sena.edu.co", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Obtiene el mensaje de error para email vacío
        /// </summary>
        /// <returns>Cadena con el mensaje de error para email vacío</returns>
        public static string GetEmailEmptyErrorMessage()
        {
            return "Por favor ingresa tu correo electrónico.";
        }

        /// <summary>
        /// Obtiene el mensaje de error para email inválido
        /// </summary>
        /// <returns>Cadena con el mensaje de error para email inválido</returns>
        public static string GetEmailInvalidErrorMessage()
        {
            return "Por favor ingresa un correo electrónico válido.";
        }
    }

    /// <summary>
    /// Validador para la página de restablecimiento de contraseña (PasswordResetPage)
    /// </summary>
    public static class PasswordResetValidator
    {
        /// <summary>
        /// Longitud mínima de contraseña
        /// </summary>
        public const int MinPasswordLength = 8;

        /// <summary>
        /// Valida que la contraseña no esté vacía
        /// </summary>
        /// <param name="password">La contraseña a validar</param>
        /// <returns>true si la contraseña no está vacía ni null, false en caso contrario</returns>
        public static bool IsPasswordValid(string? password)
        {
            return !string.IsNullOrWhiteSpace(password);
        }

        /// <summary>
        /// Valida que la contraseña tenga la longitud mínima
        /// </summary>
        /// <param name="password">La contraseña a validar</param>
        /// <returns>true si la contraseña tiene al menos 8 caracteres, false en caso contrario</returns>
        public static bool IsPasswordLengthValid(string? password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            return password.Length >= MinPasswordLength;
        }

        /// <summary>
        /// Valida que ambas contraseñas coincidan
        /// </summary>
        /// <param name="password">La primera contraseña</param>
        /// <param name="confirmPassword">La contraseña de confirmación</param>
        /// <returns>true si ambas contraseñas coinciden y no están vacías, false en caso contrario</returns>
        public static bool DoPasswordsMatch(string? password, string? confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
                return false;

            return password == confirmPassword;
        }

        /// <summary>
        /// Valida la fortaleza básica de la contraseña (al menos una letra y un número)
        /// </summary>
        /// <param name="password">La contraseña a validar</param>
        /// <returns>true si la contraseña contiene al menos una letra y un dígito, false en caso contrario</returns>
        public static bool IsPasswordStrong(string? password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            bool hasLetter = password.Any(char.IsLetter);
            bool hasDigit = password.Any(char.IsDigit);

            return hasLetter && hasDigit;
        }

        /// <summary>
        /// Obtiene el mensaje de error para contraseña vacía
        /// </summary>
        /// <returns>Cadena con el mensaje de error para contraseña vacía</returns>
        public static string GetPasswordEmptyErrorMessage()
        {
            return "Por favor ingresa la nueva contraseña.";
        }

        /// <summary>
        /// Obtiene el mensaje de error para contraseña corta
        /// </summary>
        /// <returns>Cadena con el mensaje de error para contraseña demasiado corta</returns>
        public static string GetPasswordLengthErrorMessage()
        {
            return $"La contraseña debe tener al menos {MinPasswordLength} caracteres.";
        }

        /// <summary>
        /// Obtiene el mensaje de error cuando las contraseñas no coinciden
        /// </summary>
        /// <returns>Cadena con el mensaje de error para contraseñas que no coinciden</returns>
        public static string GetPasswordMismatchErrorMessage()
        {
            return "Las contraseñas no coinciden.";
        }
    }

    /// <summary>
    /// Validador para la página de registro (RegisterPage)
    /// </summary>
    public static class RegisterValidator
    {
        /// <summary>
        /// Valida que el email no esté vacío
        /// </summary>
        /// <param name="email">El email a validar</param>
        /// <returns>true si el email no está vacío ni null, false en caso contrario</returns>
        public static bool IsEmailValid(string? email)
        {
            return !string.IsNullOrWhiteSpace(email);
        }

        /// <summary>
        /// Valida que el nombre no esté vacío
        /// </summary>
        /// <param name="name">El nombre a validar</param>
        /// <returns>true si el nombre no está vacío ni null, false en caso contrario</returns>
        public static bool IsNameValid(string? name)
        {
            return !string.IsNullOrWhiteSpace(name);
        }

        /// <summary>
        /// Valida que el apellido no esté vacío
        /// </summary>
        /// <param name="lastName">El apellido a validar</param>
        /// <returns>true si el apellido no está vacío ni null, false en caso contrario</returns>
        public static bool IsLastNameValid(string? lastName)
        {
            return !string.IsNullOrWhiteSpace(lastName);
        }

        /// <summary>
        /// Valida que el número de documento no esté vacío
        /// </summary>
        /// <param name="documentNumber">El número de documento a validar</param>
        /// <returns>true si el número de documento no está vacío ni null, false en caso contrario</returns>
        public static bool IsDocumentNumberValid(string? documentNumber)
        {
            return !string.IsNullOrWhiteSpace(documentNumber);
        }

        /// <summary>
        /// Valida que el número de documento sea numérico
        /// </summary>
        /// <param name="documentNumber">El número de documento a validar</param>
        /// <returns>true si el número de documento contiene solo dígitos, false en caso contrario</returns>
        public static bool IsDocumentNumberNumeric(string? documentNumber)
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
                return false;

            return documentNumber.All(char.IsDigit);
        }

        /// <summary>
        /// Valida que el teléfono no esté vacío
        /// </summary>
        /// <param name="phone">El número de teléfono a validar</param>
        /// <returns>true si el teléfono no está vacío ni null, false en caso contrario</returns>
        public static bool IsPhoneValid(string? phone)
        {
            return !string.IsNullOrWhiteSpace(phone);
        }

        /// <summary>
        /// Valida que el teléfono sea numérico
        /// </summary>
        /// <param name="phone">El número de teléfono a validar</param>
        /// <returns>true si el teléfono contiene solo dígitos, false en caso contrario</returns>
        public static bool IsPhoneNumeric(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            return phone.All(char.IsDigit);
        }

        /// <summary>
        /// Valida que el teléfono tenga una longitud válida (7-10 dígitos)
        /// </summary>
        /// <param name="phone">El número de teléfono a validar</param>
        /// <returns>true si el teléfono tiene entre 7 y 10 dígitos, false en caso contrario</returns>
        public static bool IsPhoneLengthValid(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            return phone.Length >= 7 && phone.Length <= 10;
        }

        /// <summary>
        /// Valida que todos los campos requeridos estén completos
        /// </summary>
        /// <param name="email">El email del usuario</param>
        /// <param name="firstName">El nombre del usuario</param>
        /// <param name="lastName">El apellido del usuario</param>
        /// <param name="documentNumber">El número de documento del usuario</param>
        /// <param name="phone">El número de teléfono del usuario</param>
        /// <param name="hasDocumentType">Indica si se ha seleccionado un tipo de documento</param>
        /// <returns>true si todos los campos requeridos están completos y válidos, false en caso contrario</returns>
        public static bool AreAllFieldsValid(string? email, string? firstName, string? lastName, 
            string? documentNumber, string? phone, bool hasDocumentType)
        {
            return IsEmailValid(email) &&
                   IsNameValid(firstName) &&
                   IsLastNameValid(lastName) &&
                   IsDocumentNumberValid(documentNumber) &&
                   IsPhoneValid(phone) &&
                   hasDocumentType;
        }

        /// <summary>
        /// Obtiene el mensaje de error para email obligatorio
        /// </summary>
        /// <returns>Cadena con el mensaje de error para email obligatorio</returns>
        public static string GetEmailRequiredErrorMessage()
        {
            return "El correo institucional es obligatorio.";
        }

        /// <summary>
        /// Obtiene el mensaje de error para nombres obligatorios
        /// </summary>
        /// <returns>Cadena con el mensaje de error para nombres obligatorios</returns>
        public static string GetNamesRequiredErrorMessage()
        {
            return "Los nombres y apellidos son obligatorios.";
        }

        /// <summary>
        /// Obtiene el mensaje de error para tipo de documento obligatorio
        /// </summary>
        /// <returns>Cadena con el mensaje de error para tipo de documento obligatorio</returns>
        public static string GetDocumentTypeRequiredErrorMessage()
        {
            return "Debes seleccionar un tipo de documento.";
        }

        /// <summary>
        /// Obtiene el mensaje de error para número de documento obligatorio
        /// </summary>
        /// <returns>Cadena con el mensaje de error para número de documento obligatorio</returns>
        public static string GetDocumentNumberRequiredErrorMessage()
        {
            return "El número de documento es obligatorio.";
        }

        /// <summary>
        /// Obtiene el mensaje de error para teléfono obligatorio
        /// </summary>
        /// <returns>Cadena con el mensaje de error para teléfono obligatorio</returns>
        public static string GetPhoneRequiredErrorMessage()
        {
            return "El teléfono es obligatorio.";
        }

        /// <summary>
        /// Obtiene el mensaje de error para documento con formato inválido
        /// </summary>
        /// <returns>Cadena con el mensaje de error para documento con formato inválido</returns>
        public static string GetDocumentFormatErrorMessage()
        {
            return "El número de documento debe contener solo dígitos.";
        }

        /// <summary>
        /// Obtiene el mensaje de error para teléfono con formato inválido
        /// </summary>
        /// <returns>Cadena con el mensaje de error para teléfono con formato inválido</returns>
        public static string GetPhoneFormatErrorMessage()
        {
            return "El teléfono debe contener solo dígitos y tener entre 7 y 10 caracteres.";
        }
    }

    /// <summary>
    /// Validador para el modal de autenticación de dos factores (TwoFactorModal)
    /// </summary>
    public static class TwoFactorValidator
    {
        /// <summary>
        /// Valida que el código completo tenga exactamente 6 dígitos
        /// </summary>
        /// <param name="code1">Primer dígito del código</param>
        /// <param name="code2">Segundo dígito del código</param>
        /// <param name="code3">Tercer dígito del código</param>
        /// <param name="code4">Cuarto dígito del código</param>
        /// <param name="code5">Quinto dígito del código</param>
        /// <param name="code6">Sexto dígito del código</param>
        /// <returns>true si todos los dígitos están completos, false en caso contrario</returns>
        public static bool IsCodeComplete(string code1, string code2, string code3, 
            string code4, string code5, string code6)
        {
            return !string.IsNullOrWhiteSpace(code1) &&
                   !string.IsNullOrWhiteSpace(code2) &&
                   !string.IsNullOrWhiteSpace(code3) &&
                   !string.IsNullOrWhiteSpace(code4) &&
                   !string.IsNullOrWhiteSpace(code5) &&
                   !string.IsNullOrWhiteSpace(code6);
        }

        /// <summary>
        /// Combina los 6 dígitos en un código completo
        /// </summary>
        /// <param name="code1">Primer dígito del código</param>
        /// <param name="code2">Segundo dígito del código</param>
        /// <param name="code3">Tercer dígito del código</param>
        /// <param name="code4">Cuarto dígito del código</param>
        /// <param name="code5">Quinto dígito del código</param>
        /// <param name="code6">Sexto dígito del código</param>
        /// <returns>Cadena con los 6 dígitos combinados</returns>
        public static string CombineCode(string code1, string code2, string code3, 
            string code4, string code5, string code6)
        {
            return $"{code1}{code2}{code3}{code4}{code5}{code6}";
        }

        /// <summary>
        /// Valida que cada dígito sea numérico
        /// </summary>
        /// <param name="code1">Primer dígito del código</param>
        /// <param name="code2">Segundo dígito del código</param>
        /// <param name="code3">Tercer dígito del código</param>
        /// <param name="code4">Cuarto dígito del código</param>
        /// <param name="code5">Quinto dígito del código</param>
        /// <param name="code6">Sexto dígito del código</param>
        /// <returns>true si todos los dígitos son numéricos, false en caso contrario</returns>
        public static bool IsCodeNumeric(string code1, string code2, string code3, 
            string code4, string code5, string code6)
        {
            return IsDigit(code1) && IsDigit(code2) && IsDigit(code3) &&
                   IsDigit(code4) && IsDigit(code5) && IsDigit(code6);
        }

        /// <summary>
        /// Valida que un string sea un solo dígito
        /// </summary>
        /// <param name="digit">El string a validar como dígito</param>
        /// <returns>true si el string es un solo dígito numérico, false en caso contrario</returns>
        public static bool IsDigit(string? digit)
        {
            return !string.IsNullOrWhiteSpace(digit) && 
                   digit.Length == 1 && 
                   char.IsDigit(digit[0]);
        }

        /// <summary>
        /// Obtiene el mensaje de error para código incompleto
        /// </summary>
        /// <returns>Cadena con el mensaje de error para código incompleto</returns>
        public static string GetCodeIncompleteErrorMessage()
        {
            return "Por favor ingresa los 6 dígitos del código";
        }
    }
}
