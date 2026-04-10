namespace Portcullis.Api.Domain.Exceptions;
using Portcullis.Api.Domain.Enums;
public class NotAuthorizedException(Guid guid, AuditAction action) : Exception()
{
  public Guid UserId { get; } = guid;
  public AuditAction Action { get; } = action;

}
