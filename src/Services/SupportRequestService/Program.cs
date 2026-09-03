using Consul;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SupportRequestService.Configuration;
using SupportRequestService.Data;
using SupportRequestService.Services;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// Serilog Configuration
// ==========================================

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        "logs/support-request-service-.log",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();

builder.Services.AddDbContext<SupportRequestDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();

builder.Services.AddSingleton<IConsulClient>(
    new ConsulClient(config =>
    {
        config.Address = new Uri("http://localhost:8500");
    })
);

var consulConfig = new ConsulConfig();

builder.Services.AddSingleton(consulConfig);

builder.Services.AddSingleton<ConsulServiceDiscovery>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();

app.MapControllers();

// ==========================================
// Consul Service Registration
// ==========================================

var consulClient = new ConsulClient(config =>
{
    config.Address = new Uri(consulConfig.Address);
});

var registration = new AgentServiceRegistration()
{
    ID = "support-request-service-1",
    Name = consulConfig.ServiceName,
    Address = consulConfig.ServiceHost,
    Port = consulConfig.ServicePort,
    Tags = new[] { "support-request", "api" },
    Check = new AgentServiceCheck()
    {
        HTTP = $"http://{consulConfig.HealthCheckHost}:{consulConfig.ServicePort}/health",
        Interval = TimeSpan.FromSeconds(10),
        Timeout = TimeSpan.FromSeconds(5)
    }
};

await consulClient.Agent.ServiceRegister(registration);

Console.WriteLine(
    $"SupportRequestService registered with Consul at {consulConfig.ServiceHost}:{consulConfig.ServicePort}"
);

app.Lifetime.ApplicationStopping.Register(() =>
{
    consulClient.Agent.ServiceDeregister(registration.ID).Wait();
});

app.Run();