using ApiVault.Middleware;
using ApiVault.MiddleWare;
using Application.Interfaces;
using Application.Service;
using Application.Services;
using Infrastructure.DapperContext;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Data;
using System.Data.SqlClient;


namespace ApiVault
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<Dappercontext>();

            builder.Services.AddScoped<IDbConnection>(sp =>
            {
                var context = sp.GetRequiredService<Dappercontext>();
                return context.CreateConnection();
            });

            // Register your services and repositories
            builder.Services.AddScoped<IApiKeyService, ApiKeyService>();
            builder.Services.AddScoped<IApiKeyRepository, ApiKeyRepository>();

            // Register the background service for API key rotation
            builder.Services.AddHostedService<ApiKeyRotationService>();

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Register your global exception middleware first
            app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

            // Register your API key validation middleware next
            app.UseMiddleware<ApiKeyMiddleware>();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
