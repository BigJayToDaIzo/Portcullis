namespace Portcullis.Api.Domain.DTOs;

public class CreateSecretRequest
{
  public string Name { get; set; } = "";
  public string Value { get; set; } = "";
}
