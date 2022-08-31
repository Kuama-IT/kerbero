using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using KerberoWebApi.Clients;

namespace KerberoWebApi.Controllers;

// This endpoint provides a list of smartlocks linked to the account
[ApiController]
[Route("[controller]")]
public class SmartlocksList : ControllerBase
{

    private readonly ILogger<SmartlocksList> _logger;

    public SmartlocksList(ILogger<SmartlocksList> logger)
    {
        _logger = logger;
        var token = "dammiunserviziocheaccedealdb".getToken();
        "ivendorservice<Nuki>".setToken(token);
    }

    [HttpGet(Name = "GetSmartlocksList")]
    public async Task<dynamic> Get()
    {
        "ivendorservice<nuki>".SmartlocksList();
        var nuki = new NukiApiClientTryOne("asdasd");
        var res = await nuki.SmartlocksList();
        return res;
    }


}
