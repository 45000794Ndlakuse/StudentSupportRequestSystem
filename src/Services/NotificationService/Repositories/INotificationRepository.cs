using NotificationService.Models;

namespace NotificationService.Repositories;

public interface INotificationRepository
{
    Task<IEnumerable<Notification>> GetAllAsync();

    Task<Notification?> GetByIdAsync(int id);

    Task<IEnumerable<Notification>> GetByUserIdAsync(int userId);

    Task<Notification> CreateAsync(Notification notification);

    Task<bool> UpdateAsync(int id, Notification notification);

    Task<bool> DeleteAsync(int id);

    Task<bool> MarkAsReadAsync(int id);
}