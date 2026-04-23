using Microsoft.EntityFrameworkCore;
using Portcullis.Api.Data;
using Portcullis.Api.Domain.DTOs;
using Portcullis.Api.Domain.Entities;
using Portcullis.Api.Domain.Exceptions;

namespace Portcullis.Api.Domain.Services;

public class SecretService(PortcullisDbContext ctx) : ISecretService
{
    public Task AdminDeleteSecretAsync(Guid secretId)
    {
        throw new NotImplementedException();
    }

    public async Task<SecretResponse> CreateSecretAsync(string userId, CreateSecretRequest request)
    {
        if (await ctx.Secrets.AnyAsync(s => s.UserId == userId && s.Name == request.Name))
            throw new DuplicateSecretNameException(request.Name);
        var secret = new Secret
        {
            UserId = userId,
            Name = request.Name,
            Value = request.Value,
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

    public async Task DeleteSecretAsync(string userId, Guid secretId)
    {
        var secret =
            await ctx.Secrets.FirstOrDefaultAsync(s => s.UserId == userId && s.Id == secretId)
            ?? throw new SecretNotFoundException(secretId);
        ctx.Secrets.Remove(secret);
        await ctx.SaveChangesAsync();
    }

    public Task<PaginatedResponse<AdminSecretResponse>> GetAllSecretsAsync(
        AdminSecretQueryParameters queryParams
    )
    {
        throw new NotImplementedException();
    }

    public async Task<SecretResponse> GetSecretAsync(string userId, Guid secretId)
    {
        var secret =
            await ctx.Secrets.FirstOrDefaultAsync(s => s.UserId == userId && s.Id == secretId)
            ?? throw new SecretNotFoundException(secretId);
        return new SecretResponse
        {
            Id = secret.Id,
            Name = secret.Name,
            Value = secret.Value,
            CreatedAt = secret.CreatedAt.DateTime,
            UpdatedAt = secret.UpdatedAt.DateTime,
        };
    }

    public Task<PaginatedResponse<SecretResponse>> GetSecretsAsync(
        string userId,
        SecretQueryParameters queryParams
    )
    {
        throw new NotImplementedException();
    }

    public Task ResetSecretAsync(Guid secretId)
    {
        throw new NotImplementedException();
    }

    public async Task<SecretResponse> UpdateSecretAsync(
        string userId,
        Guid secretId,
        UpdateSecretRequest request
    )
    {
        throw new NotImplementedException();
    }
}
