using Consul;
using SupportRequestService.Configuration;

namespace SupportRequestService.Services;

public class ConsulServiceDiscovery
{
    private readonly ConsulClient _consulClient;

    public ConsulServiceDiscovery(ConsulConfig config)
    {
        _consulClient = new ConsulClient(options =>
        {
            options.Address = new Uri(config.Address);
        });
    }

    public async Task<string?> GetServiceUrlAsync(string serviceName)
    {
        var services = await _consulClient.Health.Service(
            serviceName,
            tag: null,
            passingOnly: true);

        var service = services.Response.FirstOrDefault();

        if (service == null)
        {
            return null;
        }

        return $"http://{service.Service.Address}:{service.Service.Port}";
    }
}