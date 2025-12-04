using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AutogestionSena.MAUI.Api.Dtos
{
    /// <summary>
    /// DTO para los datos básicos del usuario en respuestas de autenticación
    /// </summary>
    public class UserDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("role")]
        public int Role { get; set; }
  
        [JsonPropertyName("person")]
        public int Person { get; set; }
        
        [JsonPropertyName("registered")]
        public bool Registered { get; set; }
    }

    public class Person
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? SecondName { get; set; }
        public string? FirstLastName { get; set; }
        public string? SecondLastName { get; set; }
        public int PhoneNumber { get; set; }
        public int TypeIdentification { get; set; }
        public int NumberIdentification { get; set; }
        public bool Active { get; set; }
        public string? Image { get; set; }
    }

    public class Role
    {
        public string? Id { get; set; }
        public string? TypeRole { get; set; }
        public string? Description { get; set; }
        public bool Active { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public Person? Person { get; set; }
        public Role? Role { get; set; }
        public bool IsActive { get; set; }
        public bool Registered { get; set; }
    }

    /// <summary>
    /// DTO para la respuesta completa del usuario incluyendo datos del aprendiz/instructor
    /// Usado para GET security/users/{id}/
    /// </summary>
    public class UserDetailDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("person")]
        public UserPersonDto? Person { get; set; }

        [JsonPropertyName("role")]
        public UserRoleDto? Role { get; set; }

        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; }

        [JsonPropertyName("registered")]
        public bool Registered { get; set; }

        [JsonPropertyName("apprentice")]
        public UserApprenticeDto? Apprentice { get; set; }

        [JsonPropertyName("instructor")]
        public UserInstructorDto? Instructor { get; set; }
    }

    public class UserPersonDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }

        [JsonPropertyName("second_name")]
        public string? SecondName { get; set; }

        [JsonPropertyName("first_last_name")]
        public string? FirstLastName { get; set; }

        [JsonPropertyName("second_last_name")]
        public string? SecondLastName { get; set; }

        [JsonPropertyName("phone_number")]
        public long? PhoneNumber { get; set; }

        [JsonPropertyName("type_identification")]
        public int TypeIdentification { get; set; }

        [JsonPropertyName("number_identification")]
        public long NumberIdentification { get; set; }

        [JsonPropertyName("active")]
        public bool Active { get; set; }

        [JsonPropertyName("image")]
        public string? Image { get; set; }
    }

    public class UserRoleDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("type_role")]
        public string? TypeRole { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("active")]
        public bool Active { get; set; }
    }

    public class UserApprenticeDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("person")]
        public int Person { get; set; }

        [JsonPropertyName("ficha")]
        public int Ficha { get; set; }

        [JsonPropertyName("active")]
        public bool Active { get; set; }

        [JsonPropertyName("programa")]
        public UserProgramDto? Programa { get; set; }
    }

    public class UserInstructorDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        // Agregar más propiedades según sea necesario
    }

    public class UserProgramDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public class ValidateLoginResponse
    {
        [JsonPropertyName("success")]
    public string? Success { get; set; }
        
        // 🔧 AGREGADO: Propiedades para el login 2FA
        [JsonPropertyName("access")]
        public string? Access { get; set; }

        [JsonPropertyName("refresh")]
        public string? Refresh { get; set; }

        [JsonPropertyName("user")]
        public UserDto? User { get; set; }
        
        [JsonPropertyName("role")]
        public int? Role { get; set; }
    }

    public class ValidateSecondFactorResponse
    {
      [JsonPropertyName("access")]
        public string? Access { get; set; }

      [JsonPropertyName("refresh")]
        public string? Refresh { get; set; }

        [JsonPropertyName("user")]
  public UserDto? User { get; set; }

     [JsonPropertyName("success")]
        public string? Success { get; set; }
    }

    public class SecondFactorRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;
    }

    public class RegisterResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

   [JsonPropertyName("detail")]
        public string? Detail { get; set; }
    }

    public class PasswordResetRequestResponse
 {
      [JsonPropertyName("success")]
     public string? success { get; set; }

      [JsonPropertyName("message")]
        public string? Message { get; set; }
   
        [JsonPropertyName("code")]
    public string? code { get; set; }

        [JsonPropertyName("fecha_expiracion")]
   public string? fecha_expiracion { get; set; }
    }

   public class PasswordResetResponse
    {
  [JsonPropertyName("success")]
 public string? success { get; set; }

     [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}