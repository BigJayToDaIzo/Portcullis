namespace Portcullis.Api.Domain.DTOs
{
    public class SecretQueryParameters
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string SortBy { get; set; } = "CreatedAt";
        public string SortDirection { get; set; } = "desc";
        public string? Name { get; set; }
    }
}
