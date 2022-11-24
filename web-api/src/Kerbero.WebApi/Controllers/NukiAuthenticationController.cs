using System.Security.Claims;
using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.NukiAuthentication.Interfaces;
using Kerbero.Domain.NukiAuthentication.Models.PresentationRequests;
using Kerbero.Domain.NukiAuthentication.Models.PresentationResponses;
using Kerbero.WebApi.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kerbero.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/nuki/auth")]
public class NukiAuthenticationController: ControllerBase
{
	private readonly ICreateNukiAccountAndRedirectToNukiInteractor _createRedirectToNukiInteractor;
	private readonly IUpdateNukiAccountWithToken _updateNukiAccountWithTokenAndUpdateAccount;

	public NukiAuthenticationController(ICreateNukiAccountAndRedirectToNukiInteractor createRedirectToNukiInteractor,
		IUpdateNukiAccountWithToken updateNukiAccountWithTokenAndUpdateAccount)
	{
		_createRedirectToNukiInteractor = createRedirectToNukiInteractor;
		_updateNukiAccountWithTokenAndUpdateAccount = updateNukiAccountWithTokenAndUpdateAccount;
	}
	
	/// <summary>
	/// Index method controller
	/// </summary>
	/// <param name="clientId"></param>
	[HttpGet("start")]
	public async Task<ActionResult> CreateNukiAccountAndRedirectByClientId([FromQuery] string clientId)
	{
		// Get UserId from Context
		var nameIdentifierClaim = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
		if (nameIdentifierClaim is null)
		{
			return new BadRequestResult();
		}

		var userId = Guid.Parse(nameIdentifierClaim.Value);
		
		var interactorResponse =
			await _createRedirectToNukiInteractor.Handle(
				new CreateNukiAccountRedirectPresentationRequest(clientId, userId));
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
	[AllowAnonymous]
	[HttpGet("token/{clientId}")]
	public async Task<ActionResult<UpdateNukiAccountPresentationResponse>> RetrieveTokenAndUpdateNukiAccountByCode(string clientId, string code)
	{
		var interactorResponse = await _updateNukiAccountWithTokenAndUpdateAccount.Handle(new UpdateNukiAccountPresentationRequest
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
