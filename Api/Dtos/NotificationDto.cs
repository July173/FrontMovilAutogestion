using System;

namespace AutogestionSena.MAUI.Api.Dtos
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedAt { get; set; }
        public int IdUser { get; set; }
    }
}
