using Data.Context;
using Data.Repositories;
using Domain.Commands;
using Domain.Handlers;
using Domain.Interface;
using Domain.Models;
using Domain.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infra
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // ── Database ──────────────────────────────────────────────────────
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("Data")
                ));

            // ── Generic Repository ────────────────────────────────────────────
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // ── MediatR ───────────────────────────────────────────────────────
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(User).Assembly));

            // ── AutoMapper ────────────────────────────────────────────────────
            services.AddAutoMapper(typeof(User).Assembly);

            // ── MediatR CRUD Handlers ─────────────────────────────────────────
            RegisterHandlers<User>(services);
            RegisterHandlers<Role>(services);
            RegisterHandlers<UserRole>(services);
            RegisterHandlers<WorkFlowDefinition>(services);
            RegisterHandlers<Node>(services);
            RegisterHandlers<Edge>(services);
            RegisterHandlers<WorkFlowInstance>(services);
            RegisterHandlers<WorkFlowInstanceHistory>(services);

            return services;
        }

        private static void RegisterHandlers<T>(IServiceCollection services) where T : BaseEntity
        {
            services.AddScoped<IRequestHandler<GetListGenericQuery<T>, IEnumerable<T>>, GetListGenericHandler<T>>();
            services.AddScoped<IRequestHandler<GetGenericQuery<T>, T?>, GetGenericHandler<T>>();
            services.AddScoped<IRequestHandler<AddGenericCommand<T>, T>, AddGenericHandler<T>>();
            services.AddScoped<IRequestHandler<UpdateGenericCommand<T>, T>, UpdateGenericHandler<T>>();
            services.AddScoped<IRequestHandler<DeleteGenericCommand<T>, bool>, DeleteGenericHandler<T>>();
        }
    }
}
