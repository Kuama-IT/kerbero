namespace Clients{
    public interface IVendorClient
    {
        public abstract Task<string> AuthorizeApi(string secret); 
    }
}