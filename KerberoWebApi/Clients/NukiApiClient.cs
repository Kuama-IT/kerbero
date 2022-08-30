namespace Clients {
    using System.Net.Http.Headers;
    using KerberoWebApi.Utils;
    using Microsoft.AspNetCore.WebUtilities;
    using System = global::System;
    public class NukiApiClient: Clients.IVendorClient
    {
        private string _baseUrl = "api.nuki.io";
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


        // Base method for Nuki GET calls
        protected virtual async Task<string> GetAsyncBase(string path) {
            var url = new UrlBuilder().UrlBaseBuilder(_baseUrl, path);
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

        public virtual async Task<bool> AuthorizeApi(string clientSecret)
        {
            var query = new Dictionary<string, string>();
            query["response_type"] = "code";
            query["client_id"] = clientSecret;
            query["redirect_uri"] = "https://test.com:5220/VendorAuthorization/auth";
            query["scope"] = $"account notification smartlock smartlock.readOnly smartlock.action smartlock.auth smartlock.config smartlock.log";
            // use a url builder
            UrlBuilder urlBuilder = new UrlBuilder();
            urlBuilder.Scheme = "https";
            var url = urlBuilder.UrlBaseBuilder(_baseUrl, "/oauth/authorize", query);
            var unAuthClient = new HttpClient();
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
            }
            catch(HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");	
                Console.WriteLine("Message :{0} ",e.Message);
                return false;
            }
            return true;
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