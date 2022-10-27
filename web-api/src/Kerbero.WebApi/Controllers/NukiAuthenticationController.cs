using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.NukiAuthentication.Interfaces;
using Kerbero.Domain.NukiAuthentication.Models.PresentationRequests;
using Kerbero.Domain.NukiAuthentication.Models.PresentationResponses;
using Kerbero.WebApi.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Kerbero.WebApi.Controllers;

[ApiController]
[Route("nuki/auth")]
public class NukiAuthenticationController: ControllerBase
{
	private readonly Interactor<NukiRedirectPresentationRequest, NukiRedirectPresentationResponse> _provideRedirectUrlInteractor;
	private readonly InteractorAsync<NukiAccountPresentationRequest, NukiAccountPresentationResponse> _createAccountInteractor;

	public NukiAuthenticationController(IProvideNukiAuthRedirectUrlInteractor provideRedirectUrlInteractor,
		ICreateNukiAccountInteractor createAccountInteractor)
	{
		_provideRedirectUrlInteractor = provideRedirectUrlInteractor;
		_createAccountInteractor = createAccountInteractor;
	}

	/// <summary>
	/// Index method controller
	/// </summary>
	/// <param name="clientId"></param>
	[HttpGet("start")]
	public ActionResult RedirectByClientId(string clientId)
	{
		var interactorResponse = _provideRedirectUrlInteractor.Handle(new NukiRedirectPresentationRequest(clientId));
		if (interactorResponse.IsSuccess)
			return Redirect(interactorResponse.Value.RedirectUri.ToString());
		
		var error = interactorResponse.Errors.First();
		return ModelState.AddErrorAndReturnAction(error);
	}
	
	/// <summary>
	/// That endpoint is called from Nuki Apis with a valid OAuth2 code
	/// </summary>
	/// <param name="clientId"></param>
	/// clientId must be specified in redirect url inserted in Nuki Web Api by the user
	/// <param name="code"></param>
	/// <returns></returns>
	[HttpGet("token/{clientId}")]
	public async Task<ActionResult<NukiAccountPresentationResponse>> RetrieveTokenByCode(string clientId, string code)
	{
		var interactorResponse = await _createAccountInteractor.Handle(new NukiAccountPresentationRequest
		{
			Code = code,
			ClientId = clientId
		});
		if (interactorResponse.IsSuccess)
		{
			return Ok(interactorResponse.Value);
		}

		var error = interactorResponse.Errors.First();
		return ModelState.AddErrorAndReturnAction(error);
	}
}
