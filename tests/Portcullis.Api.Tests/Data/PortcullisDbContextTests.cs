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
}
