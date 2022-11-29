namespace Kerbero.Domain.Common.Errors;

public class UnsupportedSmartLockProviderError : KerberoError
{
  public UnsupportedSmartLockProviderError(string? message = "This provider is not supported") : base(message)
  {
  }
}