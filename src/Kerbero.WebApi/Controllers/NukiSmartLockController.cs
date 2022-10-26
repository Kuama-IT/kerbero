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
	private readonly IAuthenticateNukiAccountInteractor _authenticateNukiAccountInteractor;
	private readonly IGetNukiSmartLocksInteractor _getNukiSmartLocksInteractor;
	private readonly ICreateNukiSmartLockInteractor _createNukiSmartLockInteractor;

	public NukiSmartLockController(IAuthenticateNukiAccountInteractor authenticateNukiAccountInteractor,
		IGetNukiSmartLocksInteractor getNukiSmartLocksInteractor,
		ICreateNukiSmartLockInteractor createNukiSmartLockInteractor)
	{
		_authenticateNukiAccountInteractor = authenticateNukiAccountInteractor;
		_getNukiSmartLocksInteractor = getNukiSmartLocksInteractor;
		_createNukiSmartLockInteractor = createNukiSmartLockInteractor;
	}


	[HttpGet]
	public async Task<ActionResult> GetSmartLocksByKerberoAccount(int accountId)
	{
		var authenticationResponse = await _authenticateNukiAccountInteractor.Handle(new AuthenticateRepositoryPresentationRequest
		{
			NukiAccountId = accountId
		});
		if (authenticationResponse.IsFailed)
		{
			var error = authenticationResponse.Errors.First();
			return ModelState.AddErrorAndReturnAction(error);
		}

		var interactorResponse = await _getNukiSmartLocksInteractor.Handle(
			new NukiSmartLocksPresentationRequest(authenticationResponse.Value.Token)
		);
		if (interactorResponse.IsFailed)
		{
			var error = interactorResponse.Errors.First();
			return ModelState.AddErrorAndReturnAction(error);
		}

		return Ok(interactorResponse.Value);
	}
	
	
	[HttpGet("{smartLockId:int}")]
	public async Task<ActionResult> CreateNukiSmartLockById(int accountId, int smartLockId)
	{
		var authenticationResponse = await _authenticateNukiAccountInteractor.Handle(new AuthenticateRepositoryPresentationRequest
		{
			NukiAccountId = accountId
		});
		if (authenticationResponse.IsFailed)
		{
			var error = authenticationResponse.Errors.First();
			return ModelState.AddErrorAndReturnAction(error);
		}
		var interactorResponse =
			await _createNukiSmartLockInteractor.Handle(new CreateNukiSmartLockPresentationRequest(authenticationResponse.Value.Token, accountId, smartLockId));
		if (interactorResponse.IsFailed)
		{
			var error = interactorResponse.Errors.First();
			return ModelState.AddErrorAndReturnAction(error);
		}

		return Ok(interactorResponse.Value);
	}
}
