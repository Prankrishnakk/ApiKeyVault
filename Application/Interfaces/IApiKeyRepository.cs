using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IApiKeyRepository
    {
        Task<IEnumerable<ApiKey>> GetAllActiveAsync();
        Task<ApiKey?> GetByIdAsync(int configId);
        Task AddAsync(ApiKey apiKey);
        Task UpdateAsync(ApiKey apiKey);
        Task DeleteAsync(int configId);
    }

}
