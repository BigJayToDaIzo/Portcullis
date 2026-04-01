namespace Portcullis.Api.Domain.Entities;

public class Secret {
  public Guid Id { get; init; } = Guid.NewGuid();
  public string UserId { get; init; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string? Value { get; set; }
  public DateTimeOffset CreatedAt { get; init;} = DateTimeOffset.UtcNow;
  public DateTimeOffset UpdatedAt { get; set;} = DateTimeOffset.UtcNow;
}
