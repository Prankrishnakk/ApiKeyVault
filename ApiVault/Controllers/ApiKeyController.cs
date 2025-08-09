using Application.ApiResponse;
using Application.Dtos;
using Application.Interfaces;
using Domain;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuthKey.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiKeyController : ControllerBase
    {
        private readonly IApiKeyService _apiKeyService;

        public ApiKeyController(IApiKeyService apiKeyService)
        {
            _apiKeyService = apiKeyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActive()
        {
            var apiKeys = await _apiKeyService.GetActiveApiKeysAsync();
            return Ok(ApiResponse<IEnumerable<ApiKey>>.Success(apiKeys));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var apiKey = await _apiKeyService.GetApiKeyByIdAsync(id);
            if (apiKey == null)
                return NotFound(ApiResponse<ApiKey>.Fail($"API key with ID {id} not found.", 404));

            return Ok(ApiResponse<ApiKey>.Success(apiKey));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ApiKeyCreateDto dto)
        {
            await _apiKeyService.CreateApiKeyAsync(dto);
            return Ok(ApiResponse<object>.Success(null, "API key created successfully."));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ApiKey apiKey)
        {
            await _apiKeyService.UpdateApiKeyAsync(apiKey);
            return Ok(ApiResponse<object>.Success(null, "API key updated successfully."));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _apiKeyService.RemoveApiKeyAsync(id);
            return Ok(ApiResponse<object>.Success(null, "API key deleted successfully."));
        }
    }
}
