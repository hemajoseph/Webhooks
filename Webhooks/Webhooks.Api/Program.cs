using Webhooks.Api.Repositories;
using Webhooks.Api.DTO;
using Webhooks.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Webhooks.Api.Services;
using Webhooks.Api.Data;
using Microsoft.EntityFrameworkCore;
using Webhooks.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddSingleton<InMemoryStudentRepository>();  //REgister all services like this, or error while accessing EP
//builder.Services.AddSingleton<InMemoryWebhookSubscriptionRepository>();
//builder.Services.AddHttpClient<WebhookDispatcher>();  //call this when httpclient is needed as DI instead of httpclientfactory

builder.Services.AddScoped<WebhookDispatcher>(); 
builder.Services.AddHttpClient();
builder.Services.AddDbContext<WebhookDbcontext>(options => {
    options.UseNpgsql(builder.Configuration.GetConnectionString("webhooks"));
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    await app.ApplyMigrationsWithRetryAsync(); //User defined Extension method for the app (Webapplication class) defind in \Extensions folder
}

app.UseHttpsRedirection();

app.MapPost("/students", ([FromBody] RegisterStudentRequest request, [FromServices] WebhookDbcontext dbContext, [FromServices] WebhookDispatcher webhookDispatcher) =>
{
    var student = new Student() { Id= Guid.NewGuid(), Name=request.Name, Grade=request.Grade};
    //studentRepository.Add(student);
    dbContext.students.Add(student);
    //webhookDispatcher.Dispatch<Student>("student.created", student);
    webhookDispatcher.DispatchAsync<Student>("student.created", student);
    return Results.Ok(student);
})
.WithName("StudentRegister")
.WithOpenApi();

app.MapGet("/students", (WebhookDbcontext dbContext) => {
    //return studentRepository.GetAll();
    dbContext.students.ToList<Student>();
}).WithTags("Students");

app.MapPost("/webhooks/subscriptions", ([FromBody] CreateWebhookRequest request, [FromServices] WebhookDbcontext dbContext) => {
    WebhookSubscription subscription = new WebhookSubscription() { Id = Guid.NewGuid(), EventType= request.EventType, CreatedAt=DateTime.UtcNow, WebhookUrl = request.WebhookUrl };
    //webhookRepository.Add(subscription);
    dbContext.subscriptions.Add(subscription);
    dbContext.SaveChanges();
    return Results.Ok(subscription);
});

app.MapGet("/webhooks/subscriptions", ([FromServices] WebhookDbcontext dbContext) =>
{
    //var subscriptions = webhookRepository.GetAll();
    var subscriptions = dbContext.subscriptions.ToList<WebhookSubscription>();
    return Results.Ok(subscriptions);
});

app.Run();

