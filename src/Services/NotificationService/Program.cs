using Consul;
using NotificationService.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();

// OpenAPI
builder.Services.AddOpenApi();

// Consul configuration
var consulConfig = new ConsulConfig();

builder.Services.AddSingleton(consulConfig);

var app = builder.Build();

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
    ID = "notification-service-1",
    Name = consulConfig.ServiceName,
    Address = consulConfig.ServiceHost,
    Port = consulConfig.ServicePort,

    Tags = new[]
    {
        "notification",
        "api"
    },

    Check = new AgentServiceCheck
    {
        HTTP = $"http://{consulConfig.ServiceHost}:{consulConfig.ServicePort}/health",
        Interval = TimeSpan.FromSeconds(10),
        Timeout = TimeSpan.FromSeconds(5)
    }
};

// Register service with Consul
await consulClient.Agent.ServiceRegister(registration);

Console.WriteLine(
    $"NotificationService registered with Consul at {consulConfig.ServiceHost}:{consulConfig.ServicePort}"
);

// Deregister service when application stops
app.Lifetime.ApplicationStopping.Register(() =>
{
    consulClient.Agent.ServiceDeregister(registration.ID).Wait();
});

app.Run();