using Microsoft.EntityFrameworkCore;
using SocService.Data;
using SocService.Models;

namespace SocService.Repositories;

public class SocRepository : ISocRepository
{
    private readonly SocDbContext _context;

    public SocRepository(SocDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SecurityEvent>> GetAllAsync()
    {
        return await _context.SecurityEvents
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<SecurityEvent?> GetByIdAsync(int id)
    {
        return await _context.SecurityEvents
            .AsNoTracking()
            .FirstOrDefaultAsync(se => se.EventId == id);
    }

    public async Task<IEnumerable<SecurityEvent>> GetByServiceNameAsync(string serviceName)
    {
        return await _context.SecurityEvents
            .AsNoTracking()
            .Where(se => se.ServiceName == serviceName)
            .ToListAsync();
    }

    public async Task<SecurityEvent> CreateAsync(SecurityEvent securityEvent)
    {
        _context.SecurityEvents.Add(securityEvent);
        await _context.SaveChangesAsync();

        return securityEvent;
    }

    public async Task<bool> UpdateAsync(int id, SecurityEvent securityEvent)
    {
        var existingSecurityEvent = await _context.SecurityEvents
            .FirstOrDefaultAsync(se => se.EventId == id);

        if (existingSecurityEvent == null)
            return false;

        existingSecurityEvent.ServiceName = securityEvent.ServiceName;
        existingSecurityEvent.Message = securityEvent.Message;
        existingSecurityEvent.Timestamp = securityEvent.Timestamp;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var securityEvent = await _context.SecurityEvents
            .FirstOrDefaultAsync(se => se.EventId == id);

        if (securityEvent == null)
            return false;

        _context.SecurityEvents.Remove(securityEvent);
        await _context.SaveChangesAsync();

        return true;
    }

}