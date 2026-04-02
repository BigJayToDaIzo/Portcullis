namespace Portcullis.Api.Domain.Entities;

public class User
{
  public string Id { get; init; } = string.Empty;
  public string DisplayName { get; set; } = "Doe, Jane";
  public string Email { get; set; } = "anon@anon.non";
  public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
  public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
