using Microsoft.AspNetCore.Mvc;
using KerberoWebApi.Clients;
using KerberoWebApi.Clients.Requests;
using KerberoWebApi.Clients.Responses;
using KerberoWebApi.Models;
using KerberoWebApi.Models.Device;

namespace KerberoWebApi.Controllers;

/// <summary>
/// Manage or query Smartlock linked to an account
/// </summary>
[ApiController]
[Route("smartlock")]
public class SmartLock : ControllerBase
{
	private readonly ILogger<SmartLock> _logger;
	private readonly ApplicationContext _context;
	private readonly IEnumerable<IVendorClient> _clients;

	public SmartLock(IEnumerable<IVendorClient> clients, ILogger<SmartLock> logger, ApplicationContext context)
	{
		_clients = clients;
		_logger = logger;
		_context = context;
	}

	/// <summary>
	/// Retrieve the SmartLock from the DB (update first?)
	/// </summary>
	/// <param name="hostId"></param>
	/// <param name="smartLockId"></param>
	/// <param name="update"></param>
	[HttpGet]
	public async Task<DeviceSmartLock> GetSmartLock(int hostId, int smartLockId, bool update)
	{
		// retrieve the account with the hostId from the vendor, if there is any throw code 400 
		var account = GetAuthenticatedAccount(hostId) ??
		              throw new BadHttpRequestException("No account associated to the vendor provided");
		var deviceSmartLock = _context.DeviceList.FirstOrDefault(device =>
			                      device.DeviceVendorAccount.Id == account.Id && device.Id == smartLockId) ??
		                      throw new BadHttpRequestException("No device founded in db");
		if (update)
		{
			// update the information from the vendor API
			var client =
				_clients.FirstOrDefault(clientE => clientE.Name == 
				                                   _context.DeviceVendorType.FirstOrDefault(vendor => vendor.Id 
						                                   == deviceSmartLock.DeviceVendorAccount.DeviceVendorId)
					                                   ?.Name) ??
				throw new BadHttpRequestException("Invalid smartlock vendor name");
			client.SetToken(account.Token ?? throw new BadHttpRequestException("Invalid account, no API key"));
			ISmartLockResponse res = await client.GetSmartLock(deviceSmartLock.VendorSmartlockId);
			deviceSmartLock = client.MapSmartLockDeviceResponse(res, account);
			
			_context.DeviceList.Update(deviceSmartLock);
			await _context.SaveChangesAsync();
		}

		deviceSmartLock.DeviceVendorAccount = new DeviceVendorAccount(); // to avoid cycle
		return deviceSmartLock;
	}
	
	/// <summary>
	/// Register a Device/SmartLock on Db
	/// </summary>
	/// <param name="request"></param>
	/// <param name="hostId"></param>
	/// <param name="vendor"></param>
	/// <returns></returns>
	[HttpPost("register")]
	public async Task RegisterSmartLock(SmartLockRequest request, int hostId, string vendor)
	{
		// retrieve the account with the hostId from the vendor, if there is any throw code 400 
		var account = GetAuthenticatedAccount(hostId) ??
		              throw new BadHttpRequestException("No account associated to the vendor provided");
		// get the client 
		var client = _clients.FirstOrDefault(clientE => clientE.Name == vendor) ??
		             throw new BadHttpRequestException("Invalid smartlock vendor name");
		client.SetToken(account.Token ?? throw new BadHttpRequestException("Invalid account, no API key"));
		// map request on Device db
		var device = client.MapSmartLockDeviceRequest(request, account);
		//upload on db
		_context.DeviceList.Add(device);
		await _context.SaveChangesAsync();
		return;
	}

	/// <summary>
	/// Open the SmartLock
	/// </summary>
	/// <param name="hostId"></param>
	/// <param name="smartLockId"></param>
	/// <exception cref="BadHttpRequestException"></exception>
	[HttpGet("open")]
	public async Task<bool> OpenSmartLock(int hostId, int smartLockId)
	{
		// retrieve the account with the hostId from the vendor, if there is any throw code 400 
		var account = GetAuthenticatedAccount(hostId) ??
		              throw new BadHttpRequestException("No account associated to the vendor provided");
		// get the smartLock
		var smartLock = _context.DeviceList.FirstOrDefault(device => device.Id == smartLockId)
			?? throw new BadHttpRequestException($"No smartLock found with {smartLockId} id");
		// get the client
		var client =
			_clients.FirstOrDefault(clientE => clientE.Name == 
			                                   _context.DeviceVendorType.FirstOrDefault(vendor => vendor.Id 
					                                   == smartLock.DeviceVendorAccount.DeviceVendorId)
				                                   ?.Name) ??
			throw new BadHttpRequestException("Invalid smartlock vendor name");
		client.SetToken(account.Token ?? throw new BadHttpRequestException("Invalid account, no API key"));
		return await client.OpenSmartLock(smartLock.VendorSmartlockId);
	}
	
	/// <summary>
	/// Close the SmartLock
	/// </summary>
	/// <param name="hostId"></param>
	/// <param name="smartLockId"></param>
	/// <exception cref="BadHttpRequestException"></exception>
	[HttpGet("close")]
	public async Task<bool> CloseSmartLock(int hostId, int smartLockId)
	{
		// retrieve the account with the hostId from the vendor, if there is any throw code 400 
		var account = GetAuthenticatedAccount(hostId) ??
		              throw new BadHttpRequestException("No account associated to the vendor provided");
		// get the smartLock
		var smartLock = _context.DeviceList.FirstOrDefault(device => device.Id == smartLockId)
		                ?? throw new BadHttpRequestException($"No smartLock found with {smartLockId} id");
		// get the client
		var client =
			_clients.FirstOrDefault(clientE => clientE.Name == 
			                                   _context.DeviceVendorType.FirstOrDefault(vendor => vendor.Id 
					                                   == smartLock.DeviceVendorAccount.DeviceVendorId)
				                                   ?.Name) ??
			throw new BadHttpRequestException("Invalid smartlock vendor name");
		client.SetToken(account.Token ?? throw new BadHttpRequestException("Invalid account, no API key"));
		return await client.CloseSmartLock(smartLock.VendorSmartlockId);
	}
	

	/// <summary>
	/// Return an VendorAccount given host id and vendor name, if there is not it returns null
	/// </summary>
	/// <param name="hostId"></param>
	/// <returns></returns>
	public DeviceVendorAccount? GetAuthenticatedAccount(int hostId)
	{
		return _context.DeviceVendorAccountList
			.FirstOrDefault(vendorAccount => vendorAccount.HostId == hostId);
	}
}