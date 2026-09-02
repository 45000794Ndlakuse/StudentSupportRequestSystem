using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace NotificationService.Data;

public class NotificationDbContextFactory : IDesignTimeDbContextFactory<NotificationDbContext>
{
    public NotificationDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("NotificationDb");

        var optionsBuilder = new DbContextOptionsBuilder<NotificationDbContext>();

        optionsBuilder.UseSqlServer(connectionString);

        return new NotificationDbContext(optionsBuilder.Options);
    }
}