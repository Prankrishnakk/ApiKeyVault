using Application.Interfaces;
using Dapper;
using Domain;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class ApiKeyRepository : IApiKeyRepository
    {
        private readonly IDbConnection _dbConnection;

        public ApiKeyRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<ApiKey>> GetAllActiveAsync()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Flag", 1);

            var apiKeys = await _dbConnection.QueryAsync<ApiKey>(
                "sp_MasterApiKey", parameters, commandType: CommandType.StoredProcedure);

            return apiKeys;
        }

        public async Task<ApiKey?> GetByIdAsync(int configId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Flag", 4);
            parameters.Add("@ConfigId", configId);

            var apiKey = await _dbConnection.QueryFirstOrDefaultAsync<ApiKey>(
                "sp_MasterApiKey", parameters, commandType: CommandType.StoredProcedure);

            return apiKey;
        }

        public async Task AddAsync(ApiKey apiKey)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Flag", 2);

            // Serialize only properties required by the SP insert logic
            var jsonData = new
            {
                apiKey.ProjectName,
                apiKey.KeyName,
                apiKey.RotationMinutes,
                apiKey.RotationSeconds
            };

            parameters.Add("@JsonData", JsonSerializer.Serialize(jsonData));

            await _dbConnection.ExecuteAsync(
                "sp_MasterApiKey", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(ApiKey apiKey)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Flag", 3);
            parameters.Add("@ConfigId", apiKey.ConfigId);

            var jsonData = new
            {
                apiKey.ProjectName,
                apiKey.KeyName,
                apiKey.LastRotated,
                apiKey.RotationMinutes,
                apiKey.NextRotation,
                apiKey.IsActive,
                apiKey.CreatedAt,
                apiKey.UpdatedAt
            };

            parameters.Add("@JsonData", JsonSerializer.Serialize(jsonData));

            await _dbConnection.ExecuteAsync(
                "sp_MasterApiKey", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(int configId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Flag", 5);
            parameters.Add("@ConfigId", configId);

            await _dbConnection.ExecuteAsync(
                "sp_MasterApiKey", parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
