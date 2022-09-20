using Kerbero.Common.Errors;
using Kerbero.Common.Interactors;
using Kerbero.Common.Interfaces;
using Kerbero.Common.Models;
using Kerbero.WebApi.Models.ErrorMapper;
using Microsoft.AspNetCore.Mvc;

namespace Kerbero.WebApi.Controllers;

[Route("nuki/auth")]
public class NukiAuthenticationController : Controller
{
	private readonly Interactor<string, Uri> _provideRedirectUrlInteractor;
	private readonly InteractorAsync<NukiAccountExternalRequestDto, NukiAccountPresentationDto> _createAccountInteractor;

	public NukiAuthenticationController(Interactor<string, Uri> provideRedirectUrlInteractor,
		InteractorAsync<NukiAccountExternalRequestDto, NukiAccountPresentationDto> createAccountInteractor)
	{
		_provideRedirectUrlInteractor = provideRedirectUrlInteractor;
		_createAccountInteractor = createAccountInteractor;
	}

	/// <summary>
	/// Index method controller
	/// </summary>
	/// <param name="clientId"></param>
	[HttpPost]
	public RedirectResult RedirectForCode(string clientId)
	{
		var interactorResult = _provideRedirectUrlInteractor.Handle(clientId);
		
		if (interactorResult.IsSuccess)
			return Redirect(interactorResult.Value.ToString());

		throw HttpResponseExceptionMap.Map((KerberoError)interactorResult.Errors.First());
	}
	
	/// <summary>
	/// That endpoint is called from Nuki Apis with a valid OAuth2 code
	/// </summary>
	/// <param name="clientId"></param>
	/// clientId must be specified in redirect url inserted in Nuki Web Api by the user
	/// <param name="code"></param>
	/// <returns></returns>
	[HttpGet("{clientId}")]
	public async Task<NukiAccountPresentationDto> RetrieveToken(string clientId, string code)
	{
		var interactorResponse = await _createAccountInteractor.Handle(new NukiAccountExternalRequestDto()
		{
			Code = code,
			ClientId = clientId
		});
		
		if (interactorResponse.IsSuccess) return interactorResponse.Value;

		throw HttpResponseExceptionMap.Map((KerberoError)interactorResponse.Errors.First());
	}
}
