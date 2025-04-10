using System.Linq;
using Webhooks.Api.Models;

namespace Webhooks.Api.Repositories
{
    public class InMemoryWebhookSubscriptionRepository
    {
        private List<WebhookSubscription> _subscriptions = [];
        public void Add(WebhookSubscription subscription) { _subscriptions.Add(subscription); }
        public void Remove(WebhookSubscription subscription) { _subscriptions.Remove(subscription); }
        public IReadOnlyList<WebhookSubscription> GetAll() { return _subscriptions; }
        public IReadOnlyList<WebhookSubscription> GetByEventType(string eventType) { return _subscriptions.Where(s=>s.EventType == eventType).ToList().AsReadOnly(); }
    }
}
