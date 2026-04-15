using Microsoft.EntityFrameworkCore;
using Portcullis.Api.Data;
using Portcullis.Api.Domain.DTOs;
using Portcullis.Api.Domain.Entities;
using Portcullis.Api.Domain.Exceptions;

namespace Portcullis.Api.Domain.Services
{
    public class SecretService(PortcullisDbContext ctx) : ISecretService
    {
        public Task AdminDeleteSecretAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<SecretResponse> CreateSecretAsync(string userId, CreateSecretRequest req)
        {
            if (await ctx.Secrets.AnyAsync(s => s.UserId == userId && s.Name == req.Name))
                throw new DuplicateSecretNameException(req.Name);
            var secret = new Secret
            {
                UserId = userId,
                Name = req.Name,
                Value = req.Value,
            };
            ctx.Secrets.Add(secret);
            await ctx.SaveChangesAsync();
            return new SecretResponse
            {
                Id = secret.Id,
                Name = secret.Name,
                Value = secret.Value,
                CreatedAt = secret.CreatedAt.DateTime,
                UpdatedAt = secret.UpdatedAt.DateTime,
            };
        }

        public Task DeleteSecretAsync(string username, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedResponse<AdminSecretResponse>> GetAllSecretsAsync(
            AdminSecretQueryParameters queryParams
        )
        {
            throw new NotImplementedException();
        }

        public Task<SecretResponse> GetSecretAsync(string username, Guid secretId)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedResponse<SecretResponse>> GetSecretsAsync(
            string username,
            SecretQueryParameters queryParams
        )
        {
            throw new NotImplementedException();
        }

        public Task ResetSecretAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<SecretResponse> UpdateSecretAsync(
            string username,
            Guid secretId,
            UpdateSecretRequest request
        )
        {
            throw new NotImplementedException();
        }
    }
}
