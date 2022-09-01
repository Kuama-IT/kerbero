using KerberoWebApi.Clients.Nuki;
using KerberoWebApi.Models;
using KerberoWebApi.Models.Device;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KerberoWebApi.Controllers;

[Microsoft.AspNetCore.Mvc.Route("nuki")]
[ApiController]
public class NukiAuthController : ControllerBase
{
  private ILogger<NukiAuthController> _logger;

  private NukiClientAuthentication _nukiVendorClient;

  private readonly DeviceVendorAccountContext _context;


  public NukiAuthController(ILogger<NukiAuthController> logger, NukiClientAuthentication nukiVendorClient,  DeviceVendorAccountContext context)
  {
    _context = context;
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
  public async Task<ActionResult<DeviceVendorAccount>> CodeCallback(string? code, string? clientId, string? error, string? error_description)
  {
    if(!String.IsNullOrWhiteSpace(error))
      throw new BadHttpRequestException(error + error_description);
    _nukiVendorClient.SetClientId(clientId);

    // test vendorAccount for DB
    var vendorAccount = new DeviceVendorAccount() {
      Token = code,
      ApiKey = "dasdasd",
      ClientId = clientId,
      RefreshToken = "asdasda",
      ClientSecret = "dasffdfa",
      Name = "nuki",
      Id = "NOTNULL"
    };
    _context.DeviceVendorAccountList.Add(vendorAccount);
    await _context.SaveChangesAsync();

    // var response = await _nukiVendorClient.RetrieveTokens(code);

    // "qualcosachepuòaccedereadb".saveToken(_context);

    return CreatedAtAction(nameof(GetVendorAccountList), new { id = vendorAccount.Id }, vendorAccount);
  } 
  
  [HttpGet("token")]
  public Task<Object?> TokenCallback(string code)
  {
    _nukiVendorClient.SuccessfulCallback();
    return null;
  }

  [HttpGet("list")]
  public async Task<ActionResult<IEnumerable<DeviceVendorAccount>>> GetVendorAccountList()
  {
      return await _context.DeviceVendorAccountList.ToListAsync();
  }
}