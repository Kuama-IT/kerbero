using System.Web;
using Microsoft.AspNetCore.WebUtilities;

namespace KerberoWebApi.Utils
{
    public class UrlBuilder: UriBuilder
    {
        public string UrlBaseBuilder(string _baseUrl, string path, Dictionary<string, string?>? args = default)
        {
            Host = _baseUrl;
            Path = path;

            if(args != null && args.Count != 0)
            {
                var query = HttpUtility.ParseQueryString(Query);
                foreach(var arg in args)
                {
                    query.Add(arg.Key.ToString(), arg.Value != null ? arg.Value.ToString() : "");
                }
                Query = query.ToString();
            }
            return ToString();
        }

    }
}