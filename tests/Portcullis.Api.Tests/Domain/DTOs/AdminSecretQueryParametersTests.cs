using Portcullis.Api.Domain.DTOs;

namespace Portcullis.Api.Tests.Domain.DTOs;

public class AdminSecretQueryParametersTests
{
    [Fact]
    public void Inherits_From_SecretQueryParameters()
    {
        var query = new AdminSecretQueryParameters();

        Assert.IsType<SecretQueryParameters>(query, exactMatch: false);
    }

    [Fact]
    public void UserName_Filter_Has_No_Default()
    {
        var query = new AdminSecretQueryParameters();

        Assert.Null(query.UserName);
    }

    [Fact]
    public void UserName_Filter_Can_Be_Set()
    {
        var query = new AdminSecretQueryParameters { UserName = "jmyers" };

        Assert.Equal("jmyers", query.UserName);
    }
}
