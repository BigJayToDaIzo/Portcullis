using Microsoft.EntityFrameworkCore;
using Portcullis.Api.Data;
using Portcullis.Api.Domain.Entities;
using Portcullis.Api.Domain.Enums;
using Testcontainers.PostgreSql;

namespace Portcullis.Api.Tests.Data.Configuration;

public class AuditLogEntryConfigurationTests : IAsyncLifetime
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
    public async Task AuditLogEntry_SecretId_HasNoForeignKey()
    {
        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

        var user = new User { Id = "user-1", DisplayName = "Test User", Email = "test@test.com" };
        context.Users.Add(user);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var entry = new AuditLogEntry
        {
            UserId = "user-1",
            SecretId = Guid.NewGuid(),
            Action = AuditAction.Delete,
            Description = "Deleted a secret that no longer exists"
        };
        context.AuditLogEntries.Add(entry);

        var saved = await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        Assert.Equal(1, saved);
    }
}
