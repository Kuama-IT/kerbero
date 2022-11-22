using Kerbero.Domain.NukiActions.Interfaces;
using Kerbero.Domain.NukiActions.Models.PresentationRequest;
using Kerbero.Domain.NukiAuthentication.Dtos;
using Kerbero.Domain.NukiAuthentication.Interfaces;
using Kerbero.WebApi.Models.Requests;
using Kerbero.WebApi.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kerbero.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/smartlocks")]
public class NukiSmartLockController : ControllerBase
{
	private readonly IGetNukiAccountInteractor _getNukiAccountInteractor;
	private readonly IOpenNukiSmartLockInteractor _openNukiSmartLockInteractor;
	private readonly IGetNukiSmartLocksInteractor _getNukiSmartLocksInteractor;
	private readonly ICreateNukiSmartLockInteractor _createNukiSmartLockInteractor;
	private readonly ICloseNukiSmartLockInteractor _closeNukiSmartLockInteractor;

	public NukiSmartLockController(IGetNukiAccountInteractor getNukiAccountInteractor,
		IGetNukiSmartLocksInteractor getNukiSmartLocksInteractor,
		ICreateNukiSmartLockInteractor createNukiSmartLockInteractor,
		IOpenNukiSmartLockInteractor openNukiSmartLockInteractor,
		ICloseNukiSmartLockInteractor closeNukiSmartLockInteractor)
	{
		_getNukiAccountInteractor = getNukiAccountInteractor;
		_openNukiSmartLockInteractor = openNukiSmartLockInteractor;
		_getNukiSmartLocksInteractor = getNukiSmartLocksInteractor;
		_createNukiSmartLockInteractor = createNukiSmartLockInteractor;
		_closeNukiSmartLockInteractor = closeNukiSmartLockInteractor;
	}


	[HttpGet]
	public async Task<ActionResult> GetSmartLocksByKerberoAccount(int accountId)
	{
		var authenticationResponse = await _getNukiAccountInteractor.Handle(new GetNukiAccountParams
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
	
	[HttpPost("import/nuki/")]
	public async Task<ActionResult> CreateNukiSmartLockById(CreateNukiSmartLockRequest createNukiSmartLockRequest)
	{
		var authenticationResponse = await _getNukiAccountInteractor.Handle(new GetNukiAccountParams
		{
			NukiAccountId = createNukiSmartLockRequest.AccountId
		});
		if (authenticationResponse.IsFailed)
		{
			var error = authenticationResponse.Errors.First();
			return ModelState.AddErrorAndReturnAction(error);
		}
		var interactorResponse =
			await _createNukiSmartLockInteractor.Handle(new CreateNukiSmartLockPresentationRequest(
				authenticationResponse.Value.Token, createNukiSmartLockRequest.AccountId,
				createNukiSmartLockRequest.ExternalSmartLockId));
		if (interactorResponse.IsFailed)
		{
			var error = interactorResponse.Errors.First();
			return ModelState.AddErrorAndReturnAction(error);
		}

		return Ok(interactorResponse.Value);
	}
	
	[HttpPut("unlock")]
	public async Task<ActionResult> OpenNukiSmartLockById(OpenNukiSmartLockRequest openNukiSmartLockRequest)
	{
		var authenticationResponse = await _getNukiAccountInteractor.Handle(new GetNukiAccountParams
		{
			NukiAccountId = openNukiSmartLockRequest.AccountId
		});
		if (authenticationResponse.IsFailed)
		{
			var error = authenticationResponse.Errors.First();
			return ModelState.AddErrorAndReturnAction(error);
		}

		var interactorResponse =
			await _openNukiSmartLockInteractor.Handle(
				new OpenNukiSmartLockPresentationRequest(authenticationResponse.Value.Token, openNukiSmartLockRequest.SmartLockId)
			);

		if (interactorResponse.IsFailed)
		{
			var error = interactorResponse.Errors.First();
			return ModelState.AddErrorAndReturnAction(error);
		}

		return Ok();
	}

	[HttpPut("lock")]
	public async Task<ActionResult> CloseSmartLockById(CloseNukiSmartLockRequest closeNukiSmartLockRequest)
	{
		var authenticationResponse = await _getNukiAccountInteractor.Handle(
			new GetNukiAccountParams
		{
			NukiAccountId = closeNukiSmartLockRequest.AccountId
		});
		if (authenticationResponse.IsFailed)
		{
			var error = authenticationResponse.Errors.First();
			return ModelState.AddErrorAndReturnAction(error);
		}
		
		var interactorResponse =
			await _closeNukiSmartLockInteractor.Handle(
				new CloseNukiSmartLockPresentationRequest(authenticationResponse.Value.Token, closeNukiSmartLockRequest.SmartLockId));

		if (interactorResponse.IsFailed)
		{
			var error = interactorResponse.Errors.First();
			return ModelState.AddErrorAndReturnAction(error);
		}

		return Ok();
	}
}
