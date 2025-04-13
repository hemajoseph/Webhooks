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
    }
}
