namespace Portcullis.Api.Domain.DTOs;

public record AdminSecretResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public string Value { get; init; } = "";
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;
    public Guid UserId { get; init; }
    public string UserName { get; init; } = "";
}
