namespace Clients {
    using System.Net.Http.Headers;
    using Microsoft.AspNetCore.WebUtilities;
    using System = global::System;
    public class NukiApiClient: Clients.IVendorClient
    {
        private string _baseUrl = "https://api.nuki.io/";
        private System.Net.Http.HttpClient _httpClient;
        // private System.Lazy<Newtonsoft.Json.JsonSerializerSettings> _settings;

        public NukiApiClient()
        {
            _httpClient = new HttpClient();
            // Bearer 07fdd3f0d724f8c3b8afc2cd8bd5fa679f2c918949a711805f17984dc4957da0506ca839c61eb052
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "07fdd3f0d724f8c3b8afc2cd8bd5fa679f2c918949a711805f17984dc4957da0506ca839c61eb052");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            // _settings = new System.Lazy<Newtonsoft.Json.JsonSerializerSettings>(CreateSerializerSettings);
        }

        public string BaseUrl
        {
            get { return _baseUrl; }
            set { _baseUrl = value; }
        }

        public string UrlBaseBuilder(string path, Dictionary<string, string?>? args = default)
        {
            var uriBuilder = new UriBuilder();
            uriBuilder.Host = _baseUrl;
            uriBuilder.Path = path;

            if(args != null && args.Count != 0)
            {
                uriBuilder = new UriBuilder(QueryHelpers.AddQueryString(uriBuilder.ToString(), args));
            }
            return uriBuilder.ToString();
        }

        // Base method for Nuki GET calls
        protected virtual async Task<string> GetAsyncBase(string path) {
            var url = UrlBaseBuilder(path);
            string responseBody = "";
            try	
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                responseBody = await response.Content.ReadAsStringAsync();

                Console.WriteLine(responseBody);
            }
            catch(HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");	
                Console.WriteLine("Message :{0} ",e.Message);
            }
            return responseBody;
        } 

        public virtual async Task<string> SmartlocksList()
        {
            return await GetAsyncBase("/smartlock");
        }

        public virtual async Task<string> AuthorizeApi(string clientSecret)
        {
            // use a url builder
            var urlBuilder = new System.Text.StringBuilder();
            urlBuilder.Append(UrlBaseBuilder("/oauth/authorize"))
                .Append("?response_type=code&redirect_uri=https%3A%2F%2Ftest.com%3A5220%2FVendorAuthorization%2Fauth&client_id=")
                .Append(clientSecret)
                .Append("&scope=account%20notification%20smartlock%20smartlock.readOnly%20smartlock.action%20smartlock.auth%20smartlock.config%20smartlock.log");
            var url = urlBuilder.ToString();
            var unAuthClient = new HttpClient();
            return url;
        }

        public virtual async Task<List<string>> RetrieveTokens(string clientId, string clientSecret, string code)
        {
            var returnTokens = new List<string>();
            var responseBody = "";
            var url = "";
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                responseBody = await response.Content.ReadAsStringAsync();
            }
            catch(HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");	
                Console.WriteLine("Message :{0} ",e.Message);
            }
            return returnTokens;
        }
    }
}