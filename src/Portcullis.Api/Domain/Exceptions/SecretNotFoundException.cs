namespace Portcullis.Api.Domain.Exceptions;

public class SecretNotFoundException(string message) : Exception(message)
{
}
