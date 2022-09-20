using Kerbero.Common.Interfaces;
using Kerbero.Common.Repositories;

namespace Kerbero.Common.Interactors;

public class ProvideNukiAuthRedirectUrlInteractor: Interactor<string, Uri>
{
	private readonly INukiExternalAuthenticationRepository _nukiExternalAuthenticationRepository;

	public ProvideNukiAuthRedirectUrlInteractor(INukiExternalAuthenticationRepository nukiExternalAuthenticationRepository)
	{
		_nukiExternalAuthenticationRepository = nukiExternalAuthenticationRepository;
	}

	public Uri Handle(string clientId)
	{
		return _nukiExternalAuthenticationRepository.BuildUriForCode(clientId);
	}
}
