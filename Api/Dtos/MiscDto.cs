using System;
using AutogestionSena.MAUI.Api.Dtos;

namespace AutogestionSena.MAUI.Api.Dtos
{
    public class InfoCardPropsDto
    {
        public string? Title { get; set; }
        public string? StatusLabel { get; set; }
        public string? StatusColor { get; set; } // "green" o "red"
        public string? Description { get; set; }
        public int Count { get; set; }
        public string? ButtonText { get; set; }
        // Métodos de acción pueden ser delegados o eventos en C#
        public Action? OnButtonClick { get; set; }
        public string? ActionLabel { get; set; }
        public string? ActionType { get; set; } // "enable" o "disable"
        public Action? OnActionClick { get; set; }
    }

    public class UsuarioRegistradoDto
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? Estado { get; set; }
        public int Role { get; set; }
        public bool? Registered { get; set; }
        public Person? Person { get; set; }
    }

    public class ConfirmModalPropsDto
    {
        public bool IsOpen { get; set; }
        public string? Title { get; set; }
        public string? Message { get; set; }
        public string? ConfirmText { get; set; }
        public string? CancelText { get; set; }
        public Action? OnConfirm { get; set; }
        public Action? OnCancel { get; set; }
    }
}