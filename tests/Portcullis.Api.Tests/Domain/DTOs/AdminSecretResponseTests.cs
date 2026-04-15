using Portcullis.Api.Domain.DTOs;

namespace Portcullis.Api.Tests.Domain.DTOs;

public class AdminSecretResponseTests
{
    [Fact]
    public void AdminSecretResponse_Has_Common_Properties()
    {
        var id = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var later = now.AddMinutes(5);

        var response = new AdminSecretResponse
        {
            Id = id,
            Name = "my-api-key",
            Value = "super-secret-value-123",
            CreatedAt = now,
            UpdatedAt = later,
        };

        Assert.Equal(id, response.Id);
        Assert.Equal("my-api-key", response.Name);
        Assert.Equal("super-secret-value-123", response.Value);
        Assert.Equal(now, response.CreatedAt);
        Assert.Equal(later, response.UpdatedAt);
    }

    [Fact]
    public void AdminSecretResponse_Has_UserId_Property()
    {
        var userId = Guid.NewGuid();
        var response = new AdminSecretResponse { UserId = userId };

        Assert.Equal(userId, response.UserId);
    }

    [Fact]
    public void AdminSecretResponse_Has_UserName_Property()
    {
        var response = new AdminSecretResponse { UserName = "jmyers" };

        Assert.Equal("jmyers", response.UserName);
    }
}
