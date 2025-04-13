using Webhooks.Api.Repositories;
using Webhooks.Api.Data;
using Webhooks.Api.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Webhooks.Api.Services
{
    public class WebhookDispatcher
    {
        //InMemoryWebhookSubscriptionRepository _subscriptionRepository;
        //HttpClient _httpClient;

        WebhookDbcontext _dbContext;
        IHttpClientFactory _httpClientFactory;    

        public WebhookDispatcher(IHttpClientFactory httpClientFactory, WebhookDbcontext dbcontext /*, InMemoryWebhookSubscriptionRepository subscriptionRepository*/)
        {
            //_subscriptionRepository = subscriptionRepository;
            //_httpClient= httpClient;
            _dbContext = dbcontext;
            _httpClientFactory = httpClientFactory;
        }
        public async void Dispatch<T>(string eventType, T data) {
            //var subscriptions = _subscriptionRepository.GetByEventType(eventType);

            var subscriptions = _dbContext.subscriptions.Where(s => s.EventType == eventType).ToList() ;
            
            foreach (var subscription in subscriptions) {
                using var httpClient = _httpClientFactory.CreateClient();  //only one instance will be creted as this is a factory now and the scope will be managed as the for loop
                var payload = new WebhookPayload<T> { 
                    Id = Guid.NewGuid(),
                    EventType = subscription.EventType,
                    TimeStamp = DateTime.Now,
                    SubscriptionId= subscription.Id,
                    Data = data
                };

                var jsonPayload = JsonSerializer.Serialize(payload);
                try
                {
                    var response = httpClient.PostAsJsonAsync(subscription.WebhookUrl, payload);
                    var attempt = new WebhookDeliveryAttempt
                    {
                        Id = Guid.NewGuid(),
                        SubscriptionId = subscription.Id,
                        Payload = jsonPayload,
                        ResponseStatusCode = (int)response.Status,
                        Success = response.IsCompletedSuccessfully,
                        Timestamp = DateTime.UtcNow
                    };
                    _dbContext.deliveryAttempts.Add(attempt);
                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception ex) {
                    var attempt = new WebhookDeliveryAttempt
                    {
                        Id = Guid.NewGuid(),
                        SubscriptionId = subscription.Id,
                        Payload = jsonPayload,
                        ResponseStatusCode = null,
                        Success = false,
                        Timestamp = DateTime.UtcNow
                    };

                    _dbContext.deliveryAttempts.Add(attempt);
                    await _dbContext.SaveChangesAsync();
                }
                
            }
        }
    }


}
