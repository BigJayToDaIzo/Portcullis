using Portcullis.Api.Domain.DTOs;

namespace Portcullis.Api.Domain.Services;

public interface ISecretService
{
    public Task<PaginatedResponse<SecretResponse>> GetSecretsAsync(
        string userId,
        SecretQueryParameters queryParams
    );
    public Task<SecretResponse> GetSecretAsync(string userId, Guid secretId);
    public Task<SecretResponse> CreateSecretAsync(string userId, CreateSecretRequest request);
    public Task<SecretResponse> UpdateSecretAsync(
        string userId,
        Guid secretId,
        UpdateSecretRequest request
    );
    public Task DeleteSecretAsync(string userId, Guid secretId);
    public Task<PaginatedResponse<AdminSecretResponse>> GetAllSecretsAsync(
        AdminSecretQueryParameters queryParams
    );
    public Task ResetSecretAsync(Guid secretId);
    public Task AdminDeleteSecretAsync(Guid secretId);
}
