namespace Webhooks.Api.Services
{
    public class WebhookPayload<T>
    {
        public Guid Id { get; set; }
        public string EventType { get; set; }
        public DateTime TimeStamp { get; set; }
        public Guid SubscriptionId { get; set; }
        public object Data { get; set; }
    }
}
