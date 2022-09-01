using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using KerberoWebApi.Clients;

namespace KerberoWebApi.Controllers;

// This endpoint provides a list of smartlocks linked to the account
[ApiController]
[Route("smartlock/list")]
public class SmartlocksList : ControllerBase
{

    private readonly ILogger<SmartlocksList> _logger;

    private readonly string token;

    public SmartlocksList(ILogger<SmartlocksList> logger)
    {
        _logger = logger;
        // token = "dammiunserviziocheaccedealdb".getToken();
        // "ivendorservice<Nuki>".setToken(token);
    }

    [HttpGet]
    public async Task<dynamic> Get()
    {
        // "ivendorservice<nuki>".SmartlocksList();
        var nuki = new NukiClient();
        var res = await nuki.GetSmartLocks();
        return JsonSerializer.Serialize(res.SmartlockList).ToString();
    }


}
