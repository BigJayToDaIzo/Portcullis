using Portcullis.Api.Domain.DTOs;

namespace Portcullis.Api.Tests.Domain.DTOs;

public class SecretResponseTests
{
    [Fact]
    public void SecretResponse_Has_Id_Property()
    {
        var id = Guid.NewGuid();
        var response = new SecretResponse { Id = id };

        Assert.Equal(id, response.Id);
    }

    [Fact]
    public void SecretResponse_Has_Name_Property()
    {
        var response = new SecretResponse { Name = "my-api-key" };

        Assert.Equal("my-api-key", response.Name);
    }

    [Fact]
    public void SecretResponse_Has_Value_Property()
    {
        var response = new SecretResponse { Value = "super-secret-value-123" };

        Assert.Equal("super-secret-value-123", response.Value);
    }

    [Fact]
    public void SecretResponse_Has_CreatedAt_Property()
    {
        var now = DateTime.UtcNow;
        var response = new SecretResponse { CreatedAt = now };

        Assert.Equal(now, response.CreatedAt);
    }

    [Fact]
    public void SecretResponse_Has_UpdatedAt_Property()
    {
        var now = DateTime.UtcNow;
        var response = new SecretResponse { UpdatedAt = now };

        Assert.Equal(now, response.UpdatedAt);
    }
}
