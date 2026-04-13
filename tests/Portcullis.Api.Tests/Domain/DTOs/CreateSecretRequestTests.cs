using Portcullis.Api.Domain.DTOs;
namespace Portcullis.Api.Tests.Domain.DTOs;

public class CreateSecretRequestTests
{
    [Fact]
    public void CreateSecretRequest_Has_Name_Property()
    {
        var request = new CreateSecretRequest { Name = "my-api-key" };

        Assert.Equal("my-api-key", request.Name);
    }

    [Fact]
    public void CreateSecretRequest_Has_Value_Property()
    {
        var request = new CreateSecretRequest { Value = "super-secret-value" };

        Assert.Equal("super-secret-value", request.Value);
    }
}
