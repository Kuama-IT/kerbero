using FluentResults;
using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Domain.NukiAuthentication.Repositories;

namespace Kerbero.Domain.NukiAuthentication.Interactors;

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
