namespace Portcullis.Api.Tests.Domain.Enums;

public class AuditActionTests
{
    [Fact]
    public void AuditAction_Has_Exactly_Five_Values()
    {
        var values = Enum.GetValues<Portcullis.Api.Domain.Enums.AuditAction>();

        Assert.Equal(5, values.Length);
    }

    [Fact]
    public void AuditAction_Contains_Expected_Values()
    {
        var names = Enum.GetNames<Portcullis.Api.Domain.Enums.AuditAction>();

        Assert.Contains("Create", names);
        Assert.Contains("Read", names);
        Assert.Contains("Update", names);
        Assert.Contains("Delete", names);
        Assert.Contains("Reset", names);
    }
}
