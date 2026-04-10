namespace Portcullis.Api.Tests.Domain.Exceptions;
using Portcullis.Api.Domain.Enums;
using Portcullis.Api.Domain.Exceptions;

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
}
