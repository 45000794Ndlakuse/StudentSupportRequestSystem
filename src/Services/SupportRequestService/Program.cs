using Consul;
using SupportRequestService.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

// Allows this service to make HTTP calls to other microservices
builder.Services.AddHttpClient();

// ==========================================
// Consul Configuration
// ==========================================

var consulConfig = new ConsulConfig();

builder.Services.AddSingleton(consulConfig);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Temporarily disabled while we are only using HTTP locally
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
        HTTP = $"http://{consulConfig.ServiceHost}:{consulConfig.ServicePort}/health",
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