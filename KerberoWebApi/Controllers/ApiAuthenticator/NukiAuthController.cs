using KerberoWebApi.Clients.Nuki;
using Microsoft.AspNetCore.Mvc;

namespace KerberoWebApi.Controllers;

[Microsoft.AspNetCore.Mvc.Route("nuki")]
[ApiController]
public class NukiAuthController : ControllerBase
{
  private ILogger<NukiAuthController> _logger;

  private NukiClientAuthentication _nukiVendorClient;

  public NukiAuthController(ILogger<NukiAuthController> logger, NukiClientAuthentication nukiVendorClient)
  {
    _logger = logger;
    _nukiVendorClient = nukiVendorClient;
  }
  [HttpGet(Name = "start")]
  public RedirectResult Get(string clientId)
  {
    _nukiVendorClient.SetClientId(clientId);
    var uri = _nukiVendorClient.StartAuthFlow();
    // the only way to redirect to the Nuki Web login page is returning from here a RedirectResult
    return uri == null ? throw new BadHttpRequestException("Not able to create a valid Uri for Nuki Web login") : Redirect(uri.ToString());
  }

  [HttpGet("code/{clientId}")]
  public async Task CodeCallback(string? code, string? clientId, string? error, string? error_description)
  {
    if(!String.IsNullOrWhiteSpace(error))
      throw new BadHttpRequestException(error + error_description);
    _nukiVendorClient.SetClientId(clientId);

    var response = await _nukiVendorClient.RetrieveTokens(code);
    
    _logger.LogDebug(response.AccessToken);

    // "qualcosachepuòaccedereadb".saveToken();
  } 
  
  [HttpGet("token")]
  public Task<Object?> TokenCallback(string code)
  {
    _nukiVendorClient.SuccessfulCallback();
    return null;
  }
}