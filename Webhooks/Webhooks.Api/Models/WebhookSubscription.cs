namespace Webhooks.Api.Models
{
    public sealed record WebhookSubscription
    {
        public Guid Id { get; set; }
        public string EventType  { get; set; }
        public string WebhookUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
