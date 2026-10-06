using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SocService.Data;

public class SocDbContextFactory : IDesignTimeDbContextFactory<SocDbContext>
{
    public SocDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("NotificationDb");

        var optionsBuilder = new DbContextOptionsBuilder<SocDbContext>();

        optionsBuilder.UseSqlServer(connectionString);

        return new SocDbContext(optionsBuilder.Options);
    }
}