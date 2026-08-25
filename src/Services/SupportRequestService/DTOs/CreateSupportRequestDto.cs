namespace SupportRequestService.DTOs;

public class CreateSupportRequestDto
{
    public int StudentId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Priority { get; set; } = "Normal";
}