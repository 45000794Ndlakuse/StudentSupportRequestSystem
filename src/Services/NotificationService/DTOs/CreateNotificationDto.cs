namespace NotificationService.DTOs;

public class CreateNotificationDto
{
    public int UserId { get; set; }

    public string Message { get; set; } = string.Empty;

    public string Type { get; set; } = "General";
}