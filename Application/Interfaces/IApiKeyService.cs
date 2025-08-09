using Application.Dtos;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IApiKeyService
    {
        Task<IEnumerable<ApiKey>> GetActiveApiKeysAsync();
        Task<ApiKey?> GetApiKeyByIdAsync(int configId);
        Task CreateApiKeyAsync(ApiKeyCreateDto dto);
        Task UpdateApiKeyAsync(ApiKey apiKey);
        Task RemoveApiKeyAsync(int configId);
    }

}
