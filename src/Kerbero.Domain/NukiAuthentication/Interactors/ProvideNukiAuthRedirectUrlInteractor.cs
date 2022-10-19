using FluentResults;
using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.NukiAuthentication.Interfaces;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Domain.NukiAuthentication.Repositories;

namespace Kerbero.Domain.NukiAuthentication.Interactors;

public class ProvideNukiAuthRedirectUrlInteractor: IProvideNukiAuthRedirectUrlInteractor
{
	private readonly INukiAccountExternalRepository _nukiAccountExternalRepository;

	public ProvideNukiAuthRedirectUrlInteractor(INukiAccountExternalRepository nukiAccountExternalRepository)
	{
		_nukiAccountExternalRepository = nukiAccountExternalRepository;
	}

	public Result<NukiRedirectPresentationDto> Handle(NukiRedirectExternalRequestDto request)
	{
		return _nukiAccountExternalRepository.BuildUriForCode(request);
	}
}
