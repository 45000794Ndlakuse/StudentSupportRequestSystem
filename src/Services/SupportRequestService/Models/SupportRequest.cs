namespace SupportRequestService.Models;

public class SupportRequest
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Status { get; set; } = "Submitted";

    public string Priority { get; set; } = "Normal";

    public int? AssignedStaffId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}