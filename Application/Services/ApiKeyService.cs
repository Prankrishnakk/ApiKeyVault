using Application.Dtos;
using Application.Interfaces;
using Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ApiKeyService : IApiKeyService
    {
        private readonly IApiKeyRepository _repository;

        public ApiKeyService(IApiKeyRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ApiKey>> GetActiveApiKeysAsync()
        {
            return await _repository.GetAllActiveAsync();
        }

        public async Task<ApiKey?> GetApiKeyByIdAsync(int configId)
        {
            if (configId <= 0)
                throw new ArgumentException("ConfigId must be greater than zero.", nameof(configId));

            return await _repository.GetByIdAsync(configId);
        }

        public async Task CreateApiKeyAsync(ApiKeyCreateDto dto)
        {
            ValidateApiKeyDto(dto);

            var apiKey = new ApiKey
            {
                ProjectName = dto.ProjectName,
                KeyName = dto.KeyName,
                RotationMinutes = dto.RotationMinutes,
                RotationSeconds =dto.RotationSeconds,
              
            };

            await _repository.AddAsync(apiKey);
        }

        public async Task UpdateApiKeyAsync(ApiKey apiKey)
        {
            ValidateApiKeyDomain(apiKey);

            if (apiKey.ConfigId <= 0)
                throw new ArgumentException("ConfigId must be provided for update.", nameof(apiKey.ConfigId));

            var existingKey = await _repository.GetByIdAsync(apiKey.ConfigId);
            if (existingKey == null)
                throw new KeyNotFoundException($"API key with ID {apiKey.ConfigId} not found.");

            // Update fields
            existingKey.ProjectName = apiKey.ProjectName;
            existingKey.KeyName = apiKey.KeyName;
            existingKey.RotationMinutes = apiKey.RotationMinutes;
            existingKey.IsActive = apiKey.IsActive;
            existingKey.LastRotated = apiKey.LastRotated;
            existingKey.NextRotation = apiKey.NextRotation;
            existingKey.CreatedAt = apiKey.CreatedAt;
            existingKey.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(existingKey);
        }

        public async Task RemoveApiKeyAsync(int configId)
        {
            var existingKey = await _repository.GetByIdAsync(configId);
            if (existingKey == null)
                throw new KeyNotFoundException($"API key with ID {configId} not found.");

            await _repository.DeleteAsync(configId);
        }

        private static void ValidateApiKeyDto(ApiKeyCreateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.ProjectName))
                throw new ArgumentException("ProjectName is required.", nameof(dto.ProjectName));

            if (string.IsNullOrWhiteSpace(dto.KeyName))
                throw new ArgumentException("KeyName is required.", nameof(dto.KeyName));

            if (dto.RotationMinutes < 0)
                throw new ArgumentException("RotationMinutes must be greater than zero.", nameof(dto.RotationMinutes));
        }

        private static void ValidateApiKeyDomain(ApiKey apiKey)
        {
            if (apiKey == null)
                throw new ArgumentNullException(nameof(apiKey));

            if (string.IsNullOrWhiteSpace(apiKey.ProjectName))
                throw new ArgumentException("ProjectName is required.", nameof(apiKey.ProjectName));

            if (string.IsNullOrWhiteSpace(apiKey.KeyName))
                throw new ArgumentException("KeyName is required.", nameof(apiKey.KeyName));

            if (apiKey.RotationMinutes < 0)
                throw new ArgumentException("RotationMinutes must be greater than zero.", nameof(apiKey.RotationMinutes));
        }
    }
}
