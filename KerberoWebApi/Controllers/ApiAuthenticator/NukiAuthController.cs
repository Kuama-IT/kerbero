using KerberoWebApi.Clients.Nuki;
using KerberoWebApi.Clients.Nuki.Response;
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

  private readonly ApplicationContext _context;

  public NukiAuthController(ILogger<NukiAuthController> logger, NukiClientAuthentication nukiVendorClient,  ApplicationContext context)
  {
    _context = context;
    _logger = logger;
    _nukiVendorClient = nukiVendorClient;
  }

  /// <summary>
  /// Start the Vendor authentication flow, creates a new entry on db to associate clientId and HostId
  /// </summary>
  /// <param name="clientId"></param>
  /// <returns></returns>
  [HttpGet(Name = "start")]
  public async Task<RedirectResult> Get(string clientId, int hostId)
  {
    // TODO verify a valid request parameters
    var host = _context.HostList.FirstOrDefault(host => host.Id == hostId);
    var deviceVendor = _context.DeviceVendorType.FirstOrDefault(vendor => vendor.Name == "nuki");
    if (host == null || deviceVendor == null)
    {
      throw new BadHttpRequestException("The hostId provided is not registered");
    }
    var vendorAccount = new DeviceVendorAccount() {
      ClientId = clientId,
      Host = host,
      DeviceVendor = deviceVendor     
    };
    _context.DeviceVendorAccountList.Add(vendorAccount);
    await _context.SaveChangesAsync();

    _nukiVendorClient.SetClientId(clientId);
    var uri = _nukiVendorClient.StartAuthFlow();
    // the only way to redirect to the Nuki Web login page is returning from here a RedirectResult
    return uri == null ? throw new BadHttpRequestException("Not able to create a valid Uri for Nuki Web login") : Redirect(uri.ToString());
  }

  /// <summary>
  /// The callback from Nuki Web authentication, it returns a code and calls a get to finally retrieve the api token.
  /// </summary>
  /// <param name="code"></param>
  /// <param name="clientId"></param>
  /// <param name="error"></param>
  /// <param name="error_description"></param>
  /// <returns></returns>
  [HttpGet("code/{clientId}")]
  public async Task CodeCallback(string? code, string? clientId, string? error, string? error_description)
  {
    try
    {
      if(!String.IsNullOrWhiteSpace(error))
        throw new BadHttpRequestException(error + error_description);
      
      _nukiVendorClient.SetClientId(clientId);

      // TODO uncomment when obtain client Secret
      var response = await _nukiVendorClient.RetrieveTokens(code);

      // update the db entry with the tokens and data received
      var vendorAccount = _context.DeviceVendorAccountList.FirstOrDefault(item => item.ClientId == clientId);
      if(vendorAccount != null)
      {
        vendorAccount.ApiKey = response.AccessToken ?? "";
        vendorAccount.RefreshToken = response.RefreshToken ?? "";
        vendorAccount.Token = response.AccessToken ?? "";
        await _context.SaveChangesAsync();
      }
      else
        throw new Exception("Can not retrieve token API for the account");
      return ;
    }
    catch(Exception e)
    {
      // clear invalid entries
      IEnumerable<DeviceVendorAccount> dva = _context.DeviceVendorAccountList.Where(entity => entity.ApiKey == null);
      _context.DeviceVendorAccountList.RemoveRange(dva);
      throw new BadHttpRequestException(e.Message);
    }
  } 
  
  [HttpGet("token")]
  public string TokenCallback(string code)
  {
    _nukiVendorClient.SuccessfulCallback();
    return "Success!";
  }
}