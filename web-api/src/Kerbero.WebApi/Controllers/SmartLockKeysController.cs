using Kerbero.Domain.Common.Models;
using Kerbero.Domain.SmartLockKeys.Dtos;
using Kerbero.Domain.SmartLockKeys.Interfaces;
using Kerbero.WebApi.Extensions;
using Kerbero.WebApi.Models.Requests;
using Kerbero.WebApi.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kerbero.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SmartLockKeysController: ControllerBase
{
	private readonly ICreateSmartLockKeyInteractor _createSmartLockKeyInteractor;
	private readonly IGetSmartLockKeysInteractor _getSmartLockKeysInteractor;
	private readonly IOpenSmartLockWithKeyInteractor _openSmartLockWithKeyInteractor;

	public SmartLockKeysController(
		ICreateSmartLockKeyInteractor createSmartLockKeyInteractor, 
		IGetSmartLockKeysInteractor getSmartLockKeysInteractor,
		IOpenSmartLockWithKeyInteractor openSmartLockWithKeyInteractor
		)
	{
		_createSmartLockKeyInteractor = createSmartLockKeyInteractor;
		_getSmartLockKeysInteractor = getSmartLockKeysInteractor;
		_openSmartLockWithKeyInteractor = openSmartLockWithKeyInteractor;
	}

	[HttpPost]
	public async Task<ActionResult<SmartLockKeyDto>> CreateSmartLockKeyBySmartLockId(CreateSmartLockKeyRequest request)
	{
		var provider = SmartLockProvider.TryParse(request.SmartLockProvider);

		if (provider is null)
		{
			return BadRequest();
		}
		
		var createInteractorResult = await _createSmartLockKeyInteractor.Handle(
				request.SmartLockId,
				request.ExpiryDate,
				request.CredentialId,
				provider);
		
		if (createInteractorResult.IsFailed)
		{
			var error = createInteractorResult.Errors.First();
			return ModelState.AddErrorAndReturnAction(error);
		}

		return Ok(createInteractorResult.Value);
	}
	
	[HttpGet]
	public async Task<ActionResult<List<SmartLockKeyDto>>> GetAllKeys()
	{
		var interactorResponse = await _getSmartLockKeysInteractor.Handle(
			HttpContext.GetAuthenticatedUserId());
		
		if (interactorResponse.IsFailed)
		{
			var error = interactorResponse.Errors.First();
			return ModelState.AddErrorAndReturnAction(error);
		}

		return interactorResponse.Value;
	}

	[AllowAnonymous]
	[HttpPut("open-smartlock")]
	public async Task<ActionResult> OpenSmartLockWithKeyAndPassword(OpenSmartLockWithKeyRequest request)
	{
		var interactorResponse = await _openSmartLockWithKeyInteractor.Handle(
			request.SmartLockKeyId,
			request.KeyPassword
		);
		
		if (interactorResponse.IsFailed)
		{
			var error = interactorResponse.Errors.First();
			return ModelState.AddErrorAndReturnAction(error);
		}

		return NoContent();
	}

}
