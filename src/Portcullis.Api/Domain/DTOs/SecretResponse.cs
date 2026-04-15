namespace Portcullis.Api.Domain.DTOs;

public record SecretResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public string Value { get; init; } = "";
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;
}
