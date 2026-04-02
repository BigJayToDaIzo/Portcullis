namespace Portcullis.Api.Domain.Entities;

using Portcullis.Api.Domain.Enums;

public class AuditLogEntry
{
  public Guid Id { get; init; } = Guid.NewGuid();
  public string UserId { get; init; } = string.Empty;
  public string? TargetUserId { get; init; }
  public Guid? SecretId { get; init; }
  public AuditAction Action { get; set; } = AuditAction.Create;
  public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
  public string Description { get; set; } = string.Empty;
}
