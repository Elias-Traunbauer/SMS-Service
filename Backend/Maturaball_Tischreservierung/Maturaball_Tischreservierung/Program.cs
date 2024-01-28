
using Maturaball_Tischreservierung.Middlewares;
using Maturaball_Tischreservierung.Models;
using Maturaball_Tischreservierung.Services;

namespace Maturaball_Tischreservierung
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            ApiConfig apiConfig = new();
            builder.Configuration.Bind("ApiConfig", apiConfig);
            builder.Services.AddSingleton(apiConfig);

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<ServerSettingsService>();
            builder.Services.AddScoped<EmailDeliveryService>();
            builder.Services.AddScoped<ClientService>();

            builder.Services.AddSingleton<JsonWebTokenService>();
            builder.Services.AddSingleton<SessionBlacklistService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseUserAuthentication();

            app.MapControllers();

            app.Run();
        }
    }
}
