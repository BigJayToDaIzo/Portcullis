using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Portcullis.Api.Data;
using Portcullis.Api.Domain.DTOs;
using Portcullis.Api.Domain.Entities;
using Portcullis.Api.Domain.Exceptions;
using Portcullis.Api.Domain.Services;

namespace Portcullis.Api.Tests.Domain.Services;

public class SecretServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly PortcullisDbContext _dbContext;
    private readonly SecretService _service;

    public SecretServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<PortcullisDbContext>()
            .UseSqlite(_connection)
            .Options;

        _dbContext = new PortcullisDbContext(options);
        _dbContext.Database.EnsureCreated();

        _service = new SecretService(_dbContext);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public async Task CreateSecretAsync_WithValidRequest_CreatesAndReturnsSecretResponse()
    {
        var ct = TestContext.Current.CancellationToken;

        _dbContext.Users.Add(new User { Id = "user-1", DisplayName = "Test User", Email = "test@test.com" });
        await _dbContext.SaveChangesAsync(ct);

        var request = new CreateSecretRequest { Name = "api-key", Value = "secret-123" };

        var result = await _service.CreateSecretAsync("user-1", request);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("api-key", result.Name);
        Assert.Equal("secret-123", result.Value);

        var saved = await _dbContext.Secrets.FirstOrDefaultAsync(s => s.Id == result.Id, ct);
        Assert.NotNull(saved);
        Assert.Equal("user-1", saved.UserId);
        Assert.Equal("api-key", saved.Name);
        Assert.Equal("secret-123", saved.Value);
    }

    [Fact]
    public async Task CreateSecretAsync_WithDuplicateName_ThrowsDuplicateSecretNameException()
    {
        var ct = TestContext.Current.CancellationToken;

        _dbContext.Users.Add(new User { Id = "user-1", DisplayName = "Test User", Email = "test@test.com" });
        _dbContext.Secrets.Add(new Secret { UserId = "user-1", Name = "api-key", Value = "existing" });
        await _dbContext.SaveChangesAsync(ct);

        var request = new CreateSecretRequest { Name = "api-key", Value = "new-value" };

        var ex = await Assert.ThrowsAsync<DuplicateSecretNameException>(
            () => _service.CreateSecretAsync("user-1", request)
        );

        Assert.Equal("api-key", ex.Name);
    }
}
