using Portcullis.Api.Domain.DTOs;

namespace Portcullis.Api.Tests.Domain.DTOs;
public class UpdateSecretRequestTests
{
    [Fact]
    public void UpdateSecretRequest_Has_Name_Property()
    {
        var request = new UpdateSecretRequest { Name = "renamed-secret" };

        Assert.Equal("renamed-secret", request.Name);
    }

    [Fact]
    public void UpdateSecretRequest_Has_Value_Property()
    {
        var request = new UpdateSecretRequest { Value = "new-secret-value" };

        Assert.Equal("new-secret-value", request.Value);
    }
}
