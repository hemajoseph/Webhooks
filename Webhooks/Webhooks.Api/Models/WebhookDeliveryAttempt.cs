namespace Webhooks.Api.Models
{
    public class WebhookDeliveryAttempt
    {
        public Guid Id { get; set; }
        public Guid SubscriptionId { get; set; }
        public string Payload { get; set; }
        public int? ResponseStatusCode { get; set; }
        public bool Success { get; set; }
        public DateTime Timestamp { get; set; }   
    }
}
