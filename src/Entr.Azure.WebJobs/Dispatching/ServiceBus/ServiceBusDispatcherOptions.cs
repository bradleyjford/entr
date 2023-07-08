namespace Entr.Azure.WebJobs.Dispatching.ServiceBus
{
    public class ServiceBusDispatcherOptions
    {
        public string ServiceBusConnectionString { get; set; } = "ServiceBus";
        public string Topic { get; set; }
        public string Subscription { get; set; }
        public IContentTypeConverter ContentTypeConverter { get; set; }
        public int MaxConcurrentCalls { get; set; } = 1;
        public int MaxDeliveryCount { get; set; } = 10;
    }
}
