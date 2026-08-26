namespace NotificationService.Configuration
{
    public class ConsulConfig
    {
        public string Address { get; set; } = "http://localhost:8500";
        public string ServiceName { get; set; } = "NotificationService";
        public string ServiceHost { get; set; } = "localhost";
        public int ServicePort { get; set; } = 5195;
    }
}