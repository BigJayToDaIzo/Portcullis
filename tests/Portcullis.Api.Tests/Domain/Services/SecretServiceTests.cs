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

        _dbContext.Users.Add(
            new User
            {
                Id = "user-1",
                DisplayName = "Test User",
                Email = "test@test.com",
            }
        );
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

        _dbContext.Users.Add(
            new User
            {
                Id = "user-1",
                DisplayName = "Test User",
                Email = "test@test.com",
            }
        );
        _dbContext.Secrets.Add(
            new Secret
            {
                UserId = "user-1",
                Name = "api-key",
                Value = "existing",
            }
        );
        await _dbContext.SaveChangesAsync(ct);

        var request = new CreateSecretRequest { Name = "api-key", Value = "new-value" };

        var ex = await Assert.ThrowsAsync<DuplicateSecretNameException>(() =>
            _service.CreateSecretAsync("user-1", request)
        );

        Assert.Equal("api-key", ex.Name);
    }

    [Fact]
    public async Task GetSecretAsync_WithOwnerAndExistingSecret_ReturnsSecretResponse()
    {
        var ct = TestContext.Current.CancellationToken;

        _dbContext.Users.Add(
            new User
            {
                Id = "user-1",
                DisplayName = "Test User",
                Email = "test@test.com",
            }
        );
        var secret = new Secret
        {
            UserId = "user-1",
            Name = "api-key",
            Value = "secret-123",
        };
        _dbContext.Secrets.Add(secret);
        await _dbContext.SaveChangesAsync(ct);

        var result = await _service.GetSecretAsync("user-1", secret.Id);

        Assert.Equal(secret.Id, result.Id);
        Assert.Equal("api-key", result.Name);
        Assert.Equal("secret-123", result.Value);
        Assert.Equal(secret.CreatedAt.DateTime, result.CreatedAt);
        Assert.Equal(secret.UpdatedAt.DateTime, result.UpdatedAt);
    }

    [Fact]
    public async Task GetSecretAsync_WithNonExistentSecret_ThrowsSecretNotFoundException()
    {
        var ct = TestContext.Current.CancellationToken;

        _dbContext.Users.Add(
            new User
            {
                Id = "user-1",
                DisplayName = "Test User",
                Email = "test@test.com",
            }
        );
        await _dbContext.SaveChangesAsync(ct);

        var missingId = Guid.NewGuid();

        var ex = await Assert.ThrowsAsync<SecretNotFoundException>(() =>
            _service.GetSecretAsync("user-1", missingId)
        );

        Assert.Equal(missingId, ex.SecretId);
    }

    [Fact]
    public async Task GetSecretAsync_WhenSecretBelongsToDifferentUser_ThrowsSecretNotFoundException()
    {
        var ct = TestContext.Current.CancellationToken;

        _dbContext.Users.Add(
            new User
            {
                Id = "user-1",
                DisplayName = "User One",
                Email = "one@test.com",
            }
        );
        _dbContext.Users.Add(
            new User
            {
                Id = "user-2",
                DisplayName = "User Two",
                Email = "two@test.com",
            }
        );
        var secret = new Secret
        {
            UserId = "user-2",
            Name = "api-key",
            Value = "secret-123",
        };
        _dbContext.Secrets.Add(secret);
        await _dbContext.SaveChangesAsync(ct);

        var ex = await Assert.ThrowsAsync<SecretNotFoundException>(() =>
            _service.GetSecretAsync("user-1", secret.Id)
        );

        Assert.Equal(secret.Id, ex.SecretId);
    }

    [Fact]
    public async Task DeleteSecretAsync_WithOwnerAndExistingSecret_RemovesSecret()
    {
        var ct = TestContext.Current.CancellationToken;

        _dbContext.Users.Add(
            new User
            {
                Id = "user-1",
                DisplayName = "Test User",
                Email = "test@test.com",
            }
        );
        var secret = new Secret
        {
            UserId = "user-1",
            Name = "api-key",
            Value = "secret-123",
        };
        _dbContext.Secrets.Add(secret);
        await _dbContext.SaveChangesAsync(ct);

        await _service.DeleteSecretAsync("user-1", secret.Id);

        var saved = await _dbContext.Secrets.FirstOrDefaultAsync(s => s.Id == secret.Id, ct);
        Assert.Null(saved);
    }

    [Fact]
    public async Task DeleteSecretAsync_WithNonExistentSecret_ThrowsSecretNotFoundException()
    {
        var ct = TestContext.Current.CancellationToken;

        _dbContext.Users.Add(
            new User
            {
                Id = "user-1",
                DisplayName = "Test User",
                Email = "test@test.com",
            }
        );
        await _dbContext.SaveChangesAsync(ct);

        var missingId = Guid.NewGuid();

        var ex = await Assert.ThrowsAsync<SecretNotFoundException>(() =>
            _service.DeleteSecretAsync("user-1", missingId)
        );

        Assert.Equal(missingId, ex.SecretId);
    }

    [Fact]
    public async Task DeleteSecretAsync_WhenSecretBelongsToDifferentUser_ThrowsAndLeavesSecretIntact()
    {
        var ct = TestContext.Current.CancellationToken;

        _dbContext.Users.Add(
            new User
            {
                Id = "user-1",
                DisplayName = "User One",
                Email = "one@test.com",
            }
        );
        _dbContext.Users.Add(
            new User
            {
                Id = "user-2",
                DisplayName = "User Two",
                Email = "two@test.com",
            }
        );
        var secret = new Secret
        {
            UserId = "user-2",
            Name = "api-key",
            Value = "secret-123",
        };
        _dbContext.Secrets.Add(secret);
        await _dbContext.SaveChangesAsync(ct);

        var ex = await Assert.ThrowsAsync<SecretNotFoundException>(() =>
            _service.DeleteSecretAsync("user-1", secret.Id)
        );

        Assert.Equal(secret.Id, ex.SecretId);

        var stillThere = await _dbContext.Secrets.FirstOrDefaultAsync(s => s.Id == secret.Id, ct);
        Assert.NotNull(stillThere);
    }

    [Fact]
    public async Task RotateSecretAsync_WithOwnerAndExistingSecret_UpdatesValueAndReturnsResponse()
    {
        var ct = TestContext.Current.CancellationToken;

        _dbContext.Users.Add(
            new User
            {
                Id = "user-1",
                DisplayName = "Test User",
                Email = "test@test.com",
            }
        );
        var secret = new Secret
        {
            UserId = "user-1",
            Name = "api-key",
            Value = "old-value",
        };
        _dbContext.Secrets.Add(secret);
        await _dbContext.SaveChangesAsync(ct);

        var request = new RotateSecretRequest { Value = "new-value" };

        var result = await _service.RotateSecretAsync("user-1", secret.Id, request);

        Assert.Equal(secret.Id, result.Id);
        Assert.Equal("api-key", result.Name);
        Assert.Equal("new-value", result.Value);

        var saved = await _dbContext.Secrets.FirstOrDefaultAsync(s => s.Id == secret.Id, ct);
        Assert.NotNull(saved);
        Assert.Equal("api-key", saved.Name);
        Assert.Equal("new-value", saved.Value);
    }

    [Fact]
    public async Task RenameSecretAsync_WithOwnerAndExistingSecret_UpdatesNameAndReturnsResponse()
    {
        var ct = TestContext.Current.CancellationToken;

        _dbContext.Users.Add(
            new User
            {
                Id = "user-1",
                DisplayName = "Test User",
                Email = "test@test.com",
            }
        );
        var secret = new Secret
        {
            UserId = "user-1",
            Name = "old-name",
            Value = "secret-123",
        };
        _dbContext.Secrets.Add(secret);
        await _dbContext.SaveChangesAsync(ct);

        var request = new RenameSecretRequest { Name = "new-name" };

        var result = await _service.RenameSecretAsync("user-1", secret.Id, request);

        Assert.Equal(secret.Id, result.Id);
        Assert.Equal("new-name", result.Name);
        Assert.Equal("secret-123", result.Value);

        var saved = await _dbContext.Secrets.FirstOrDefaultAsync(s => s.Id == secret.Id, ct);
        Assert.NotNull(saved);
        Assert.Equal("new-name", saved.Name);
        Assert.Equal("secret-123", saved.Value);
    }

    [Fact]
    public async Task RenameSecretAsync_WithNonExistentSecret_ThrowsSecretNotFoundException()
    {
        var ct = TestContext.Current.CancellationToken;

        _dbContext.Users.Add(
            new User
            {
                Id = "user-1",
                DisplayName = "Test User",
                Email = "test@test.com",
            }
        );
        await _dbContext.SaveChangesAsync(ct);

        var missingId = Guid.NewGuid();
        var request = new RenameSecretRequest { Name = "new-name" };

        var ex = await Assert.ThrowsAsync<SecretNotFoundException>(() =>
            _service.RenameSecretAsync("user-1", missingId, request)
        );

        Assert.Equal(missingId, ex.SecretId);
    }

    [Fact]
    public async Task RenameSecretAsync_WhenSecretBelongsToDifferentUser_ThrowsAndLeavesSecretIntact()
    {
        var ct = TestContext.Current.CancellationToken;

        _dbContext.Users.Add(
            new User
            {
                Id = "user-1",
                DisplayName = "User One",
                Email = "one@test.com",
            }
        );
        _dbContext.Users.Add(
            new User
            {
                Id = "user-2",
                DisplayName = "User Two",
                Email = "two@test.com",
            }
        );
        var secret = new Secret
        {
            UserId = "user-2",
            Name = "victim-name",
            Value = "secret-123",
        };
        _dbContext.Secrets.Add(secret);
        await _dbContext.SaveChangesAsync(ct);

        var request = new RenameSecretRequest { Name = "attacker-rename" };

        var ex = await Assert.ThrowsAsync<SecretNotFoundException>(() =>
            _service.RenameSecretAsync("user-1", secret.Id, request)
        );

        Assert.Equal(secret.Id, ex.SecretId);

        var stillThere = await _dbContext.Secrets.FirstOrDefaultAsync(s => s.Id == secret.Id, ct);
        Assert.NotNull(stillThere);
        Assert.Equal("victim-name", stillThere.Name);
    }

    [Fact]
    public async Task RenameSecretAsync_WhenNewNameCollidesWithOwnersExistingSecret_ThrowsDuplicateSecretNameException()
    {
        var ct = TestContext.Current.CancellationToken;

        _dbContext.Users.Add(
            new User
            {
                Id = "user-1",
                DisplayName = "Test User",
                Email = "test@test.com",
            }
        );
        _dbContext.Secrets.Add(
            new Secret
            {
                UserId = "user-1",
                Name = "alpha",
                Value = "alpha-value",
            }
        );
        var target = new Secret
        {
            UserId = "user-1",
            Name = "beta",
            Value = "beta-value",
        };
        _dbContext.Secrets.Add(target);
        await _dbContext.SaveChangesAsync(ct);

        var request = new RenameSecretRequest { Name = "alpha" };

        var ex = await Assert.ThrowsAsync<DuplicateSecretNameException>(() =>
            _service.RenameSecretAsync("user-1", target.Id, request)
        );

        Assert.Equal("alpha", ex.Name);
    }

    [Fact]
    public async Task RenameSecretAsync_WhenNewNameMatchesCurrentName_SucceedsAsNoOp()
    {
        var ct = TestContext.Current.CancellationToken;

        _dbContext.Users.Add(
            new User
            {
                Id = "user-1",
                DisplayName = "Test User",
                Email = "test@test.com",
            }
        );
        var secret = new Secret
        {
            UserId = "user-1",
            Name = "foo",
            Value = "secret-123",
        };
        _dbContext.Secrets.Add(secret);
        await _dbContext.SaveChangesAsync(ct);

        var request = new RenameSecretRequest { Name = "foo" };

        var result = await _service.RenameSecretAsync("user-1", secret.Id, request);

        Assert.Equal(secret.Id, result.Id);
        Assert.Equal("foo", result.Name);
        Assert.Equal("secret-123", result.Value);

        var saved = await _dbContext.Secrets.FirstOrDefaultAsync(s => s.Id == secret.Id, ct);
        Assert.NotNull(saved);
        Assert.Equal("foo", saved.Name);
    }

    [Fact]
    public async Task ResetSecretAsync_WithExistingSecret_NullsValueAndLeavesOtherFieldsIntact()
    {
        var ct = TestContext.Current.CancellationToken;

        _dbContext.Users.Add(
            new User
            {
                Id = "user-1",
                DisplayName = "Test User",
                Email = "test@test.com",
            }
        );
        var secret = new Secret
        {
            UserId = "user-1",
            Name = "api-key",
            Value = "secret-123",
        };
        _dbContext.Secrets.Add(secret);
        await _dbContext.SaveChangesAsync(ct);

        await _service.ResetSecretAsync(secret.Id);

        var saved = await _dbContext.Secrets.FirstOrDefaultAsync(s => s.Id == secret.Id, ct);
        Assert.NotNull(saved);
        Assert.Null(saved.Value);
        Assert.Equal("api-key", saved.Name);
        Assert.Equal("user-1", saved.UserId);
    }

    [Fact]
    public async Task ResetSecretAsync_WithNonExistentSecret_ThrowsSecretNotFoundException()
    {
        var missingId = Guid.NewGuid();

        var ex = await Assert.ThrowsAsync<SecretNotFoundException>(() =>
            _service.ResetSecretAsync(missingId)
        );

        Assert.Equal(missingId, ex.SecretId);
    }

    [Fact]
    public async Task ResetSecretAsync_WhenValueIsAlreadyNull_SucceedsAsNoOp()
    {
        var ct = TestContext.Current.CancellationToken;

        _dbContext.Users.Add(
            new User
            {
                Id = "user-1",
                DisplayName = "Test User",
                Email = "test@test.com",
            }
        );
        var secret = new Secret
        {
            UserId = "user-1",
            Name = "api-key",
            Value = null,
        };
        _dbContext.Secrets.Add(secret);
        await _dbContext.SaveChangesAsync(ct);

        await _service.ResetSecretAsync(secret.Id);

        var saved = await _dbContext.Secrets.FirstOrDefaultAsync(s => s.Id == secret.Id, ct);
        Assert.NotNull(saved);
        Assert.Null(saved.Value);
        Assert.Equal("api-key", saved.Name);
    }

    [Fact]
    public async Task AdminDeleteSecretAsync_WithExistingSecret_RemovesSecret()
    {
        var ct = TestContext.Current.CancellationToken;

        _dbContext.Users.Add(
            new User
            {
                Id = "user-1",
                DisplayName = "Test User",
                Email = "test@test.com",
            }
        );
        var secret = new Secret
        {
            UserId = "user-1",
            Name = "api-key",
            Value = "secret-123",
        };
        _dbContext.Secrets.Add(secret);
        await _dbContext.SaveChangesAsync(ct);

        await _service.AdminDeleteSecretAsync(secret.Id);

        var saved = await _dbContext.Secrets.FirstOrDefaultAsync(s => s.Id == secret.Id, ct);
        Assert.Null(saved);
    }

    [Fact]
    public async Task AdminDeleteSecretAsync_WithNonExistentSecret_ThrowsSecretNotFoundException()
    {
        var missingId = Guid.NewGuid();

        var ex = await Assert.ThrowsAsync<SecretNotFoundException>(() =>
            _service.AdminDeleteSecretAsync(missingId)
        );

        Assert.Equal(missingId, ex.SecretId);
    }
}
