namespace Webhooks.Api.DTO
{
    public class CreateWebhookRequest
    {
        public string EventType { get; set; }
        public string WebhookUrl { get; set; }
    }
}
