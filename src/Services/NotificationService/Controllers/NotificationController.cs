using Microsoft.AspNetCore.Mvc;
using NotificationService.DTOs;
using NotificationService.Models;

namespace NotificationService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private static readonly List<Notification> Notifications = new();

    // GET: api/notifications
    [HttpGet]
    public IActionResult GetAllNotifications()
    {
        return Ok(Notifications);
    }

    // GET: api/notifications/1
    [HttpGet("{id}")]
    public IActionResult GetNotificationById(int id)
    {
        var notification = Notifications
            .FirstOrDefault(n => n.Id == id);

        if (notification == null)
        {
            return NotFound("Notification not found.");
        }

        return Ok(notification);
    }

    // GET: api/notifications/user/1
    [HttpGet("user/{userId}")]
    public IActionResult GetNotificationsByUser(int userId)
    {
        var userNotifications = Notifications
            .Where(n => n.UserId == userId)
            .ToList();

        return Ok(userNotifications);
    }

    // POST: api/notifications
    [HttpPost]
    public IActionResult CreateNotification(
        CreateNotificationDto createDto)
    {
        var notification = new Notification
        {
            Id = Notifications.Count + 1,
            UserId = createDto.UserId,
            Message = createDto.Message,
            Type = createDto.Type,
            IsRead = false
        };

        Notifications.Add(notification);

        return CreatedAtAction(
            nameof(GetNotificationById),
            new { id = notification.Id },
            notification
        );
    }

    // PUT: api/notifications/1/read
    [HttpPut("{id}/read")]
    public IActionResult MarkNotificationAsRead(
        int id,
        MarkNotificationReadDto readDto)
    {
        var notification = Notifications
            .FirstOrDefault(n => n.Id == id);

        if (notification == null)
        {
            return NotFound("Notification not found.");
        }

        notification.IsRead = readDto.IsRead;

        return Ok(notification);
    }

    // DELETE: api/notifications/1
    [HttpDelete("{id}")]
    public IActionResult DeleteNotification(int id)
    {
        var notification = Notifications
            .FirstOrDefault(n => n.Id == id);

        if (notification == null)
        {
            return NotFound("Notification not found.");
        }

        Notifications.Remove(notification);

        return NoContent();
    }
}