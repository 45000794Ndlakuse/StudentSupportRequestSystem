using Microsoft.AspNetCore.Mvc;
using SupportRequestService.DTOs;
using SupportRequestService.Models;

namespace SupportRequestService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SupportRequestsController : ControllerBase
{
    private static readonly List<SupportRequest> Requests = new();

    // GET: api/supportrequests
    [HttpGet]
    public IActionResult GetAllRequests()
    {
        return Ok(Requests);
    }

    // GET: api/supportrequests/1
    [HttpGet("{id}")]
    public IActionResult GetRequestById(int id)
    {
        var request = Requests.FirstOrDefault(r => r.Id == id);

        if (request == null)
        {
            return NotFound("Support request not found.");
        }

        return Ok(request);
    }

    // GET: api/supportrequests/student/1
    [HttpGet("student/{studentId}")]
    public IActionResult GetRequestsByStudent(int studentId)
    {
        var studentRequests = Requests
            .Where(r => r.StudentId == studentId)
            .ToList();

        return Ok(studentRequests);
    }

    // POST: api/supportrequests
    [HttpPost]
    public IActionResult CreateRequest(CreateSupportRequestDto createDto)
    {
        var request = new SupportRequest
        {
            Id = Requests.Count + 1,
            StudentId = createDto.StudentId,
            Title = createDto.Title,
            Description = createDto.Description,
            Category = createDto.Category,
            Priority = createDto.Priority,
            Status = "Submitted"
        };

        Requests.Add(request);

        return CreatedAtAction(
            nameof(GetRequestById),
            new { id = request.Id },
            request
        );
    }

    // PUT: api/supportrequests/1
    [HttpPut("{id}")]
    public IActionResult UpdateRequest(
        int id,
        UpdateSupportRequestDto updateDto)
    {
        var request = Requests.FirstOrDefault(r => r.Id == id);

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

        return Ok(request);
    }

    // PUT: api/supportrequests/1/status
    [HttpPut("{id}/status")]
    public IActionResult UpdateRequestStatus(
        int id,
        UpdateRequestStatusDto statusDto)
    {
        var request = Requests.FirstOrDefault(r => r.Id == id);

        if (request == null)
        {
            return NotFound("Support request not found.");
        }

        request.Status = statusDto.Status;
        request.UpdatedAt = DateTime.UtcNow;

        return Ok(request);
    }

    // DELETE: api/supportrequests/1
    [HttpDelete("{id}")]
    public IActionResult DeleteRequest(int id)
    {
        var request = Requests.FirstOrDefault(r => r.Id == id);

        if (request == null)
        {
            return NotFound("Support request not found.");
        }

        Requests.Remove(request);

        return NoContent();
    }
}