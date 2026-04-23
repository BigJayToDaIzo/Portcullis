namespace Portcullis.Api.Domain.Exceptions;

public class SecretNotFoundException(Guid guid) : Exception($"Secret '{guid}' was not found.")
{
    public Guid SecretId { get; } = guid;
}
