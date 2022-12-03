using FluentResults;

namespace Kerbero.Domain.NukiCredentials.Interfaces;

public interface IBuildNukiRedirectUriInteractor
{
  Task<Result<Uri>> Handle();
}