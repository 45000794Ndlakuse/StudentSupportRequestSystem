using SocService.Model;

namespace SocService.Repositories;

public interface ISocRepository
{
    Task<IEnumerable<SecurityEvent>> GetAllAsync();

    Task<SecurityEvent?> GetByIdAsync(int EventId);

    Task<IEnumerable<SecurityEvent>> GetByServiceNameAsync(string serviceName);

    Task<SecurityEvent> CreateAsync(SecurityEvent securityEvent);

    Task<bool> UpdateAsync(int id, SecurityEvent securityEvent);

    Task<bool> DeleteAsync(int id);


}