using Dapper;
using Domain;
using Microsoft.Data.SqlClient; // Make sure to import this for SqlConnection
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Service
{
    public class ApiKeyRotationService : BackgroundService
    {
        private readonly ILogger<ApiKeyRotationService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public ApiKeyRotationService(ILogger<ApiKeyRotationService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var dbConnection = scope.ServiceProvider.GetRequiredService<IDbConnection>();

                    if (dbConnection.State != ConnectionState.Open)
                    {
                        if (dbConnection is SqlConnection sqlConn)
                        {
                            await sqlConn.OpenAsync(stoppingToken);
                        }
                        else
                        {
                            // fallback to synchronous open if not SqlConnection
                            dbConnection.Open();
                        }
                    }

                    var parameters = new DynamicParameters();
                    parameters.Add("@Flag", 1);

                    var rotatedKeys = await dbConnection.QueryAsync<ApiKey>(
                        "sp_MasterApiKey",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    if (rotatedKeys.AsList().Count == 0)
                    {
                        _logger.LogInformation("No API keys to rotate at this time.");
                    }
                    else
                    {
                        foreach (var key in rotatedKeys)
                        {
                            _logger.LogInformation($"API key for project '{key.ProjectName}' with KeyName '{key.KeyName}' is active. Next rotation at {key.NextRotation}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during API key rotation.");
                }

                await Task.Delay(TimeSpan.FromSeconds(40), stoppingToken);
            }
        }

     
    }
}
