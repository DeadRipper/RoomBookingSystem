
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using RBA.CacheBookings;
using RBA.DBase;
using RBA.DBase.DBRelations;
using RBA.DBase.Managers;
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

            builder.Services.AddScoped<IRoomManagment, RoomManagment>();
            builder.Services.AddScoped<IDBWorker, DBWorker>();
            builder.Services.AddSingleton<ICacheWorker, CBWorker>();            

            builder.Services.AddHttpLogging(logging =>
            {
                logging.LoggingFields = HttpLoggingFields.Request | HttpLoggingFields.Response;
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

            app.UseHttpsRedirection();

            //app.UseMiddleware<LogRequestMiddleware>();

            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}
