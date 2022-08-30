
using Clients;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class VendorAuthorizationController : ControllerBase
{

    private readonly ILogger<VendorAuthorizationController> _logger;

    public VendorAuthorizationController(ILogger<VendorAuthorizationController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Ask the authorization for the specific vendor client
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [HttpGet(Name = "AuthorizeApiCalls")]
    public async Task<RedirectResult> Get(string name)
    {
        IVendorClient client;
        string clientId = "";
        string clientSecret = "";
        if(name == "nuki")
        {
            client = new Clients.NukiApiClient();
            HttpContext.Session.SetString("clientId", clientId);
            HttpContext.Session.SetString("clientSecret", clientSecret);
        }
        else
            client = new Clients.NukiApiClient();

        var res = await client.AuthorizeApi("N-rzp1z8UdSa6MPYEM_1wA");
        return new RedirectResult(res);
    }

    /// <summary>
    /// Callback method for nuki to receive the authentication code
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    [HttpGet("auth")]
    public async Task<RedirectResult> CallbackAuth(string code)
    {
        // code=iaQ1W3l8ixV5-Ch2BLDxce4q9HCouWEt09V_UVkanGU.4GF6FJQfzZMzOuKSQotB0SPI8Ra8JG86f_TZhEOdbSU
        // scope=account+notification+smartlock+smartlock.readOnly+smartlock.action+smartlock.auth+smartlock.config+smartlock.log+offline_access
        // state=s6XDF5Rtf3uqxFb4iACChkNkixsxsmaR
        string clientId = HttpContext.Session.GetString("clientId") ?? throw new BadHttpRequestException("No ClientId provided", 400);
        string clientSecret = HttpContext.Session.GetString("clientSecret") ?? throw new BadHttpRequestException("No ClientSecret provided", 400);
        
        return new RedirectResult("http://localhost:5220/swagger/index.html");
    }
}