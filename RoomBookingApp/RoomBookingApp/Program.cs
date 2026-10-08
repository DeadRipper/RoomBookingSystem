
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RBA.CacheBookings;
using RBA.DBase.DBRelations;
using RBA.DBase.Managers.Admin;
using RBA.DBase.Managers.Auth;
using RBA.DBase.Managers.DbWorker;
using RBA.DBase.Managers.Room;
using RBA.DBase.Managers.User;
using RBA.DBase.Workers.AdminWorkers;
using RBA.DBase.Workers.AuthWorkers;
using RBA.DBase.Workers.RoomWorkers;
using RBA.DBase.Workers.UserWorkers;
using RoomBookingApp.Helpers;
using RoomBookingApp.Middlewares;

namespace RoomBookingApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddDbContext<AppDbContext>((sp, options) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var connectionString = config.GetConnectionString("ConnStr");
                options.UseSqlServer(connectionString);
            });

            builder.Services.AddSingleton<ICacheWorker, CBWorker>();

            builder.Services.AddTransient<IRoomManager, RoomWorker>();
            builder.Services.AddTransient<IAdminManagment, AdminWorker>();
            builder.Services.AddTransient<IUserManager, UserWorker>();
            builder.Services.AddTransient<IAuthManager, AuthWorker>();

            builder.Services.AddTransient<IRoomDbManager, RoomDbWorker>();
            builder.Services.AddTransient<IDbAuthManager, DbAuthWorker>();
            builder.Services.AddTransient<IUserDBWorker, UserDbWorker>();
            builder.Services.AddTransient<IAdminDbManager, AdminDbWorker>();


            builder.Services.AddHttpLogging(logging =>
            {
                logging.LoggingFields = HttpLoggingFields.Request | HttpLoggingFields.Response;
            });

            builder.Services.AddAuthentication(cfg =>
            {
                cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                cfg.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = false;
                x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });

            var app = builder.Build();
            
            app.UseHttpLogging();

            using (var scope = app.Services.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseAuthentication();

            app.UseHttpsRedirection();            

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
