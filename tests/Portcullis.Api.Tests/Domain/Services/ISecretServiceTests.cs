using NSubstitute;
using Portcullis.Api.Domain.DTOs;
using Portcullis.Api.Domain.Services;

namespace Portcullis.Api.Tests.Domain.Services;

public class ISecretServiceTests
{
    [Fact]
    public async Task GetSecretsAsync_AcceptsUserIdAndQueryParams_ReturnsPaginatedSecretResponse()
    {
        var service = Substitute.For<ISecretService>();
        var queryParams = new SecretQueryParameters();
        var expected = new PaginatedResponse<SecretResponse>();

        service.GetSecretsAsync("user-1", queryParams).Returns(expected);

        var result = await service.GetSecretsAsync("user-1", queryParams);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task GetSecretAsync_AcceptsUserIdAndSecretId_ReturnsSecretResponse()
    {
        var service = Substitute.For<ISecretService>();
        var secretId = Guid.NewGuid();
        var expected = new SecretResponse { Id = secretId, Name = "my-key" };

        service.GetSecretAsync("user-1", secretId).Returns(expected);

        var result = await service.GetSecretAsync("user-1", secretId);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task CreateSecretAsync_AcceptsUserIdAndRequest_ReturnsSecretResponse()
    {
        var service = Substitute.For<ISecretService>();
        var request = new CreateSecretRequest { Name = "api-key", Value = "secret-123" };
        var expected = new SecretResponse { Id = Guid.NewGuid(), Name = "api-key", Value = "secret-123" };

        service.CreateSecretAsync("user-1", request).Returns(expected);

        var result = await service.CreateSecretAsync("user-1", request);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task UpdateSecretAsync_AcceptsUserIdSecretIdAndRequest_ReturnsSecretResponse()
    {
        var service = Substitute.For<ISecretService>();
        var secretId = Guid.NewGuid();
        var request = new UpdateSecretRequest { Name = "updated-key" };
        var expected = new SecretResponse { Id = secretId, Name = "updated-key" };

        service.UpdateSecretAsync("user-1", secretId, request).Returns(expected);

        var result = await service.UpdateSecretAsync("user-1", secretId, request);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task DeleteSecretAsync_AcceptsUserIdAndSecretId_ReturnsTask()
    {
        var service = Substitute.For<ISecretService>();
        var secretId = Guid.NewGuid();

        await service.DeleteSecretAsync("user-1", secretId);

        await service.Received(1).DeleteSecretAsync("user-1", secretId);
    }

    [Fact]
    public async Task GetAllSecretsAsync_AcceptsAdminQueryParams_ReturnsPaginatedAdminSecretResponse()
    {
        var service = Substitute.For<ISecretService>();
        var queryParams = new AdminSecretQueryParameters();
        var expected = new PaginatedResponse<AdminSecretResponse>();

        service.GetAllSecretsAsync(queryParams).Returns(expected);

        var result = await service.GetAllSecretsAsync(queryParams);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task ResetSecretAsync_AcceptsSecretId_ReturnsTask()
    {
        var service = Substitute.For<ISecretService>();
        var secretId = Guid.NewGuid();

        await service.ResetSecretAsync(secretId);

        await service.Received(1).ResetSecretAsync(secretId);
    }

    [Fact]
    public async Task AdminDeleteSecretAsync_AcceptsSecretId_ReturnsTask()
    {
        var service = Substitute.For<ISecretService>();
        var secretId = Guid.NewGuid();

        await service.AdminDeleteSecretAsync(secretId);

        await service.Received(1).AdminDeleteSecretAsync(secretId);
    }
}
