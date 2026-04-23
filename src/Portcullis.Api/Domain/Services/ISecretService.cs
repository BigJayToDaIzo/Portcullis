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

    public Task<SecretResponse> RenameSecretAsync(
        string userId,
        Guid secretId,
        RenameSecretRequest request
    );
    public Task<SecretResponse> RotateSecretAsync(
        string userId,
        Guid secretId,
        RotateSecretRequest request
    );
    public Task DeleteSecretAsync(string userId, Guid secretId);
    public Task<PaginatedResponse<AdminSecretResponse>> GetAllSecretsAsync(
        AdminSecretQueryParameters queryParams
    );
    public Task ResetSecretAsync(Guid secretId);
    public Task AdminDeleteSecretAsync(Guid secretId);
}
