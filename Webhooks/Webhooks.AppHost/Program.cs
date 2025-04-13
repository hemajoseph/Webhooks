var builder = DistributedApplication.CreateBuilder(args);

var database = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin()
    .AddDatabase("webhooks");

builder.AddProject<Projects.Webhooks_Api>("webhooks-api");

builder.Build().Run();
