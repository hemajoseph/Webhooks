using System.Runtime.CompilerServices;
using Webhooks.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Webhooks.Api.Extensions
{
    public static class WebApplicationExtensions
    {
        public static async Task ApplyMigrationsAsync(this WebApplication app) {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<WebhookDbcontext>();

            await db.Database.MigrateAsync();
        }

        public static async Task ApplyMigrationsWithRetryAsync(this WebApplication app, int retries = 5)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<WebhookDbcontext>();

            for (int i = 0; i < retries; i++)
            {
                try
                {
                    await db.Database.MigrateAsync();
                    return;
                }
                catch (Npgsql.NpgsqlException ex)
                {
                    Console.WriteLine($"Attempt {i + 1} failed: {ex.Message}");
                    await Task.Delay(2000); // wait 2 seconds and try again
                }
            }

            throw new Exception("Failed to apply migrations after retrying.");
        }
    }
}
