using Microsoft.AspNetCore.Mvc;
using KerberoWebApi.Clients;
using KerberoWebApi.Models;
using KerberoWebApi.Models.Device;

namespace KerberoWebApi.Controllers;

/// <summary>
/// Manage or query Smartlock linked to an account
/// </summary>
[ApiController]
[Route("smartlock")]
public class Smartlock : ControllerBase
{

    private readonly ILogger<Smartlock> _logger;
    private readonly ApplicationContext _context;
    private readonly IEnumerable<IVendorClient> _clients;

    public Smartlock(IEnumerable<IVendorClient> clients, ILogger<Smartlock> logger, ApplicationContext context)
    {
        _clients = clients;
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// Register a Device/SmartLock on Db
    /// </summary>
    /// <param name="hostId"></param>
    /// <returns></returns>
    [HttpPost("register")]
    public async Task RegisterSmartLock(int hostId, string vendor)
    {
        var account = getAuthenticatedAccount(hostId, vendor) ?? throw new BadHttpRequestException("No account associated to the vendor provided");
        
        return ;
    }
    
    /// <summary>
    /// Return an VendorAccount given host id and vendor name, if there is not it returns null
    /// </summary>
    /// <param name="hostId"></param>
    /// <param name="vendor"></param>
    /// <returns></returns>
    public DeviceVendorAccount? getAuthenticatedAccount(int hostId, string vendor)
    {
        return _context.DeviceVendorAccountList
        .FirstOrDefault(vendorAccount => vendorAccount.HostId == hostId);
    }


}
