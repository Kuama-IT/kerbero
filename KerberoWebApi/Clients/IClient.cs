namespace Clients{
    // Basic interface for clients, which communicate with Smartlocks.
    public interface IVendorClient
    {
        public abstract Task<bool> AuthorizeApi(string secret); 
        public abstract Task<List<string>> RetrieveTokens(string clientId, string clientSecret, string code);
    }
}