using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupportRequestService.Data;
using SupportRequestService.DTOs;
using SupportRequestService.Models;

namespace SupportRequestService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SupportRequestsController : ControllerBase
{
    private readonly SupportRequestDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SupportRequestsController> _logger;

    public SupportRequestsController(
        SupportRequestDbContext context,
        IHttpClientFactory httpClientFactory,
        ILogger<SupportRequestsController> logger)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    // GET: api/supportrequests
    [HttpGet]
    public async Task<IActionResult> GetAllRequests()
    {
        var requests = await _context.SupportRequests.ToListAsync();

        _logger.LogInformation(
            "Retrieved {RequestCount} support requests",
            requests.Count);

        return Ok(requests);
    }

    // GET: api/supportrequests/1
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRequestById(int id)
    {
        var request = await _context.SupportRequests
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request == null)
        {
            _logger.LogWarning(
                "Support request lookup failed. RequestId: {RequestId}",
                id);

            return NotFound("Support request not found.");
        }

        _logger.LogInformation(
            "Support request retrieved. RequestId: {RequestId}",
            id);

        return Ok(request);
    }

    // GET: api/supportrequests/student/1
    [HttpGet("student/{studentId}")]
    public async Task<IActionResult> GetRequestsByStudent(int studentId)
    {
        var studentRequests = await _context.SupportRequests
            .Where(r => r.StudentId == studentId)
            .ToListAsync();

        _logger.LogInformation(
            "Retrieved {RequestCount} support requests for StudentId: {StudentId}",
            studentRequests.Count,
            studentId);

        return Ok(studentRequests);
    }

    // POST: api/supportrequests
    [HttpPost]
    public async Task<IActionResult> CreateRequest(
        CreateSupportRequestDto createDto)
    {
        var request = new SupportRequest
        {
            StudentId = createDto.StudentId,
            Title = createDto.Title,
            Description = createDto.Description,
            Category = createDto.Category,
            Priority = createDto.Priority,
            Status = "Submitted",
            CreatedAt = DateTime.UtcNow
        };

        _context.SupportRequests.Add(request);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Support request created. RequestId: {RequestId}, StudentId: {StudentId}, Category: {Category}, Priority: {Priority}",
            request.Id,
            request.StudentId,
            request.Category,
            request.Priority);

        // Send notification
        try
        {
            var client = _httpClientFactory.CreateClient();

            var notification = new
            {
                UserId = request.StudentId,
                Message = $"Your support request '{request.Title}' has been successfully submitted.",
                Type = "SupportRequest"
            };

            var response = await client.PostAsJsonAsync(
                "http://localhost:5195/api/notifications",
                notification
            );

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Notification request failed. SupportRequestId: {RequestId}, StatusCode: {StatusCode}",
                    request.Id,
                    response.StatusCode);
            }
            else
            {
                _logger.LogInformation(
                    "Notification sent successfully. SupportRequestId: {RequestId}",
                    request.Id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error communicating with NotificationService. SupportRequestId: {RequestId}",
                request.Id);
        }

        return CreatedAtAction(
            nameof(GetRequestById),
            new { id = request.Id },
            request
        );
    }

    // PUT: api/supportrequests/1
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRequest(
        int id,
        UpdateSupportRequestDto updateDto)
    {
        var request = await _context.SupportRequests
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request == null)
        {
            _logger.LogWarning(
                "Support request update failed. RequestId: {RequestId}",
                id);

            return NotFound("Support request not found.");
        }

        request.Title = updateDto.Title;
        request.Description = updateDto.Description;
        request.Category = updateDto.Category;
        request.Priority = updateDto.Priority;
        request.Status = updateDto.Status;
        request.AssignedStaffId = updateDto.AssignedStaffId;
        request.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Support request updated. RequestId: {RequestId}, Status: {Status}, Priority: {Priority}, AssignedStaffId: {AssignedStaffId}",
            request.Id,
            request.Status,
            request.Priority,
            request.AssignedStaffId);

        return Ok(request);
    }

    // PUT: api/supportrequests/1/status
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateRequestStatus(
        int id,
        UpdateRequestStatusDto statusDto)
    {
        var request = await _context.SupportRequests
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request == null)
        {
            _logger.LogWarning(
                "Support request status update failed. RequestId: {RequestId}",
                id);

            return NotFound("Support request not found.");
        }

        request.Status = statusDto.Status;
        request.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Support request status changed. RequestId: {RequestId}, NewStatus: {Status}",
            request.Id,
            request.Status);

        return Ok(request);
    }

    // DELETE: api/supportrequests/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRequest(int id)
    {
        var request = await _context.SupportRequests
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request == null)
        {
            _logger.LogWarning(
                "Support request deletion failed. RequestId: {RequestId}",
                id);

            return NotFound("Support request not found.");
        }

        _context.SupportRequests.Remove(request);

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Support request deleted. RequestId: {RequestId}, StudentId: {StudentId}",
            request.Id,
            request.StudentId);

        return NoContent();
    }
}