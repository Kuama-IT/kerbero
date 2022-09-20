using FluentResults;
using Kerbero.Common.Interfaces;
using Kerbero.Common.Models;
using Kerbero.Common.Repositories;

namespace Kerbero.Common.Interactors;

public class ProvideNukiAuthRedirectUrlInteractor: Interactor<NukiRedirectExternalRequestDto, NukiRedirectPresentationDto>
{
	private readonly INukiExternalAuthenticationRepository _nukiExternalAuthenticationRepository;

	public ProvideNukiAuthRedirectUrlInteractor(INukiExternalAuthenticationRepository nukiExternalAuthenticationRepository)
	{
		_nukiExternalAuthenticationRepository = nukiExternalAuthenticationRepository;
	}

	public Result<NukiRedirectPresentationDto> Handle(NukiRedirectExternalRequestDto request)
	{
		return _nukiExternalAuthenticationRepository.BuildUriForCode(request);
	}
}
