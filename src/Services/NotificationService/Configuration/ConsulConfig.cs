namespace NotificationService.Configuration
{
    public class ConsulConfig
    {
        public string Address { get; set; } = "http://localhost:8500";

        public string ServiceName { get; set; } = "NotificationService";

        // Address returned by Consul to other Windows services
        public string ServiceHost { get; set; } = "localhost";

        // Address Consul uses for health checks from inside Docker
        public string HealthCheckHost { get; set; } = "host.docker.internal";

        public int ServicePort { get; set; } = 5195;
    }
}