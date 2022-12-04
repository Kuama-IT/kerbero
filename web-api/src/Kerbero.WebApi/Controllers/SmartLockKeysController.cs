using Kerbero.Domain.Common.Models;
using Kerbero.Domain.SmartLockKeys.Interfaces;
using Kerbero.WebApi.Dtos;
using Kerbero.WebApi.Extensions;
using Kerbero.WebApi.Mappers;
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
	public async Task<ActionResult<SmartLockKeyResponseDto>> CreateSmartLockKeyBySmartLockId(CreateSmartLockKeyRequestDto requestDto)
	{
		var provider = SmartLockProvider.TryParse(requestDto.SmartLockProvider);

		if (provider is null)
		{
			return BadRequest();
		}
		
		var createInteractorResult = await _createSmartLockKeyInteractor.Handle(
				requestDto.SmartLockId,
				requestDto.ExpiryDate,
				requestDto.CredentialId,
				provider);
		
		if (createInteractorResult.IsFailed)
		{
			var error = createInteractorResult.Errors.First();
			return ModelState.AddErrorAndReturnAction(error);
		}

		return Ok(createInteractorResult.Value);
	}
	
	[HttpGet]
	public async Task<ActionResult<List<SmartLockKeyResponseDto>>> GetAllKeys()
	{
		var interactorResponse = await _getSmartLockKeysInteractor.Handle(
			HttpContext.GetAuthenticatedUserId());
		
		if (interactorResponse.IsFailed)
		{
			var error = interactorResponse.Errors.First();
			return ModelState.AddErrorAndReturnAction(error);
		}

		return SmartLockKeyMapper.Map(interactorResponse.Value);
	}

	[AllowAnonymous]
	[HttpPut("open-smartlock")]
	public async Task<ActionResult> OpenSmartLockWithKeyAndPassword(OpenSmartLockWithKeyRequestDto request)
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
