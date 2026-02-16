using AutoMapper;
using Data.Context;
using Data.Repositories;
using Domain.Commands;
using Domain.DTOs;
using Domain.Handlers;
using Domain.Interface;
using Domain.Mappings;
using Domain.Models;
using Domain.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 2. Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Workflow Management API",
        Version = "v1",
        Description = "Generic Workflow Management System - PFE Project"
    });
});

// 3. Configure Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Data")
    ));

// 4. Register Generic Repository
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// 5. Register MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(User).Assembly);
});

// 6. Manually register generic handlers for each entity

// User handlers
builder.Services.AddScoped<IRequestHandler<GetListGenericQuery<User>, IEnumerable<User>>, GetListGenericHandler<User>>();
builder.Services.AddScoped<IRequestHandler<GetGenericQuery<User>, User?>, GetGenericHandler<User>>();
builder.Services.AddScoped<IRequestHandler<AddGenericCommand<User>, User>, AddGenericHandler<User>>();
builder.Services.AddScoped<IRequestHandler<UpdateGenericCommand<User>, User>, UpdateGenericHandler<User>>();
builder.Services.AddScoped<IRequestHandler<DeleteGenericCommand<User>, bool>, DeleteGenericHandler<User>>();

// Role handlers
builder.Services.AddScoped<IRequestHandler<GetListGenericQuery<Role>, IEnumerable<Role>>, GetListGenericHandler<Role>>();
builder.Services.AddScoped<IRequestHandler<GetGenericQuery<Role>, Role?>, GetGenericHandler<Role>>();
builder.Services.AddScoped<IRequestHandler<AddGenericCommand<Role>, Role>, AddGenericHandler<Role>>();
builder.Services.AddScoped<IRequestHandler<UpdateGenericCommand<Role>, Role>, UpdateGenericHandler<Role>>();
builder.Services.AddScoped<IRequestHandler<DeleteGenericCommand<Role>, bool>, DeleteGenericHandler<Role>>();

// UserRole handlers
builder.Services.AddScoped<IRequestHandler<GetListGenericQuery<UserRole>, IEnumerable<UserRole>>, GetListGenericHandler<UserRole>>();
builder.Services.AddScoped<IRequestHandler<GetGenericQuery<UserRole>, UserRole?>, GetGenericHandler<UserRole>>();
builder.Services.AddScoped<IRequestHandler<AddGenericCommand<UserRole>, UserRole>, AddGenericHandler<UserRole>>();
builder.Services.AddScoped<IRequestHandler<UpdateGenericCommand<UserRole>, UserRole>, UpdateGenericHandler<UserRole>>();
builder.Services.AddScoped<IRequestHandler<DeleteGenericCommand<UserRole>, bool>, DeleteGenericHandler<UserRole>>();

// WorkFlowDefinition handlers
builder.Services.AddScoped<IRequestHandler<GetListGenericQuery<WorkFlowDefinition>, IEnumerable<WorkFlowDefinition>>, GetListGenericHandler<WorkFlowDefinition>>();
builder.Services.AddScoped<IRequestHandler<GetGenericQuery<WorkFlowDefinition>, WorkFlowDefinition?>, GetGenericHandler<WorkFlowDefinition>>();
builder.Services.AddScoped<IRequestHandler<AddGenericCommand<WorkFlowDefinition>, WorkFlowDefinition>, AddGenericHandler<WorkFlowDefinition>>();
builder.Services.AddScoped<IRequestHandler<UpdateGenericCommand<WorkFlowDefinition>, WorkFlowDefinition>, UpdateGenericHandler<WorkFlowDefinition>>();
builder.Services.AddScoped<IRequestHandler<DeleteGenericCommand<WorkFlowDefinition>, bool>, DeleteGenericHandler<WorkFlowDefinition>>();

// Node handlers
builder.Services.AddScoped<IRequestHandler<GetListGenericQuery<Node>, IEnumerable<Node>>, GetListGenericHandler<Node>>();
builder.Services.AddScoped<IRequestHandler<GetGenericQuery<Node>, Node?>, GetGenericHandler<Node>>();
builder.Services.AddScoped<IRequestHandler<AddGenericCommand<Node>, Node>, AddGenericHandler<Node>>();
builder.Services.AddScoped<IRequestHandler<UpdateGenericCommand<Node>, Node>, UpdateGenericHandler<Node>>();
builder.Services.AddScoped<IRequestHandler<DeleteGenericCommand<Node>, bool>, DeleteGenericHandler<Node>>();

// Edge handlers
builder.Services.AddScoped<IRequestHandler<GetListGenericQuery<Edge>, IEnumerable<Edge>>, GetListGenericHandler<Edge>>();
builder.Services.AddScoped<IRequestHandler<GetGenericQuery<Edge>, Edge?>, GetGenericHandler<Edge>>();
builder.Services.AddScoped<IRequestHandler<AddGenericCommand<Edge>, Edge>, AddGenericHandler<Edge>>();
builder.Services.AddScoped<IRequestHandler<UpdateGenericCommand<Edge>, Edge>, UpdateGenericHandler<Edge>>();
builder.Services.AddScoped<IRequestHandler<DeleteGenericCommand<Edge>, bool>, DeleteGenericHandler<Edge>>();

// WorkFlowInstance handlers
builder.Services.AddScoped<IRequestHandler<GetListGenericQuery<WorkFlowInstance>, IEnumerable<WorkFlowInstance>>, GetListGenericHandler<WorkFlowInstance>>();
builder.Services.AddScoped<IRequestHandler<GetGenericQuery<WorkFlowInstance>, WorkFlowInstance?>, GetGenericHandler<WorkFlowInstance>>();
builder.Services.AddScoped<IRequestHandler<AddGenericCommand<WorkFlowInstance>, WorkFlowInstance>, AddGenericHandler<WorkFlowInstance>>();
builder.Services.AddScoped<IRequestHandler<UpdateGenericCommand<WorkFlowInstance>, WorkFlowInstance>, UpdateGenericHandler<WorkFlowInstance>>();
builder.Services.AddScoped<IRequestHandler<DeleteGenericCommand<WorkFlowInstance>, bool>, DeleteGenericHandler<WorkFlowInstance>>();

// 7. Register AutoMapper (if you have mapping profiles)
// builder.Services.AddAutoMapper(typeof(WorkflowMappingProfile).Assembly);

// 8. Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 9. Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// 10. Configure Middleware
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

app.Logger.LogInformation("🚀 Workflow Management API Started!");
app.Logger.LogInformation("📊 Entities: Users, Roles, UserRoles, WorkFlowDefinitions, Nodes, Edges, WorkFlowInstances");

app.Run();