using Kerbero.Domain.SmartLockKeys.Dtos;
using Kerbero.Domain.SmartLockKeys.Interfaces;
using Kerbero.Domain.SmartLocks.Models;
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

	public SmartLockKeysController(ICreateSmartLockKeyInteractor createSmartLockKeyInteractor)
	{
		_createSmartLockKeyInteractor = createSmartLockKeyInteractor;
	}

	[HttpPost]
	public async Task<ActionResult<SmartLockKeyDto>> CreateSmartLockKeyBySmartLockId([FromBody] CreateSmartLockKeyRequest request)
	{
		var provider = SmartLockProvider.TryParse(request.SmartLockProvider);

		if (provider is null)
		{
			return BadRequest();
		}
		
		var createInteractorResult = await _createSmartLockKeyInteractor.Handle(
			new CreateSmartLockKeyParams(
				request.SmartLockId,
				request.ExpiryDate,
				request.CredentialId,
				provider));
		
		if (createInteractorResult.IsFailed)
		{
			var error = createInteractorResult.Errors.First();
			return ModelState.AddErrorAndReturnAction(error);
		}

		return Ok(createInteractorResult.Value);
	}
	
	
}
