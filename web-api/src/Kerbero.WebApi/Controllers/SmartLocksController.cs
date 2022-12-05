using Kerbero.Domain.Common.Models;
using Kerbero.Domain.NukiCredentials.Interfaces;
using Kerbero.Domain.SmartLocks.Interfaces;
using Kerbero.WebApi.Dtos.SmartLocks;
using Kerbero.WebApi.Extensions;
using Kerbero.WebApi.Mappers;
using Kerbero.WebApi.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kerbero.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[Controller]")]
public class SmartLocksController : ControllerBase
{
  private readonly IGetSmartLocksInteractor _getSmartLocksInteractor;
  private readonly IGetNukiCredentialsByUserInteractor _getNukiCredentialsByUserInteractor;
  private readonly IOpenSmartLockInteractor _openSmartLockInteractor;
  private readonly ICloseSmartLockInteractor _closeSmartLockInteractor;

  public SmartLocksController(IGetSmartLocksInteractor getSmartLocksInteractor,
    IGetNukiCredentialsByUserInteractor getNukiCredentialsByUserInteractor,
    IOpenSmartLockInteractor openSmartLockInteractor,
    ICloseSmartLockInteractor closeSmartLockInteractor)
  {
    _getSmartLocksInteractor = getSmartLocksInteractor;
    _getNukiCredentialsByUserInteractor = getNukiCredentialsByUserInteractor;
    _openSmartLockInteractor = openSmartLockInteractor;
    _closeSmartLockInteractor = closeSmartLockInteractor;
  }

  [Authorize]
  [HttpGet]
  public async Task<ActionResult<List<SmartLockResponseDto>>> GetAll()
  {
    var userId = HttpContext.GetAuthenticatedUserId();

    var nukiCredentialsResult = await _getNukiCredentialsByUserInteractor.Handle(userId: userId);

    if (nukiCredentialsResult.IsFailed)
    {
      var error = nukiCredentialsResult.Errors.First();
      return ModelState.AddErrorAndReturnAction(error);
    }

    var smartLocksResult = await _getSmartLocksInteractor.Handle(nukiCredentialsResult.Value);

    if (smartLocksResult.IsFailed)
    {
      var error = nukiCredentialsResult.Errors.First();
      return ModelState.AddErrorAndReturnAction(error);
    }

    return SmartLockMapper.Map(smartLocksResult.Value);
  }

  [Authorize]
  [HttpPut]
  [Route("{smartLockId}/open")]
  public async Task<ActionResult> Open( OpenSmartLockRequestDto request, string smartLockId)
  {
    var provider = SmartLockProvider.TryParse(request.SmartLockProvider);

    if (provider is null)
    {
      return BadRequest();
    }

    var userId = HttpContext.GetAuthenticatedUserId();

    var result = await _openSmartLockInteractor.Handle(
      smartLockProvider: provider,
      userId: userId,
      smartLockId: smartLockId,
      credentialId: request.CredentialsId
    );

    if (result.IsFailed)
    {
      var error = result.Errors.First();
      return ModelState.AddErrorAndReturnAction(error);
    }

    return NoContent();
  }
  
  [Authorize]
  [HttpPut]
  [Route("{smartLockId}/close")]
  public async Task<ActionResult> Close( CloseSmartLockRequest request, string smartLockId)
  {
    var provider = SmartLockProvider.TryParse(request.SmartLockProvider);

    if (provider is null)
    {
      return BadRequest();
    }

    var userId = HttpContext.GetAuthenticatedUserId();

    var result = await _closeSmartLockInteractor.Handle(
      smartLockProvider: provider,
      userId: userId,
      smartLockId: smartLockId,
      credentialId: request.CredentialsId
    );

    if (result.IsFailed)
    {
      var error = result.Errors.First();
      return ModelState.AddErrorAndReturnAction(error);
    }

    return NoContent();
  }
}
