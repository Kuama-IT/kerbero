using FluentResults;
using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.NukiAuthentication.Interfaces;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Domain.NukiAuthentication.Models.ExternalRequests;
using Kerbero.Domain.NukiAuthentication.Models.PresentationRequests;
using Kerbero.Domain.NukiAuthentication.Models.PresentationResponses;
using Kerbero.Domain.NukiAuthentication.Repositories;

namespace Kerbero.Domain.NukiAuthentication.Interactors;

public class ProvideNukiAuthRedirectUrlInteractor: IProvideNukiAuthRedirectUrlInteractor
{
	private readonly INukiAccountExternalRepository _nukiAccountExternalRepository;

	public ProvideNukiAuthRedirectUrlInteractor(INukiAccountExternalRepository nukiAccountExternalRepository)
	{
		_nukiAccountExternalRepository = nukiAccountExternalRepository;
	}

	public Result<NukiRedirectPresentationResponse> Handle(NukiRedirectPresentationRequest request)
	{
		return _nukiAccountExternalRepository.BuildUriForCode(new NukiRedirectExternalRequest(request.ClientId));
	}
}
