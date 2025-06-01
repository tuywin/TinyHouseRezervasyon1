namespace TinyHouseRezervasyon.Models;

public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public object Data { get; set; } = new();
} 