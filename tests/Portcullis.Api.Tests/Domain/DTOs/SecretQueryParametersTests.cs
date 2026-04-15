using Portcullis.Api.Domain.DTOs;

namespace Portcullis.Api.Tests.Domain.DTOs;

public class SecretQueryParametersTests
{
    [Fact]
    public void Defaults_Are_Set()
    {
        var query = new SecretQueryParameters();

        Assert.Equal(1, query.Page);
        Assert.Equal(20, query.PageSize);
        Assert.Equal("CreatedAt", query.SortBy);
        Assert.Equal("desc", query.SortDirection);
    }

    [Fact]
    public void Name_Filter_Has_No_Default()
    {
        var query = new SecretQueryParameters();

        Assert.Null(query.Name);
    }

    [Fact]
    public void Name_Filter_Can_Be_Set()
    {
        var query = new SecretQueryParameters { Name = "api-key" };

        Assert.Equal("api-key", query.Name);
    }
}
