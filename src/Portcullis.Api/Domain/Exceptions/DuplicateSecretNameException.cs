namespace Portcullis.Api.Domain.Exceptions;

public class DuplicateSecretNameException(string name)
    : Exception($"A secret named '{name}' already exists.")
{
    public string Name { get; } = name;
}
