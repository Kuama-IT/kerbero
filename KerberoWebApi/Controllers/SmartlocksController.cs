using Microsoft.AspNetCore.Mvc;
using KerberoWebApi.Clients;
using KerberoWebApi.Clients.Responses;
using KerberoWebApi.Models;

namespace KerberoWebApi.Controllers;

/// <summary>
/// Manage or query smartlocks linked to an account
/// </summary>
[ApiController]
[Route("smartlocks")]
public class SmartLocks : ControllerBase
{

    private readonly ILogger<SmartLocks> _logger;
    private readonly ApplicationContext _context;
    private readonly IEnumerable<IVendorClient> _clients;

    public SmartLocks(IEnumerable<IVendorClient> clients, ILogger<SmartLocks> logger, ApplicationContext context)
    {
        _clients = clients;
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// It returns a list of smartlock for each vendor
    /// </summary>
    /// <param name="vendorName"></param>
    /// <param name="hostId"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<List<ISmartLockResponse>> GetSmartLocksList(int hostId, string? vendorName)
    {
        // verify the account host id and download the vendor accounts
        var deviceSubscriptions = _context.DeviceVendorAccountList
        .Where(vendorAccount => vendorAccount.HostId == hostId && (String.IsNullOrWhiteSpace(vendorName) || vendorName == vendorAccount.DeviceVendor.Name))
        ?.Select(vendorAccounts => new Tuple<string,string>(vendorAccounts.DeviceVendor.Name, vendorAccounts.Token ?? "")) 
            ?? throw new BadHttpRequestException("This hostId does not exists, or no device vendor accounts are associated to it");
        // assumption: an host owns only a vendor account type
        var vendorSmartlock = new List<ISmartLockResponse>();
        // call the client for each vendor account
        foreach (var deviceSub in deviceSubscriptions)
        {
            var client = _clients.FirstOrDefault(client => client.Name == deviceSub.Item1) 
                ?? throw new BadHttpRequestException($"No Client found for this type of vendor {deviceSub.Item1}");
            client.SetToken(deviceSub.Item2);
            var res = await client.GetSmartLocks();
            vendorSmartlock.AddRange(res);
        }
        // if smartLockId filter is active 
        return vendorSmartlock;
    }
}
