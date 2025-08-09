using Dapper;
using Domain;
using Microsoft.AspNetCore.Http;
using System;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiVault.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string HeaderName = "x-api-key";

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IDbConnection dbConnection)
        {
            var path = context.Request.Path.Value?.ToLower() ?? "";

            // Skip API key check for create or other public endpoints
            if (path.StartsWith("/api/apikey"))
            {
                await _next(context);
                return;
            }

            var apiKeyHeader = context.Request.Headers[HeaderName].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(apiKeyHeader))
                throw new UnauthorizedAccessException("API Key is missing.");

            // Query the key from DB using Dapper
            var query = @"
                SELECT TOP 1 *
                FROM dbo.mtbl_master_apikeys
                WHERE IsActive = 1 AND KeyValue = @KeyValue";

            var key = (await dbConnection.QueryAsync<ApiKey>(query, new { KeyValue = apiKeyHeader }))
                      .FirstOrDefault();

            if (key == null)
                throw new UnauthorizedAccessException("Invalid API Key.");

            var elapsed = DateTime.UtcNow - key.LastRotated;
            if (elapsed.TotalMinutes >= key.RotationMinutes)
                throw new UnauthorizedAccessException("API Key has expired.");

            context.Items["ApiKey"] = key;

            await _next(context);
        }
    }


}
