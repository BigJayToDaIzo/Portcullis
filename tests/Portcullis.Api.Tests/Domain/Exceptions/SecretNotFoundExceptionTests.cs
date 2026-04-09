namespace Portcullis.Api.Tests.Domain.Exceptions;

public class SecretNotFoundExceptionTests
{
    [Fact]
    public void SecretNotFoundException_Inherits_From_Exception()
    {
        var ex = new Portcullis.Api.Domain.Exceptions.SecretNotFoundException("not found");

        Assert.IsAssignableFrom<Exception>(ex);
    }
}
