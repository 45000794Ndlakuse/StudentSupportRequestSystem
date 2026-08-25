namespace SupportRequestService.DTOs;

public class UpdateSupportRequestDto
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Priority { get; set; } = "Normal";

    public string Status { get; set; } = string.Empty;

    public int? AssignedStaffId { get; set; }
}