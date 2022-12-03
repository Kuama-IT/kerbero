using Kerbero.Domain.NukiCredentials.Dtos;
using Kerbero.Domain.NukiCredentials.Interfaces;
using Kerbero.WebApi.Dtos;
using Kerbero.WebApi.Extensions;
using Kerbero.WebApi.Mappers;
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
  private readonly ICreateNukiCredentialDraftInteractor _createNukiCredentialDraft;
  private readonly IConfirmNukiDraftCredentialsInteractor _confirmNukiDraftCredentials;
  private readonly IBuildNukiRedirectUriInteractor _buildNukiRedirectUri;
  private readonly IConfiguration _configuration;

  public NukiCredentialsController(
    ICreateNukiCredentialInteractor createNukiCredential,
    ICreateNukiCredentialDraftInteractor createNukiCredentialDraft,
    IConfiguration configuration,
    IConfirmNukiDraftCredentialsInteractor confirmNukiDraftCredentials, 
    IBuildNukiRedirectUriInteractor buildNukiRedirectUri
  )
  {
    _createNukiCredential = createNukiCredential;
    _createNukiCredentialDraft = createNukiCredentialDraft;
    _configuration = configuration;
    _confirmNukiDraftCredentials = confirmNukiDraftCredentials;
    _buildNukiRedirectUri = buildNukiRedirectUri;
  }
  
  /// <summary>
  /// Prepares Nuki Credential draft for current user and returns where to redirect the user to continue with the create
  /// Nuki Account procedure
  /// </summary>
  [HttpPost("draft")]
  public async Task<ActionResult<NukiCredentialDraftDto>> CreateDraft()
  {
    var userId = HttpContext.GetAuthenticatedUserId();

    var createDraftResult =
      await _createNukiCredentialDraft.Handle(userId);

    if (createDraftResult.IsFailed)
    {
      var error = createDraftResult.Errors.First();
      return ModelState.AddErrorAndReturnAction(error);
    }

    var uriResult = await _buildNukiRedirectUri.Handle();

    if (uriResult.IsFailed)
    {
      var error = uriResult.Errors.First();
      return ModelState.AddErrorAndReturnAction(error);
    }

    return NukiCredentialMapper.Map(uriResult.Value);
  }

  /// <summary>
  /// This endpoint is called from Nuki Apis with a valid OAuth2 code after the user grants US access to his Nuki Account
  /// </summary>
  /// <param name="code">Returned by nuki to let us ask for a token</param>
  /// <returns></returns>
  [HttpGet("confirm-draft-hook")]
  public async Task<ActionResult<NukiCredentialDto>> ConfirmDraft(string code)
  {
    var userId = HttpContext.GetAuthenticatedUserId();

    var interactorResponse = await _confirmNukiDraftCredentials.Handle(
      code,
      userId
    );

    var redirectUrl = _configuration["WEB_APP_URL"] + "?nuki-credential-registered=";

    return RedirectPermanent(redirectUrl + interactorResponse.IsSuccess.ToString().ToLower());
  }

  /// <summary>
  /// This endpoint is called from Nuki Apis with a valid OAuth2 token
  /// </summary>
  /// <param name="request"></param>
  /// <returns></returns>
  [HttpPost]
  public async Task<ActionResult<NukiCredentialDto>> CreateNukiCredentialsWithToken(
     CreateNukiCredentialRequest request)
  {
    var interactorResponse = await _createNukiCredential.Handle(
      userId: HttpContext.GetAuthenticatedUserId(),
      token: request.Token
    );

    if (interactorResponse.IsFailed)
    {
      var error = interactorResponse.Errors.First();
      return ModelState.AddErrorAndReturnAction(error);
    }

    return interactorResponse.Value;
  }
}
