using Portcullis.Api.Domain.Exceptions;
namespace Portcullis.Api.Tests.Domain.Exceptions;

public class DuplicateSecretNameExceptionTests
{
    [Fact]
    public void DuplicateSecretNameException_Inherits_From_Exception()
    {
        var ex = new DuplicateSecretNameException("my-secret");

        Assert.IsType<Exception>(ex, exactMatch: false);
    }

    [Fact]
    public void DuplicateSecretNameException_Exposes_Name_Property()
    {
        var name = "my-secret";

        var ex = new DuplicateSecretNameException(name);

        Assert.Equal(name, ex.Name);
    }

    [Fact]
    public void DuplicateSecretNameException_Generates_Message_From_Name()
    {
        var name = "my-secret";

        var ex = new DuplicateSecretNameException(name);

        Assert.Equal($"A secret named '{name}' already exists.", ex.Message);
    }
}
