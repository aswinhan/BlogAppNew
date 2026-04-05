// src/DigitalPlatform.Api/Program.cs
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Modules.Blog.Features;
using Modules.Blog.Infrastructure.Database;
using Modules.Common.API.ErrorHandling;
using Modules.Common.API.Extensions;
using Modules.Common.Infrastructure.Outbox;
using Modules.CRM.Features;
using Modules.CRM.Infrastructure.Database;
using Modules.Identity.Features.Users;
using Modules.Identity.Infrastructure.Auth;
using Modules.Identity.Infrastructure.Database;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Aspire Service Defaults (OpenTelemetry, Metrics, Health Checks)
builder.AddServiceDefaults();

// 2. Global Exception Handling (RFC 7807)
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddDistributedMemoryCache(); // Fallback for EF Core Design-Time
builder.AddRedisDistributedCache("redis");    // Run-Time Aspire Redis injection

// --- ADD AUTHENTICATION & AUTHORIZATION ---
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddTransient<ITokenProvider, TokenProvider>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
        };
    });

builder.Services.AddAuthorization();

// 3. Database Contexts & Outbox Interceptor
var outboxInterceptor = new InsertOutboxMessagesInterceptor();

builder.AddNpgsqlDbContext<IdentityDbContext>("postgres", configureDbContextOptions: options =>
    options.AddInterceptors(outboxInterceptor));
builder.Services.AddScoped<IIdentityDbContext>(sp => sp.GetRequiredService<IdentityDbContext>());

builder.AddNpgsqlDbContext<BlogDbContext>("postgres", configureDbContextOptions: options =>
    options.AddInterceptors(outboxInterceptor));
builder.Services.AddScoped<IBlogDbContext>(sp => sp.GetRequiredService<BlogDbContext>());

builder.AddNpgsqlDbContext<CrmDbContext>("postgres", configureDbContextOptions: options =>
    options.AddInterceptors(outboxInterceptor));
builder.Services.AddScoped<ICrmDbContext>(sp => sp.GetRequiredService<CrmDbContext>());

// 4. MediatR Registration (Scanning all loaded module assemblies)
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(
        typeof(Modules.Identity.Features.Users.Register.RegisterUserCommand).Assembly,
        typeof(Modules.Blog.Features.Articles.Create.CreateArticleCommand).Assembly,
        typeof(Modules.CRM.Features.Subscribers.Subscribe.SubscribeCommand).Assembly
    );
});

// 5. Endpoint Discovery
builder.Services.AddEndpoints(typeof(Modules.Identity.Features.Users.Register.RegisterUserEndpoint).Assembly);
builder.Services.AddEndpoints(typeof(Modules.Blog.Features.Articles.Create.CreateArticleEndpoint).Assembly);
builder.Services.AddEndpoints(typeof(Modules.CRM.Features.Subscribers.Subscribe.SubscribeEndpoint).Assembly);

// 6. ADD SWAGGER UI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<DigitalPlatform.Api.BackgroundJobs.BlogOutboxProcessor>();

var useAzure = builder.Configuration.GetValue<bool>("Storage:UseAzure");
if (useAzure)
{
    var blobConn = builder.Configuration.GetConnectionString("BlobStorage");
    builder.Services.AddSingleton(x => new Azure.Storage.Blobs.BlobServiceClient(blobConn));
    builder.Services.AddScoped<Modules.Common.Application.Storage.IFileService, Modules.Common.Infrastructure.Storage.AzureBlobFileService>();
}
else
{
    builder.Services.AddScoped<Modules.Common.Application.Storage.IFileService, Modules.Common.Infrastructure.Storage.LocalFileService>();
}

var app = builder.Build();

// 7. ENABLE SWAGGER UI IN PIPELINE
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapDefaultEndpoints();
app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

// Auto-migrate databases for local development (Big Tech DX standard)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Applying database migrations...");

        // By pulling the execution strategy, we allow EF Core to automatically retry if the network blips
        var identityDb = services.GetRequiredService<IdentityDbContext>();
        identityDb.Database.CreateExecutionStrategy().Execute(() => identityDb.Database.Migrate());

        var blogDb = services.GetRequiredService<BlogDbContext>();
        blogDb.Database.CreateExecutionStrategy().Execute(() => blogDb.Database.Migrate());

        var crmDb = services.GetRequiredService<CrmDbContext>();
        crmDb.Database.CreateExecutionStrategy().Execute(() => crmDb.Database.Migrate());

        logger.LogInformation("Database migrations applied successfully.");
    }
    catch (Exception ex)
    {
        logger.LogCritical(ex, "A fatal error occurred while migrating the databases.");
        throw; // We must crash the app if the DB is in an invalid state
    }
}

app.MapEndpoints();

app.Run();