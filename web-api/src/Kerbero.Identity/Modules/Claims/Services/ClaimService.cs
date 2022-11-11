using Kerbero.Identity.Modules.Claims.Mappings;
using Kerbero.Identity.Library.Modules.Claims.Dtos;

namespace Kerbero.Identity.Modules.Claims.Services;

class ClaimService : IClaimService
{
  private readonly IClaimManager _claimManager;

  public ClaimService(IClaimManager claimManager)
  {
    _claimManager = claimManager;
  }

  public List<ClaimReadDto> GetAll()
  {
    var entities = _claimManager.GetAll();
    return ClaimMappings.Map(entities);
  }
}