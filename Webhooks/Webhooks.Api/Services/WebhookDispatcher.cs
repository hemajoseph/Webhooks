using Webhooks.Api.Repositories;

namespace Webhooks.Api.Services
{
    public class WebhookDispatcher
    {
        InMemoryWebhookSubscriptionRepository _subscriptionRepository;
        HttpClient _httpClient;

        public WebhookDispatcher(HttpClient httpClient, InMemoryWebhookSubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
            _httpClient= httpClient;
        }
        public void Dispatch(string eventType, object payload) {
           var subscriptions = _subscriptionRepository.GetByEventType(eventType);
            foreach (var subscription in subscriptions) {
                var request = new { Id = Guid.NewGuid(), EventType = subscription.EventType, TimeStamp = DateTime.Now, SubscriptionId = subscription.Id, Data = payload };
                _httpClient.PostAsJsonAsync(subscription.WebhookUrl, payload);
            }
        }
    }
}
