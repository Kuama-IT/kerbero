using Kerbero.Domain.NukiCredentials.Dtos;
using Kerbero.Domain.NukiCredentials.Interfaces;
using Kerbero.Domain.NukiCredentials.Mappers;
using Kerbero.Domain.SmartLocks.Dtos;
using Kerbero.Domain.SmartLocks.Interfaces;
using Kerbero.Domain.SmartLocks.Params;
using Kerbero.WebApi.Extensions;
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

  public SmartLocksController(IGetSmartLocksInteractor getSmartLocksInteractor,
    IGetNukiCredentialsByUserInteractor getNukiCredentialsByUserInteractor)
  {
    _getSmartLocksInteractor = getSmartLocksInteractor;
    _getNukiCredentialsByUserInteractor = getNukiCredentialsByUserInteractor;
  }

  [Authorize]
  [HttpGet]
  public async Task<ActionResult<List<SmartLockDto>>> GetAll()
  {
    var userId = HttpContext.GetAuthenticatedUserId();

    var nukiCredentialsResult = await _getNukiCredentialsByUserInteractor.Handle(
      new GetNukiCredentialsByUserInteractorParams
      {
        UserId = userId,
      });

    if (nukiCredentialsResult.IsFailed)
    {
      var error = nukiCredentialsResult.Errors.First();
      return ModelState.AddErrorAndReturnAction(error);
    }

    var nukiCredentials = NukiCredentialMapper.Map(nukiCredentialsResult.Value);
    var smartLocksResult = await _getSmartLocksInteractor.Handle(new GetSmartLocksInteractorParams(nukiCredentials));

    if (smartLocksResult.IsFailed)
    {
      var error = nukiCredentialsResult.Errors.First();
      return ModelState.AddErrorAndReturnAction(error);
    }

    return smartLocksResult.Value;
  }
}