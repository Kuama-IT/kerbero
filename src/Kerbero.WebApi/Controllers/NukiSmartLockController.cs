using FluentResults;
using Kerbero.Domain.NukiActions.Interfaces;
using Kerbero.Domain.NukiActions.Models.PresentationRequest;
using Kerbero.Domain.NukiAuthentication.Interfaces;
using Kerbero.Domain.NukiAuthentication.Models.PresentationRequests;
using Kerbero.WebApi.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Kerbero.WebApi.Controllers;

[ApiController]
[Route("api/nuki/smartlock")]
public class NukiSmartLockController : ControllerBase
{
	private readonly IAuthenticateNukiAccountInteractor _authenticateInteractor;
	private readonly IGetNukiSmartLockListInteractor _nukiSmartLocksListInteractor;

	public NukiSmartLockController(IGetNukiSmartLockListInteractor nukiSmartLocksListInteractor,
		IAuthenticateNukiAccountInteractor authenticateInteractor)
	{
		_authenticateInteractor = authenticateInteractor;
		_nukiSmartLocksListInteractor = nukiSmartLocksListInteractor;
	}


	[HttpGet]
	public async Task<ActionResult> GetSmartLocksByKerberoAccount(int accountId)
	{
		var authenticationResponse = await _authenticateInteractor.Handle(new AuthenticateRepositoryPresentationRequest
		{
			NukiAccountId = accountId
		});
		if (authenticationResponse.IsFailed)
		{
			var error = authenticationResponse.Errors.First();
			return ModelState.AddErrorAndReturnAction(error);
		}

		var interactorResponse = await _nukiSmartLocksListInteractor.Handle(
			new NukiSmartLocksPresentationRequest(authenticationResponse.Value.Token)
		);
		if (interactorResponse.IsFailed)
		{
			var error = interactorResponse.Errors.First();
			return ModelState.AddErrorAndReturnAction(error);
		}

		return Ok(interactorResponse.Value);
	}
}
