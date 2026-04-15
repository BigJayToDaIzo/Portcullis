using Portcullis.Api.Domain.DTOs;

namespace Portcullis.Api.Domain.Services
{
    public interface ISecretService
    {
        public Task<PaginatedResponse<SecretResponse>> GetSecretsAsync(
            string username,
            SecretQueryParameters queryParams
        );
        public Task<SecretResponse> GetSecretAsync(string username, Guid secretId);
        public Task<SecretResponse> CreateSecretAsync(string username, CreateSecretRequest request);
        public Task<SecretResponse> UpdateSecretAsync(
            string username,
            Guid secretId,
            UpdateSecretRequest request
        );
        public Task DeleteSecretAsync(string username, Guid userId);
        public Task<PaginatedResponse<AdminSecretResponse>> GetAllSecretsAsync(
            AdminSecretQueryParameters queryParams
        );
        public Task ResetSecretAsync(Guid id);
        public Task AdminDeleteSecretAsync(Guid id);
    }
}
