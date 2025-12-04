using System;

namespace AutogestionSena.MAUI.Api.Dtos
{
    public class ApiResponseParsedDto
    {
        public bool Success { get; set; }
        public string? Detail { get; set; }
        public int StatusCode { get; set; }
        public bool IsSuccessStatusCode { get; set; }
    }
}
