using Webhooks.Api.Repositories;
using Webhooks.Api.Data;
using Webhooks.Api.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Webhooks.Api.Services
{
    public class WebhookDispatcher(IHttpClientFactory httpClientFactory, WebhookDbcontext dbContext)
    {
        //InMemoryWebhookSubscriptionRepository _subscriptionRepository;
        //HttpClient _httpClient;

        //WebhookDbcontext dbContext;
        //IHttpClientFactory httpClientFactory;    

        /*public WebhookDispatcher(IHttpClientFactory httpClientFactory, WebhookDbcontext dbcontext /)
        {
            //_subscriptionRepository = subscriptionRepository;
            //_httpClient= httpClient;
            dbContext = dbcontext;
            httpClientFactory = httpClientFactory;
        }*/

        public async Task DispatchAsync<T>(string eventType, T data) {
            //var subscriptions = _subscriptionRepository.GetByEventType(eventType);

            var subscriptions = await dbContext.subscriptions.AsNoTracking().Where(s => s.EventType == eventType).ToListAsync() ;
            
            foreach (WebhookSubscription subscription in subscriptions) {
                using var httpClient = httpClientFactory.CreateClient();  //only one instance will be creted as this is a factory now and the scope will be managed as the for loop
                var payload = new WebhookPayload<T> { 
                    Id = Guid.NewGuid(),
                    EventType = subscription.EventType,
                    TimeStamp = DateTime.UtcNow,
                    SubscriptionId= subscription.Id,
                    Data = data
                };

                var jsonPayload = JsonSerializer.Serialize(payload);
                try
                {
                    var response = await httpClient.PostAsJsonAsync(subscription.WebhookUrl, payload);
                    var attempt = new WebhookDeliveryAttempt
                    {
                        Id = Guid.NewGuid(),
                        SubscriptionId = subscription.Id,
                        Payload = jsonPayload,
                        ResponseStatusCode = (int)response.StatusCode,
                        Success = response.IsSuccessStatusCode,
                        Timestamp = DateTime.UtcNow
                    };
                    dbContext.deliveryAttempts.Add(attempt);
                    await dbContext.SaveChangesAsync();
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

                    dbContext.deliveryAttempts.Add(attempt);
                    await dbContext.SaveChangesAsync();
                }
                
            }
        }

        public void Dispatch<T>(string eventType, T data)
        {
            //var subscriptions = _subscriptionRepository.GetByEventType(eventType);

            var subscriptions = dbContext.subscriptions.Where(s => s.EventType == eventType).ToList();

            foreach (WebhookSubscription subscription in subscriptions)
            {
                using var httpClient = httpClientFactory.CreateClient();  //only one instance will be creted as this is a factory now and the scope will be managed as the for loop
                var payload = new WebhookPayload<T>
                {
                    Id = Guid.NewGuid(),
                    EventType = subscription.EventType,
                    TimeStamp = DateTime.UtcNow,
                    SubscriptionId = subscription.Id,
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
                    dbContext.deliveryAttempts.Add(attempt);
                    dbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    var attempt = new WebhookDeliveryAttempt
                    {
                        Id = Guid.NewGuid(),
                        SubscriptionId = subscription.Id,
                        Payload = jsonPayload,
                        ResponseStatusCode = null,
                        Success = false,
                        Timestamp = DateTime.UtcNow
                    };

                    dbContext.deliveryAttempts.Add(attempt);
                    dbContext.SaveChanges();
                }

            }
        }
    }


}
