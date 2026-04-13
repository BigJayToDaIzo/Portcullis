using Portcullis.Api.Domain.Exceptions;
namespace Portcullis.Api.Tests.Domain.Exceptions;

public class SecretNotFoundExceptionTests
{
    [Fact]
    public void SecretNotFoundException_Inherits_From_Exception()
    {
        var ex = new SecretNotFoundException(Guid.NewGuid());

        Assert.IsType<Exception>(ex, exactMatch: false);
    }

    [Fact]
    public void SecretNotFoundException_Exposes_SecretId_Property()
    {
        var secretId = Guid.NewGuid();

        var ex = new SecretNotFoundException(secretId);

        Assert.Equal(secretId, ex.SecretId);
    }

    [Fact]
    public void SecretNotFoundException_Generates_Message_From_SecretId()
    {
        var secretId = Guid.NewGuid();

        var ex = new SecretNotFoundException(secretId);

        Assert.Equal($"Secret '{secretId}' was not found.", ex.Message);
    }
}
