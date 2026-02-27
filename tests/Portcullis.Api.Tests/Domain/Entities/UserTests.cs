namespace Portcullis.Api.Tests.Domain.Entities;

public class UserTests
{
    [Fact]
    public void User_Has_Id_Property_Of_Type_String()
    {
        var property = typeof(Portcullis.Api.Domain.Entities.User).GetProperty("Id");

        Assert.NotNull(property);
        Assert.Equal(typeof(string), property.PropertyType);
    }

    [Fact]
    public void User_Has_DisplayName_Property_Of_Type_String()
    {
        var property = typeof(Portcullis.Api.Domain.Entities.User).GetProperty("DisplayName");

        Assert.NotNull(property);
        Assert.Equal(typeof(string), property.PropertyType);
    }

    [Fact]
    public void User_Has_Email_Property_Of_Type_String()
    {
        var property = typeof(Portcullis.Api.Domain.Entities.User).GetProperty("Email");

        Assert.NotNull(property);
        Assert.Equal(typeof(string), property.PropertyType);
    }

    [Fact]
    public void User_Has_CreatedAt_Property_Of_Type_DateTimeOffset()
    {
        var property = typeof(Portcullis.Api.Domain.Entities.User).GetProperty("CreatedAt");

        Assert.NotNull(property);
        Assert.Equal(typeof(DateTimeOffset), property.PropertyType);
    }

    [Fact]
    public void User_Has_UpdatedAt_Property_Of_Type_DateTimeOffset()
    {
        var property = typeof(Portcullis.Api.Domain.Entities.User).GetProperty("UpdatedAt");

        Assert.NotNull(property);
        Assert.Equal(typeof(DateTimeOffset), property.PropertyType);
    }
}
