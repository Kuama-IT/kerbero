namespace Clients{
    public interface IVendorClient
    {
        public abstract Task<bool> AuthorizeApi(string secret); 
    }
}