using System.Security.Claims;
using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.NukiAuthentication.Dtos;
using Kerbero.Domain.NukiAuthentication.Interfaces;
using Kerbero.WebApi.Extensions;
using Kerbero.WebApi.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kerbero.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class NukiCredentialsController : ControllerBase
{
  private readonly ICreateNukiCredentialDraftInteractor _createNukiCredentialDraft;
  private readonly ICreateNukiCredentialInteractor _createNukiCredential;
  private readonly IConfiguration _configuration;

  public NukiCredentialsController(
    ICreateNukiCredentialDraftInteractor createNukiCredentialDraft,
    ICreateNukiCredentialInteractor createNukiCredential,
    IConfiguration configuration
  )
  {
    _createNukiCredentialDraft = createNukiCredentialDraft;
    _createNukiCredential = createNukiCredential;
    _configuration = configuration;
  }

  /// <summary>
  /// Prepares Nuki Account draft for current user and returns where to redirect the user to continue with the create
  /// Nuki Account procedure
  /// </summary>
  /// <param name="clientId"></param>
  [HttpPost("draft")]
  public async Task<ActionResult<NukiCredentialDraftDto>> CreateDraft([FromQuery] string clientId)
    // TODO move to body payload (create type)
  {
    var userId = HttpContext.GetAuthenticatedUserId();

    var interactorResponse =
      await _createNukiCredentialDraft.Handle(new CreateNukiCredentialDraftParams(clientId, userId));

    if (interactorResponse.IsFailed)
    {
      var error = interactorResponse.Errors.First();
      return ModelState.AddErrorAndReturnAction(error);
    }

    return interactorResponse.Value;
  }

  /// <summary>
  /// This endpoint is called from Nuki Apis with a valid OAuth2 code after the user grants US access to his Nuki Account
  /// </summary>
  /// <param name="clientId"></param>
  /// clientId must be specified in redirect url inserted in Nuki Web Api by the user
  /// <param name="code"></param>
  /// <returns></returns>
  [AllowAnonymous]
  [HttpGet("confirm-draft-hook/{clientId}")]
  public async Task<ActionResult<NukiCredentialDto>> ConfirmDraft(string clientId, string code)
  {
    var interactorResponse = await _createNukiCredential.Handle(
      new CreateNukiCredentialParams
      {
        Code = code,
        ClientId = clientId,
      });

    var redirectUrl = _configuration["WEB_APP_URL"] + "?nuki-credential-registered=";

    if (interactorResponse.IsFailed)
    {
      return RedirectPermanent(redirectUrl + "false");
    }

    return RedirectPermanent(redirectUrl + "true");
  }
}