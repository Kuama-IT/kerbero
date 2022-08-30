using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
namespace KerberoWebApi.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class SmartlocksList : ControllerBase
{

    private readonly ILogger<SmartlocksList> _logger;

    public SmartlocksList(ILogger<SmartlocksList> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetSmartlocksList")]
    public async Task<dynamic> Get()
    {
        var nuki = new Clients.NukiApiClient();
        var res = await nuki.SmartlocksList();
        return res;
    }


}
