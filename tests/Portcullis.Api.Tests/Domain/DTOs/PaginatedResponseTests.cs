using Portcullis.Api.Domain.DTOs;

namespace Portcullis.Api.Tests.Domain.DTOs;

public class PaginatedResponseTests
{
    [Fact]
    public void Has_Items_Property()
    {
        var items = new List<string> { "one", "two" };
        var response = new PaginatedResponse<string> { Items = items };

        Assert.Equal(items, response.Items);
    }

    [Fact]
    public void Has_Page_Property()
    {
        var response = new PaginatedResponse<string> { Page = 3 };

        Assert.Equal(3, response.Page);
    }

    [Fact]
    public void Has_PageSize_Property()
    {
        var response = new PaginatedResponse<string> { PageSize = 25 };

        Assert.Equal(25, response.PageSize);
    }

    [Fact]
    public void Has_TotalCount_Property()
    {
        var response = new PaginatedResponse<string> { TotalCount = 100 };

        Assert.Equal(100, response.TotalCount);
    }

    [Fact]
    public void Has_TotalPages_Property()
    {
        var response = new PaginatedResponse<string> { TotalPages = 5 };

        Assert.Equal(5, response.TotalPages);
    }
}
