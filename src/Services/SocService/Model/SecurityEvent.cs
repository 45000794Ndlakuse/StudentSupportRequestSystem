
namespace SocService.Model;

public class SecurityEvent
{
public int EventId { get; set; }
public DateTime Timestamp { get; set; } = DateTime.UtcNow;
public string ServiceName { get; set; } = string.Empty;
public string EventType { get; set; } = string.Empty; // e.g. FAILED_LOGIN, UNAUTHORISED_ACCESS
public string Severity { get; set; } = "LOW"; // LOW | MEDIUM | HIGH
public string? UserId { get; set; }
public string? SourceIp { get; set; }
public string? Endpoint { get; set; }
public string? HttpMethod { get; set; }
public int? StatusCode { get; set; }
public string Message { get; set; } = string.Empty;
public string CorrelationId { get; set; } = Guid.NewGuid().ToString();
public string? AffectedEntity { get; set; }
}