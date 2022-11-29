using Kerbero.Domain.NukiAuthentication.Dtos;
using Kerbero.Domain.NukiAuthentication.Interfaces;
using Kerbero.WebApi.Extensions;
using Kerbero.WebApi.Models.Requests;
using Kerbero.WebApi.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kerbero.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class NukiCredentialsController : ControllerBase
{
  private readonly ICreateNukiCredentialInteractor _createNukiCredential;

  public NukiCredentialsController(
    ICreateNukiCredentialInteractor createNukiCredential
  )
  {
    _createNukiCredential = createNukiCredential;
  }

  /// <summary>
  /// This endpoint is called from Nuki Apis with a valid OAuth2 code after the user grants US access to his Nuki Account
  /// </summary>
  /// <param name="apiToken"></param>
  /// <returns></returns>
  [HttpPost]
  public async Task<ActionResult<NukiCredentialDto>> CreateNukiCredentialsWithToken(
    [FromBody] CreateNukiSmartLockCredentialRequest request)
  {
    var interactorResponse = await _createNukiCredential.Handle(
      new CreateNukiCredentialParams
      {
        Token = request.Token,
        UserId = HttpContext.GetAuthenticatedUserId()
      });

    if (interactorResponse.IsFailed)
    {
      var error = interactorResponse.Errors.First();
      return ModelState.AddErrorAndReturnAction(error);
    }

    return interactorResponse.Value;
  }
}