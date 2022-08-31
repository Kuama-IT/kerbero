// Basic interface for vendor api clients, used to communicate with vendor's Smart locks.
namespace KerberoWebApi.Clients;

public interface IVendorClient
{
  // public Task<bool> Authenticate(string secret);
  // public Task<List<string>> RetrieveTokens(string clientId, string clientSecret, string code);

  /// <summary>
  /// Retrieves all the Smart locks of a specific vendor
  /// </summary>
  /// <returns></returns>
  public Task GetSmartLocks();

  public Task OpenSmartLock();
}