namespace Portcullis.Api.Tests.Domain.Entities;

public class AuditLogEntryTests
{
    [Fact]
    public void AuditLogEntry_Has_Id_Property_Of_Type_Guid()
    {
        var property = typeof(Portcullis.Api.Domain.Entities.AuditLogEntry).GetProperty("Id");

        Assert.NotNull(property);
        Assert.Equal(typeof(Guid), property.PropertyType);
    }

    [Fact]
    public void AuditLogEntry_Has_UserId_Property_Of_Type_String()
    {
        var property = typeof(Portcullis.Api.Domain.Entities.AuditLogEntry).GetProperty("UserId");

        Assert.NotNull(property);
        Assert.Equal(typeof(string), property.PropertyType);
    }

    [Fact]
    public void AuditLogEntry_Has_TargetUserId_Property_Of_Type_String()
    {
        var property = typeof(Portcullis.Api.Domain.Entities.AuditLogEntry).GetProperty("TargetUserId");

        Assert.NotNull(property);
        Assert.Equal(typeof(string), property.PropertyType);
    }

    [Fact]
    public void AuditLogEntry_Has_SecretId_Property_Of_Type_Nullable_Guid()
    {
        var property = typeof(Portcullis.Api.Domain.Entities.AuditLogEntry).GetProperty("SecretId");

        Assert.NotNull(property);
        Assert.Equal(typeof(Guid?), property.PropertyType);
    }

    [Fact]
    public void AuditLogEntry_Has_Action_Property_Of_Type_AuditAction()
    {
        var property = typeof(Portcullis.Api.Domain.Entities.AuditLogEntry).GetProperty("Action");

        Assert.NotNull(property);
        Assert.Equal(typeof(Portcullis.Api.Domain.Enums.AuditAction), property.PropertyType);
    }

    [Fact]
    public void AuditLogEntry_Has_Timestamp_Property_Of_Type_DateTimeOffset()
    {
        var property = typeof(Portcullis.Api.Domain.Entities.AuditLogEntry).GetProperty("Timestamp");

        Assert.NotNull(property);
        Assert.Equal(typeof(DateTimeOffset), property.PropertyType);
    }

    [Fact]
    public void AuditLogEntry_Has_Description_Property_Of_Type_String()
    {
        var property = typeof(Portcullis.Api.Domain.Entities.AuditLogEntry).GetProperty("Description");

        Assert.NotNull(property);
        Assert.Equal(typeof(string), property.PropertyType);
    }
}
