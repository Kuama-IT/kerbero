using FluentResults;
using Kerbero.Domain.NukiAuthentication.Entities;
using Kerbero.Domain.NukiAuthentication.Interfaces;
using Kerbero.Domain.NukiAuthentication.Models.ExternalRequests;
using Kerbero.Domain.NukiAuthentication.Models.PresentationRequests;
using Kerbero.Domain.NukiAuthentication.Models.PresentationResponses;
using Kerbero.Domain.NukiAuthentication.Repositories;

namespace Kerbero.Domain.NukiAuthentication.Interactors;

public class CreateNukiAccountAndRedirectToNukiInteractor: ICreateNukiAccountAndRedirectToNukiInteractor
{
	private readonly INukiAccountExternalRepository _nukiAccountExternalRepository;
	private readonly INukiAccountPersistentRepository _nukiAccountPersistentRepository;

	public CreateNukiAccountAndRedirectToNukiInteractor(INukiAccountExternalRepository nukiAccountExternalRepository,
		INukiAccountPersistentRepository nukiAccountPersistentRepository)
	{
		_nukiAccountExternalRepository = nukiAccountExternalRepository;
		_nukiAccountPersistentRepository = nukiAccountPersistentRepository;
	}

	public async Task<Result<CreateNukiAccountAndRedirectPresentationResponse>> Handle(
		CreateNukiAccountRedirectPresentationRequest createNukiAccountRedirectPresentationRequest)
	{
		var createResult = await _nukiAccountPersistentRepository.Create(new NukiAccount
			{ ClientId = createNukiAccountRedirectPresentationRequest.ClientId });
		if(createResult.IsFailed)
			 return createResult.ToResult();
		var redirectResult = _nukiAccountExternalRepository.BuildUriForCode(
			new NukiAccountBuildUriForCodeExternalRequest(createNukiAccountRedirectPresentationRequest.ClientId));
		return redirectResult.IsFailed
			? redirectResult.ToResult()
			: new CreateNukiAccountAndRedirectPresentationResponse(redirectResult.Value.RedirectUri);
	}
}
