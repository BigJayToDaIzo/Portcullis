using Microsoft.EntityFrameworkCore;
using Portcullis.Api.Data;
using Testcontainers.PostgreSql;

namespace Portcullis.Api.Tests.Data;

public class PortcullisDbContextTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder(
        "postgres:17-alpine"
    ).Build();

    public async ValueTask InitializeAsync() => await _postgres.StartAsync();

    public async ValueTask DisposeAsync()
    {
        await _postgres.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    private PortcullisDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PortcullisDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .Options;
        return new PortcullisDbContext(options);
    }

    [Fact]
    public async Task DbContext_HasAllDbSets()
    {
        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

        Assert.NotNull(context.Users);
        Assert.NotNull(context.Secrets);
        Assert.NotNull(context.AuditLogEntries);
    }

    [Fact]
    public async Task SaveChangesAsync_Stamps_UpdatedAt_On_Modified_Entity()
    {
        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

        var user = new Api.Domain.Entities.User
        {
            Id = "user-1",
            DisplayName = "Test User",
            Email = "test@test.com",
        };
        context.Users.Add(user);

        var secret = new Api.Domain.Entities.Secret
        {
            UserId = "user-1",
            Name = "my-secret",
            Value = "original",
        };
        context.Secrets.Add(secret);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var originalUpdatedAt = secret.UpdatedAt;

        secret.Value = "modified";
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        Assert.True(secret.UpdatedAt > originalUpdatedAt);
    }
}
