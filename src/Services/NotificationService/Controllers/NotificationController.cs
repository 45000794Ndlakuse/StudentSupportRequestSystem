using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using NotificationService.DTOs;
using NotificationService.Models;
using NotificationService.Repositories;

namespace NotificationService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationRepository _repository;

    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(
    INotificationRepository repository,
    ILogger<NotificationsController> logger)
{
    _repository = repository;
    _logger = logger;
}
    // GET: api/notifications
    [HttpGet]
    public async Task<IActionResult> GetAllNotifications()
    {
        var notifications = await _repository.GetAllAsync();

        return Ok(notifications);
    }

    // GET: api/notifications/1
    [HttpGet("{id}")]
    public async Task<IActionResult> GetNotificationById(int id)
    {
        var notification = await _repository.GetByIdAsync(id);

        if (notification == null)
        {
            return NotFound("Notification not found.");
        }

        return Ok(notification);
    }

    // GET: api/notifications/user/1
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetNotificationsByUser(int userId)
    {
        var notifications = await _repository.GetByUserIdAsync(userId);

        return Ok(notifications);
    }

    // POST: api/notifications
    [HttpPost]
    public async Task<IActionResult> CreateNotification(
        CreateNotificationDto createDto)
    {
        _logger.LogInformation(
        "Creating notification for UserId {UserId} of type {Type}",
        createDto.UserId,
        createDto.Type);


        var notification = new Notification
        {
            UserId = createDto.UserId,
            Message = createDto.Message,
            Type = createDto.Type,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        var createdNotification =
            await _repository.CreateAsync(notification);

        return CreatedAtAction(
            nameof(GetNotificationById),
            new { id = createdNotification.Id },
            createdNotification
        );
    }

    // PUT: api/notifications/1
[HttpPut("{id}")]
public async Task<IActionResult> UpdateNotification(
    int id,
    Notification notification)
{
    var updated = await _repository.UpdateAsync(id, notification);

    if (!updated)
    {
        return NotFound("Notification not found.");
    }

    _logger.LogInformation(
        "Notification {NotificationId} updated",
        id);

    var updatedNotification = await _repository.GetByIdAsync(id);

    return Ok(updatedNotification);
}

    // PUT: api/notifications/1/read
    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkNotificationAsRead(
        int id,
        MarkNotificationReadDto readDto)
    {
        var notification = await _repository.GetByIdAsync(id);

        if (notification == null)
        {
            return NotFound("Notification not found.");
        }

        var updated = await _repository.MarkAsReadAsync(id);

        if (!updated)
        {
            return NotFound("Notification not found.");
        }

        var updatedNotification = await _repository.GetByIdAsync(id);

        return Ok(updatedNotification);
    }

    // DELETE: api/notifications/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNotification(int id)
    {
        var deleted = await _repository.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound("Notification not found.");
        }

        return NoContent();
    }
}
