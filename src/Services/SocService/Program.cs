using Serilog;
using Consul;
using SocService.Configuration;
using SocService.Repositories;
using Microsoft.EntityFrameworkCore;
using SocService.Data;


var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        "logs/soc-service-.log",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add controllers
builder.Services.AddControllers();


// SQL Server / Entity Framework
builder.Services.AddDbContext<SocDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SocDb")
    ));

builder.Services.AddScoped<ISocRepository, SocRepository>();

// OpenAPI
builder.Services.AddOpenApi();

//builder.Services.AddSwaggerGen();

// Consul configuration
var consulConfig = new ConsulConfig();

builder.Services.AddSingleton(consulConfig);

var app = builder.Build();
// app.UseSwagger();
// app.UseSwaggerUI();

// OpenAPI
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// ==========================================
// Consul Service Registration
// ==========================================

var consulClient = new ConsulClient(config =>
{
    config.Address = new Uri(consulConfig.Address);
});

var registration = new AgentServiceRegistration
{
    ID = "soc-service-1",
    Name = consulConfig.ServiceName,
    Address = consulConfig.ServiceHost,
    Port = consulConfig.ServicePort,

    Tags = new[]
    {
        "soc",
        "api"
    },

    Check = new AgentServiceCheck
{
    HTTP = $"http://{consulConfig.HealthCheckHost}:{consulConfig.ServicePort}/health",
    Interval = TimeSpan.FromSeconds(10),
    Timeout = TimeSpan.FromSeconds(5)
}
};

// Register service with Consul
await consulClient.Agent.ServiceRegister(registration);

Log.Information(
    "SocService registered with Consul at {Host}:{Port}",
    consulConfig.ServiceHost,
    consulConfig.ServicePort);

// Deregister service when application stops
app.Lifetime.ApplicationStopping.Register(() =>
{
    consulClient.Agent.ServiceDeregister(registration.ID).Wait();
});

app.Run();