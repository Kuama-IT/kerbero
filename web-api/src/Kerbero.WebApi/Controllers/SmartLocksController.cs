using Kerbero.Domain.Common.Models;
using Kerbero.Domain.NukiCredentials.Dtos;
using Kerbero.Domain.NukiCredentials.Interfaces;
using Kerbero.Domain.NukiCredentials.Mappers;
using Kerbero.Domain.SmartLocks.Dtos;
using Kerbero.Domain.SmartLocks.Interfaces;
using Kerbero.Domain.SmartLocks.Models;
using Kerbero.WebApi.Extensions;
using Kerbero.WebApi.Models.Requests;
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

  public SmartLocksController(IGetSmartLocksInteractor getSmartLocksInteractor,
    IGetNukiCredentialsByUserInteractor getNukiCredentialsByUserInteractor,
    IOpenSmartLockInteractor openSmartLockInteractor)
  {
    _getSmartLocksInteractor = getSmartLocksInteractor;
    _getNukiCredentialsByUserInteractor = getNukiCredentialsByUserInteractor;
    _openSmartLockInteractor = openSmartLockInteractor;
  }

  [Authorize]
  [HttpGet]
  public async Task<ActionResult<List<SmartLockDto>>> GetAll()
  {
    var userId = HttpContext.GetAuthenticatedUserId();

    var nukiCredentialsResult = await _getNukiCredentialsByUserInteractor.Handle(userId: userId);

    if (nukiCredentialsResult.IsFailed)
    {
      var error = nukiCredentialsResult.Errors.First();
      return ModelState.AddErrorAndReturnAction(error);
    }

    var nukiCredentials = NukiCredentialMapper.Map(nukiCredentialsResult.Value);
    var smartLocksResult = await _getSmartLocksInteractor.Handle(nukiCredentials);

    if (smartLocksResult.IsFailed)
    {
      var error = nukiCredentialsResult.Errors.First();
      return ModelState.AddErrorAndReturnAction(error);
    }

    return smartLocksResult.Value;
  }

  [Authorize]
  [HttpPut]
  [Route("{smartLockId}/open")]
  public async Task<ActionResult> Open( OpenSmartLockRequest request, string smartLockId)
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
}
