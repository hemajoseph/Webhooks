using Microsoft.EntityFrameworkCore;
using Webhooks.Api.Models;

namespace Webhooks.Api.Data
{
    public class WebhookDbcontext : DbContext
    {
        public DbSet<Student> students { get; set; }
        public DbSet<WebhookSubscription> subscriptions { get; set; }
        public DbSet<WebhookDeliveryAttempt> deliveryAttempts { get; set; }

        public WebhookDbcontext(DbContextOptions<WebhookDbcontext> options) : base(options)
        {
                
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Student>(builder =>
            {
                builder.ToTable("Student"); //Table: Student
                builder.HasKey(x => x.Id);
            });

            modelBuilder.Entity<WebhookSubscription>(builder =>
            {
                builder.ToTable("subscriptions", "webhooks"); //Table: Subscription, Schema:Webhook
                builder.HasKey(x => x.Id);
            });

            modelBuilder.Entity<WebhookDeliveryAttempt>(builder =>
            {
                builder.ToTable("delivery_attempts", "webhooks"); //Table: Subscription, Schema:Webhook
                builder.HasKey(x => x.Id);

                builder.HasOne<WebhookSubscription>().WithMany().HasForeignKey(d => d.SubscriptionId);
            });
        }
    }
}
