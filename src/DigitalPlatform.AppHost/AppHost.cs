// src/DigitalPlatform.AppHost/Program.cs
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin();

var redis = builder.AddRedis("redis")
    .WithRedisCommander();

var api = builder.AddProject<Projects.DigitalPlatform_Api>("api")
    .WithReference(postgres)
    .WaitFor(postgres) // CRITICAL: Do not start API until Postgres is fully online
    .WithReference(redis)
    .WaitFor(redis);   // CRITICAL: Do not start API until Redis is fully online

builder.Build().Run();