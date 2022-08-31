using KerberoWebApi.Clients.Nuki;
using KerberoWebApi.Clients.Nuki.Response;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;

namespace KerberoWebApi.Controllers;

[Microsoft.AspNetCore.Mvc.Route("VendorAuthorization")]
[ApiController]
public class NukiAuthController : ControllerBase
{
  private ILogger<VendorAuthorizationController> _logger;

  private NukiVendorClient _nukiVendorClient;

  public NukiAuthController(ILogger<VendorAuthorizationController> logger, NukiVendorClient nukiVendorClient)
  {
    _logger = logger;
    _nukiVendorClient = nukiVendorClient;
  }

  [Microsoft.AspNetCore.Mvc.Route("start")]
  [HttpGet(Name = "start")]
  public async Task<bool> Get(string clientId)
  {
    HttpContext.Session.SetString("clientId", clientId);

    _nukiVendorClient.SetClientId(clientId);

    return await _nukiVendorClient.StartAuthFlow();
  }

  [Microsoft.AspNetCore.Mvc.Route("auth")]
  [HttpGet(Name = "code")]
  public async Task CodeCallback(string code)
  {
    var clientId = HttpContext.Session.GetString("clientId");
    _nukiVendorClient.SetClientId(clientId);

    var response = await _nukiVendorClient.RetrieveTokens(code);
    
    _logger.LogDebug(response.AccessToken);

    "qualcosachepuòaccedereadb".saveToken();
  }
}