
using KerberoWebApi.Clients;
using Microsoft.AspNetCore.Mvc;

// This controller provides endpoint to authenticate with the vendor API and pair with the Smartlock
[ApiController]
[Route("[controller]")]
public class VendorAuthorizationController : ControllerBase
{

    private readonly ILogger<VendorAuthorizationController> _logger;

    /// <summary>
    /// Initialize controller logger
    /// </summary>
    /// <param name="logger"></param>
    /// <returns></returns>
    public VendorAuthorizationController(ILogger<VendorAuthorizationController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Ask the authorization for the specific vendor client
    /// </summary>
    /// <param name="name"></param>
    /// <param name="clientId"></param>
    /// <param name="clientSecret"></param>
    /// <returns></returns>
    [HttpGet(Name = "AuthorizeApiCalls")]
    public async Task<bool> Get(string name, string clientId, string clientSecret)
    {
        // IVendorClient client;
        // if(name == "nuki")
        // {
        //     client = new NukiApiClientTryOne();
        //     HttpContext.Session.SetString("clientId", clientId);
        //     HttpContext.Session.SetString("clientSecret", clientSecret);
        // }
        // else
        //     client = new Clients.NukiApiClient();
        //
        // var res = await client.Authenticate("N-rzp1z8UdSa6MPYEM_1wA");
        // return res;
        return true;
    }

    /// <summary>
    /// Callback method for nuki to receive the authentication code
    /// </summary>
    /// <param name="code"></param>
    /// <param name="scope"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    // [HttpGet("auth")]
    // public async Task<RedirectResult> CallbackAuth(string code, string scope, string state)
    // {
    //     string clientId = HttpContext.Session.GetString("clientId") ?? throw new BadHttpRequestException("No ClientId provided", 400);
    //     string clientSecret = HttpContext.Session.GetString("clientSecret") ?? throw new BadHttpRequestException("No ClientSecret provided", 400);
    //     // da istanziare il client di nuki o di un vendor generico
    //     IVendorClient client = new NukiApiClient();
    //     await client.RetrieveTokens(clientId, clientSecret, code);
    //     
    //     return new RedirectResult("http://localhost:5220/swagger/index.html");
    // }
}