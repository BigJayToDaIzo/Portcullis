using Microsoft.EntityFrameworkCore;
using Portcullis.Api.Data;
using Portcullis.Api.Domain.Entities;
using Testcontainers.PostgreSql;

namespace Portcullis.Api.Tests.Data.Configuration;

public class SecretConfigurationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();

    public async ValueTask InitializeAsync() => await _postgres.StartAsync();
    public async ValueTask DisposeAsync() => await _postgres.DisposeAsync();

    private PortcullisDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PortcullisDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .Options;

        return new PortcullisDbContext(options);
    }

    [Fact]
    public async Task Secret_Value_IsNullable()
    {
        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

        var user = new User { Id = "user-1", DisplayName = "Test User", Email = "test@test.com" };
        context.Users.Add(user);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var secret = new Secret { UserId = "user-1", Name = "my-secret", Value = null };
        context.Secrets.Add(secret);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var saved = await context.Secrets.FindAsync([secret.Id], TestContext.Current.CancellationToken);
        Assert.NotNull(saved);
        Assert.Null(saved.Value);
    }

    [Fact]
    public async Task Secret_UserId_IsForeignKey_To_User()
    {
        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

        var secret = new Secret { UserId = "nonexistent-user", Name = "my-secret", Value = "value" };
        context.Secrets.Add(secret);

        await Assert.ThrowsAsync<DbUpdateException>(
            () => context.SaveChangesAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Secret_HasCompositeUniqueIndex_On_UserId_And_Name()
    {
        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

        var user = new User { Id = "user-1", DisplayName = "Test User", Email = "test@test.com" };
        context.Users.Add(user);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var secret1 = new Secret { UserId = "user-1", Name = "my-secret", Value = "value1" };
        var secret2 = new Secret { UserId = "user-1", Name = "my-secret", Value = "value2" };

        context.Secrets.Add(secret1);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        context.Secrets.Add(secret2);
        await Assert.ThrowsAsync<DbUpdateException>(
            () => context.SaveChangesAsync(TestContext.Current.CancellationToken));
    }
}
