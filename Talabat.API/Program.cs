using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Talabat.API.Errors;
using Talabat.API.Extensions;
using Talabat.API.Helpers;
using Talabat.API.Middlewares;
using Talabat.Core.Entities.Identity;
using Talabat.Core.IRepository;
using Talabat.Repository;
using Talabat.Repository.Data;
using Talabat.Repository.Identity;

namespace Talabat.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            #region Configure Service - Add services to the container
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<StoreContext>(Options =>
            {
                Options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                    sqlServerOptions => sqlServerOptions.EnableRetryOnFailure());
            });
            builder.Services.AddDbContext<AppIdentityDbContext>(Options =>
            {
                Options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });

            builder.Services.AddApplicationService();

            builder.Services.AddSingleton<IConnectionMultiplexer>(Options =>
            {
                var Connection = builder.Configuration.GetConnectionString("RedisConnection");
                return ConnectionMultiplexer.Connect(Connection);
            });

            builder.Services.AddIdentityServices(builder.Configuration);
            builder.Services.AddCors(Options =>
            {
                Options.AddPolicy("CorsPolicy", options =>
                {
                    options.AllowAnyHeader();
                    options.AllowAnyMethod();
                    options.WithOrigins(builder.Configuration["FrontBaseUrl"]);
                });
            });
            #endregion

            var app = builder.Build();

            #region Update-Database

            using var Scope = app.Services.CreateScope(); // Group of Services that's lifetime is Scooped
            var Services = Scope.ServiceProvider; // Service it Self
            var LoggerFactory = Services.GetRequiredService<ILoggerFactory>();
            try
            {
                var dbContext = Services.GetRequiredService<StoreContext>();
                // Ask CLR for creating object from DbContext Explictly
                
                await dbContext.Database.MigrateAsync(); // Update-Database

                var IdentityDbContext = Services.GetRequiredService<AppIdentityDbContext>();
                await IdentityDbContext.Database.MigrateAsync();

                await StoreContextSeed.SeedAsync(dbContext);

                var UserManger = Services.GetRequiredService<UserManager<AppUser>>();
                await AppIdentityDbContextSeed.SeedUserAsync(UserManger);
            }
            catch (Exception ex)
            {
                var Logger = LoggerFactory.CreateLogger<Program>();
                Logger.LogError(ex, "This Error Happened During Applying Migration");
            }

            #endregion


            #region Configure - Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
            }
            app.AddSwaggerMiddlewares();
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseStaticFiles();
            app.UseStatusCodePagesWithReExecute("/error/{0}");
            app.UseHttpsRedirection();
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers(); 
            #endregion


            app.Run();
        }
    }
}
