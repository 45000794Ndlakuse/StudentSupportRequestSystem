namespace SupportRequestService.Configuration
{
    public class ConsulConfig
    {
        public string Address { get; set; } = "http://localhost:8500";
        public string ServiceName { get; set; } = "SupportRequestService";
        public string ServiceHost { get; set; } = "host.docker.internal";
        public int ServicePort { get; set; } = 5092;
    }
}