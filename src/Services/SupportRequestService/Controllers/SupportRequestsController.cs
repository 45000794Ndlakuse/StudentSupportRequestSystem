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

    public SupportRequestsController(
        SupportRequestDbContext context,
        IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
    }

    // GET: api/supportrequests
    [HttpGet]
    public async Task<IActionResult> GetAllRequests()
    {
        var requests = await _context.SupportRequests.ToListAsync();

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
            return NotFound("Support request not found.");
        }

        return Ok(request);
    }

    // GET: api/supportrequests/student/1
    [HttpGet("student/{studentId}")]
    public async Task<IActionResult> GetRequestsByStudent(int studentId)
    {
        var studentRequests = await _context.SupportRequests
            .Where(r => r.StudentId == studentId)
            .ToListAsync();

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
                Console.WriteLine(
                    $"Failed to create notification. Status: {response.StatusCode}"
                );
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Error communicating with NotificationService: {ex.Message}"
            );
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
            return NotFound("Support request not found.");
        }

        request.Status = statusDto.Status;
        request.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

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
            return NotFound("Support request not found.");
        }

        _context.SupportRequests.Remove(request);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}