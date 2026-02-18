using Data.Context;
using Data.Repositories;
using Domain.Commands;
using Domain.Handlers;
using Domain.Interface;
using Domain.Models;
using Domain.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── Controllers & Swagger ────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Workflow Management API",
        Version = "v1",
        Description = "Generic Workflow Management System"
    });
});

// ── Database ─────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Data")
    ));

// ── Generic Repository ────────────────────────────────────────────────────────
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// ── MediatR ───────────────────────────────────────────────────────────────────
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(User).Assembly));

// Helper to register all 5 CRUD handlers for one entity type
static void RegisterHandlers<T>(IServiceCollection services) where T : BaseEntity
{
    services.AddScoped<IRequestHandler<GetListGenericQuery<T>, IEnumerable<T>>, GetListGenericHandler<T>>();
    services.AddScoped<IRequestHandler<GetGenericQuery<T>, T?>, GetGenericHandler<T>>();
    services.AddScoped<IRequestHandler<AddGenericCommand<T>, T>, AddGenericHandler<T>>();
    services.AddScoped<IRequestHandler<UpdateGenericCommand<T>, T>, UpdateGenericHandler<T>>();
    services.AddScoped<IRequestHandler<DeleteGenericCommand<T>, bool>, DeleteGenericHandler<T>>();
}

RegisterHandlers<User>(builder.Services);
RegisterHandlers<Role>(builder.Services);
RegisterHandlers<UserRole>(builder.Services);
RegisterHandlers<WorkFlowDefinition>(builder.Services);
RegisterHandlers<Node>(builder.Services);
RegisterHandlers<Edge>(builder.Services);
RegisterHandlers<WorkFlowInstance>(builder.Services);

// ── CORS ──────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// ── Logging ───────────────────────────────────────────────────────────────────
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// ── Middleware ────────────────────────────────────────────────────────────────
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Workflow API V1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Logger.LogInformation("🚀 Workflow Management API started");

app.Run();