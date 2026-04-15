namespace Portcullis.Api.Domain.DTOs;

public record PaginatedResponse<T>
{
    public List<T> Items { get; init; } = [];
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
}
