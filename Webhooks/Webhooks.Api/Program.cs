using Webhooks.Api.Repositories;
using Webhooks.Api.DTO;
using Webhooks.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Webhooks.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<InMemoryStudentRepository>();  //REgister all services like this, or error while accessing EP
builder.Services.AddSingleton<InMemoryWebhookSubscriptionRepository>();
builder.Services.AddHttpClient<WebhookDispatcher>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/students", ([FromBody] RegisterStudentRequest request, [FromServices] InMemoryStudentRepository studentRepository, WebhookDispatcher webhookDispatcher) =>
{
    var student = new Student() { Id= Guid.NewGuid(), Name=request.Name, Grade=request.Grade};
    studentRepository.Add(student);
    webhookDispatcher.Dispatch("student.created", student);
    return Results.Ok(student);
})
.WithName("Student")
.WithOpenApi();

app.MapGet("/students", (InMemoryStudentRepository studentRepository) => {
    return studentRepository.GetAll();
}).WithTags("Students");

app.MapPost("/webhooks/subscriptions", ([FromBody] CreateWebhookRequest request, [FromServices]InMemoryWebhookSubscriptionRepository webhookRepository) => {
    WebhookSubscription subscription = new WebhookSubscription() { Id = Guid.NewGuid(), EventType= request.EventType, CreatedAt=DateTime.Now, WebhookUrl = request.WebhookUrl };
    return Results.Ok(subscription);
});

app.Run();

