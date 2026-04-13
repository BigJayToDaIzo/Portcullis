using Portcullis.Api.Domain.Enums;
namespace Portcullis.Api.Domain.Exceptions;

public class NotAuthorizedException(Guid guid, AuditAction action) : Exception("User is not authorized to perform this action.")
{
  public Guid UserId { get; } = guid;
  public AuditAction Action { get; } = action;
}
