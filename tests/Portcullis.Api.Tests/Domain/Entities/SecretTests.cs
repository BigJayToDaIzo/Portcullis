namespace Portcullis.Api.Tests.Domain.Entities;

public class SecretTests
{
    [Fact]
    public void Secret_Has_Id_Property_Of_Type_Guid()
    {
        var property = typeof(Portcullis.Api.Domain.Entities.Secret).GetProperty("Id");

        Assert.NotNull(property);
        Assert.Equal(typeof(Guid), property.PropertyType);
    }

    [Fact]
    public void Secret_Has_UserId_Property_Of_Type_String()
    {
        var property = typeof(Portcullis.Api.Domain.Entities.Secret).GetProperty("UserId");

        Assert.NotNull(property);
        Assert.Equal(typeof(string), property.PropertyType);
    }

    [Fact]
    public void Secret_Has_Name_Property_Of_Type_String()
    {
        var property = typeof(Portcullis.Api.Domain.Entities.Secret).GetProperty("Name");

        Assert.NotNull(property);
        Assert.Equal(typeof(string), property.PropertyType);
    }

    [Fact]
    public void Secret_Has_Value_Property_Of_Type_Nullable_String()
    {
        var property = typeof(Portcullis.Api.Domain.Entities.Secret).GetProperty("Value");

        Assert.NotNull(property);
        Assert.Equal(typeof(string), property.PropertyType);
    }

    [Fact]
    public void Secret_Has_CreatedAt_Property_Of_Type_DateTimeOffset()
    {
        var property = typeof(Portcullis.Api.Domain.Entities.Secret).GetProperty("CreatedAt");

        Assert.NotNull(property);
        Assert.Equal(typeof(DateTimeOffset), property.PropertyType);
    }

    [Fact]
    public void Secret_Has_UpdatedAt_Property_Of_Type_DateTimeOffset()
    {
        var property = typeof(Portcullis.Api.Domain.Entities.Secret).GetProperty("UpdatedAt");

        Assert.NotNull(property);
        Assert.Equal(typeof(DateTimeOffset), property.PropertyType);
    }
}
