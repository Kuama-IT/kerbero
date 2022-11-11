using Kerbero.Identity.Modules.Claims.Services;
using Kerbero.Identity.Modules.Claims.Utils;
using Kerbero.Identity.Library.Modules.Claims.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kerbero.Identity.Modules.Claims.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClaimsController : ControllerBase
{
  private readonly IClaimService _claimService;

  public ClaimsController(IClaimService claimService)
  {
    _claimService = claimService;
  }

  [HttpGet]
  [Authorize(Policy = PoliciesDefinition.Claims_GetAll)]
  public ActionResult<List<ClaimReadDto>> GetAll()
  {
    return _claimService.GetAll();
  }
}