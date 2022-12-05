using Kerbero.Domain.Common.Models;
using Kerbero.Domain.SmartLockKeys.Interfaces;
using Kerbero.WebApi.Dtos.SmartLockKeys;
using Kerbero.WebApi.Extensions;
using Kerbero.WebApi.Mappers;
using Kerbero.WebApi.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kerbero.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SmartLockKeysController : ControllerBase
{
  private readonly ICreateSmartLockKeyInteractor _createSmartLockKeyInteractor;
  private readonly IGetSmartLockKeysInteractor _getSmartLockKeysInteractor;
  private readonly IOpenSmartLockWithKeyInteractor _openSmartLockWithKeyInteractor;
  private readonly ICloseSmartLockWithKeyInteractor _closeSmartLockWithKeyInteractor;
  private readonly IDeleteSmartLockKeyInteractor _deleteSmartLockKeyInteractor;
  private readonly IUpdateSmartLockKeyValidityInteractor _updateSmartLockKeyValidityInteractor;

  public SmartLockKeysController(
    ICreateSmartLockKeyInteractor createSmartLockKeyInteractor,
    IGetSmartLockKeysInteractor getSmartLockKeysInteractor,
    IDeleteSmartLockKeyInteractor deleteSmartLockKeyInteractor,
    IOpenSmartLockWithKeyInteractor openSmartLockWithKeyInteractor,
    ICloseSmartLockWithKeyInteractor closeSmartLockWithKeyInteractor,
    IUpdateSmartLockKeyValidityInteractor updateSmartLockKeyValidityInteractor)
  {
    _createSmartLockKeyInteractor = createSmartLockKeyInteractor;
    _getSmartLockKeysInteractor = getSmartLockKeysInteractor;
    _openSmartLockWithKeyInteractor = openSmartLockWithKeyInteractor;
    _closeSmartLockWithKeyInteractor = closeSmartLockWithKeyInteractor;
    _updateSmartLockKeyValidityInteractor = updateSmartLockKeyValidityInteractor;
    _deleteSmartLockKeyInteractor = deleteSmartLockKeyInteractor;
  }

  [HttpPost]
  public async Task<ActionResult<SmartLockKeyResponseDto>> CreateSmartLockKeyBySmartLockId(
    CreateSmartLockKeyRequestDto requestDto)
  {
    var smartLockProvider = SmartLockProvider.TryParse(requestDto.SmartLockProvider);

    if (smartLockProvider is null)
    {
      return BadRequest();
    }

    var createInteractorResult = await _createSmartLockKeyInteractor.Handle(
      smartLockId: requestDto.SmartLockId,
      validUntilDate: requestDto.ValidUntilDate,
      validFromDate: requestDto.ValidFromDate,
      credentialId: requestDto.CredentialId,
      smartLockProvider: smartLockProvider
    );

    if (createInteractorResult.IsFailed)
    {
      var error = createInteractorResult.Errors.First();
      return ModelState.AddErrorAndReturnAction(error);
    }

    return SmartLockKeyMapper.Map(createInteractorResult.Value);
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
  [HttpPut("open-smart-lock")]
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
  
  [AllowAnonymous]
  [HttpPut("close-smart-lock")]
  public async Task<ActionResult> CloseSmartLockWithKeyAndPassword(CloseSmartLockWithKeyRequestDto request)
  {
    var interactorResponse = await _closeSmartLockWithKeyInteractor.Handle(
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

  [HttpPut("{smartLockKeyId}")]
  public async Task<ActionResult<SmartLockKeyResponseDto>> UpdateById(string smartLockKeyId,
    UpdateSmartLockKeyRequestDto requestDto)
  {
    var parsingResult = Guid.TryParse(smartLockKeyId, out var smartLockKeyGuid);
    if (!parsingResult)
    {
      return BadRequest();
    }

    var interactorResult = await _updateSmartLockKeyValidityInteractor.Handle(
      userId: HttpContext.GetAuthenticatedUserId(),
      smartLockKeyGuid: smartLockKeyGuid,
      validUntil: requestDto.ValidUntilDate,
      validFrom: requestDto.ValidFromDate
    );

    if (interactorResult.IsFailed)
    {
      var error = interactorResult.Errors.First();
      return ModelState.AddErrorAndReturnAction(error);
    }

    return SmartLockKeyMapper.Map(interactorResult.Value);
  }

  [HttpDelete("{smartLockKeyId}")]
  public async Task<ActionResult<SmartLockKeyResponseDto>> DeleteById(string smartLockKeyId)
  {
    var parsingResult = Guid.TryParse(smartLockKeyId, out var smartLockKeyGuid);
    if (!parsingResult)
    {
      return BadRequest();
    }

    var interactorResult = await _deleteSmartLockKeyInteractor.Handle(
      HttpContext.GetAuthenticatedUserId(),
      smartLockKeyGuid);

    if (interactorResult.IsFailed)
    {
      var error = interactorResult.Errors.First();
      return ModelState.AddErrorAndReturnAction(error);
    }

    return SmartLockKeyMapper.Map(interactorResult.Value);
  }
}