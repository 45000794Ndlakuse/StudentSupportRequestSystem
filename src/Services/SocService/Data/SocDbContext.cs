using Microsoft.EntityFrameworkCore;
using SocService.Model;


namespace SocService.Data;

public class SocDbContext : DbContext
{
    public SocDbContext(DbContextOptions<SocDbContext> options)
        : base(options)
    {
    }

    public DbSet<SecurityEvent> SecurityEvents { get; set; }

    public DbSet<SecurityAlert> SecurityAlerts { get; set; }
}