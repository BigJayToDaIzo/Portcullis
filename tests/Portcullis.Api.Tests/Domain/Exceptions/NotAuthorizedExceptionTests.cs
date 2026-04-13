using Portcullis.Api.Domain.Enums;
using Portcullis.Api.Domain.Exceptions;
namespace Portcullis.Api.Tests.Domain.Exceptions;

public class NotAuthorizedExceptionTests
{
    [Fact]
    public void NotAuthorizedException_Inherits_From_Exception()
    {
        var ex = new NotAuthorizedException(Guid.NewGuid(), AuditAction.Read);

        Assert.IsType<Exception>(ex, exactMatch: false);
    }

    [Fact]
    public void NotAuthorizedException_Exposes_UserId_Property()
    {
        var userId = Guid.NewGuid();

        var ex = new NotAuthorizedException(userId, AuditAction.Delete);

        Assert.Equal(userId, ex.UserId);
    }

    [Fact]
    public void NotAuthorizedException_Exposes_Action_Property()
    {
        var action = AuditAction.Reset;

        var ex = new NotAuthorizedException(Guid.NewGuid(), action);

        Assert.Equal(action, ex.Action);
    }

    [Fact]
    public void NotAuthorizedException_Generates_Generic_Message()
    {
        var ex = new NotAuthorizedException(Guid.NewGuid(), AuditAction.Delete);

        Assert.Equal("User is not authorized to perform this action.", ex.Message);
    }
}
