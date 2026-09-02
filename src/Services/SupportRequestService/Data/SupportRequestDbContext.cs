using Microsoft.EntityFrameworkCore;
using SupportRequestService.Models;

namespace SupportRequestService.Data;

public class SupportRequestDbContext : DbContext
{
    public SupportRequestDbContext(DbContextOptions<SupportRequestDbContext> options)
        : base(options)
    {
    }

    public DbSet<SupportRequest> SupportRequests { get; set; }
}