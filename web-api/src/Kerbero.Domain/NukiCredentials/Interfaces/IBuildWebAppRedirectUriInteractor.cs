using FluentResults;

namespace Kerbero.Domain.NukiCredentials.Interfaces;

public interface IBuildWebAppRedirectUriInteractor
{
	public Task<Result<Uri>> Handle(bool isSuccessUri);
}
