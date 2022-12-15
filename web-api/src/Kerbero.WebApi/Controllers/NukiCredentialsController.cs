using Kerbero.Domain.NukiCredentials.Interfaces;
using Kerbero.WebApi.Dtos.NukiCredentials;
using Kerbero.WebApi.Extensions;
using Kerbero.WebApi.Mappers;
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
  private readonly IGetNukiCredentialsByUserInteractor _getNukiCredentialsInteractor;
  private readonly IDeleteNukiCredentialInteractor _deleteNukiCredentialInteractor;
  private readonly IBuildWebAppRedirectUriInteractor _buildWebAppRedirectUriInteractor;
  private readonly IConfiguration _configuration;

  public NukiCredentialsController(
    ICreateNukiCredentialInteractor createNukiCredential,
    ICreateNukiCredentialDraftInteractor createNukiCredentialDraft,
    IConfiguration configuration,
    IConfirmNukiDraftCredentialsInteractor confirmNukiDraftCredentials,
    IBuildNukiRedirectUriInteractor buildNukiRedirectUri,
    IGetNukiCredentialsByUserInteractor getNukiCredentialsByUserInteractor,
    IDeleteNukiCredentialInteractor deleteNukiCredentialInteractor,
    IBuildWebAppRedirectUriInteractor buildWebAppRedirectUriInteractor
  )
  {
    _createNukiCredential = createNukiCredential;
    _createNukiCredentialDraft = createNukiCredentialDraft;
    _configuration = configuration;
    _confirmNukiDraftCredentials = confirmNukiDraftCredentials;
    _buildNukiRedirectUri = buildNukiRedirectUri;
    _getNukiCredentialsInteractor = getNukiCredentialsByUserInteractor;
    _deleteNukiCredentialInteractor = deleteNukiCredentialInteractor;
    _buildWebAppRedirectUriInteractor = buildWebAppRedirectUriInteractor;
  }

  /// <summary>
  /// Prepares Nuki Credential draft for current user and returns where to redirect the user to continue with the create
  /// Nuki Account procedure
  /// </summary>
  [HttpPost("draft")]
  public async Task<ActionResult<NukiCredentialDraftResponseDto>> CreateDraft()
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
  /// <param name="nukiCredentialRegistered">Return by nuki second tim</param>
  /// <returns></returns>
  [HttpGet("confirm-draft-hook")]
  public async Task<ActionResult<NukiCredentialResponseDto>> ConfirmDraft([FromQuery]string? code, [FromQuery(Name = "nuki-credential-registered")] bool nukiCredentialRegistered)
  {
    if (code == null)
    {
      // Redirect to web app
      var redirectUriResult = await _buildWebAppRedirectUriInteractor.Handle(nukiCredentialRegistered);
      if (redirectUriResult.IsFailed)
      {
        var error = redirectUriResult.Errors.First();
        return ModelState.AddErrorAndReturnAction(error);
      }
      var redirectToWebApp = redirectUriResult.Value;
      return Redirect(redirectToWebApp.ToString());
    }
    
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
  public async Task<ActionResult<NukiCredentialResponseDto>> CreateNukiCredentialsWithToken(
    CreateNukiCredentialRequestDto request)
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

    return NukiCredentialMapper.Map(interactorResponse.Value);
  }

  [HttpGet]
  public async Task<ActionResult<NukiCredentialListResponseDto>> GetAll()
  {
    var interactorResponse = await _getNukiCredentialsInteractor.Handle(HttpContext.GetAuthenticatedUserId());

    if (interactorResponse.IsFailed)
    {
      var error = interactorResponse.Errors.First();
      return ModelState.AddErrorAndReturnAction(error);
    }

    return NukiCredentialMapper.Map(interactorResponse.Value);
  }

  [HttpDelete("{nukiCredentialId}")]
  public async Task<ActionResult<NukiCredentialResponseDto>> DeleteNukiCredentialsById(int nukiCredentialId)
  {
    var interactorResponse = await _deleteNukiCredentialInteractor.Handle(
      HttpContext.GetAuthenticatedUserId(),
      nukiCredentialId);
    if (interactorResponse.IsFailed)
    {
      var error = interactorResponse.Errors.First();
      return ModelState.AddErrorAndReturnAction(error);
    }

    return NukiCredentialMapper.Map(interactorResponse.Value);
  }
}
