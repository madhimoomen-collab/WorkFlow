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
    cfg.RegisterServicesFromAssembly(typeof(Book).Assembly);
});

// 6. Manually register generic handlers for each entity

// Book handlers
builder.Services.AddScoped<IRequestHandler<GetListGenericQuery<Book>, IEnumerable<Book>>, GetListGenericHandler<Book>>();
builder.Services.AddScoped<IRequestHandler<GetGenericQuery<Book>, Book?>, GetGenericHandler<Book>>();
builder.Services.AddScoped<IRequestHandler<AddGenericCommand<Book>, Book>, AddGenericHandler<Book>>();
builder.Services.AddScoped<IRequestHandler<UpdateGenericCommand<Book>, Book>, UpdateGenericHandler<Book>>();
builder.Services.AddScoped<IRequestHandler<DeleteGenericCommand<Book>, bool>, DeleteGenericHandler<Book>>();

// Client handlers
builder.Services.AddScoped<IRequestHandler<GetListGenericQuery<Client>, IEnumerable<Client>>, GetListGenericHandler<Client>>();
builder.Services.AddScoped<IRequestHandler<GetGenericQuery<Client>, Client?>, GetGenericHandler<Client>>();
builder.Services.AddScoped<IRequestHandler<AddGenericCommand<Client>, Client>, AddGenericHandler<Client>>();
builder.Services.AddScoped<IRequestHandler<UpdateGenericCommand<Client>, Client>, UpdateGenericHandler<Client>>();
builder.Services.AddScoped<IRequestHandler<DeleteGenericCommand<Client>, bool>, DeleteGenericHandler<Client>>();

// ClientBook handlers
builder.Services.AddScoped<IRequestHandler<GetListGenericQuery<ClientBook>, IEnumerable<ClientBook>>, GetListGenericHandler<ClientBook>>();
builder.Services.AddScoped<IRequestHandler<GetGenericQuery<ClientBook>, ClientBook?>, GetGenericHandler<ClientBook>>();
builder.Services.AddScoped<IRequestHandler<AddGenericCommand<ClientBook>, ClientBook>, AddGenericHandler<ClientBook>>();
builder.Services.AddScoped<IRequestHandler<UpdateGenericCommand<ClientBook>, ClientBook>, UpdateGenericHandler<ClientBook>>();
builder.Services.AddScoped<IRequestHandler<DeleteGenericCommand<ClientBook>, bool>, DeleteGenericHandler<ClientBook>>();

// Reservation handlers (NEW)
builder.Services.AddScoped<IRequestHandler<GetListGenericQuery<Reservation>, IEnumerable<Reservation>>, GetListGenericHandler<Reservation>>();
builder.Services.AddScoped<IRequestHandler<GetGenericQuery<Reservation>, Reservation?>, GetGenericHandler<Reservation>>();
builder.Services.AddScoped<IRequestHandler<AddGenericCommand<Reservation>, Reservation>, AddGenericHandler<Reservation>>();
builder.Services.AddScoped<IRequestHandler<UpdateGenericCommand<Reservation>, Reservation>, UpdateGenericHandler<Reservation>>();
builder.Services.AddScoped<IRequestHandler<DeleteGenericCommand<Reservation>, bool>, DeleteGenericHandler<Reservation>>();

// 7. Register AutoMapper
builder.Services.AddAutoMapper(typeof(ReservationMappingProfile).Assembly);

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
app.Logger.LogInformation("📚 Endpoints: Books, Clients, ClientBooks, Reservations");

app.Run();