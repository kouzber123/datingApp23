using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using API.SignalR;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<DataContext>(opt =>
         {
             opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
         });
            services.AddCors();
            //scope of http level dies afgter call
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<LogUserActivity>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddSignalR();
            //singleton = server wide life cycle does not die after http request
            services.AddSingleton<PresenceTracker>();
            //section alt for connection string
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
