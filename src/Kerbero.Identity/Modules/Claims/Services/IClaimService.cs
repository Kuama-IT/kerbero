using Kerbero.Identity.Library.Modules.Claims.Dtos;

namespace Kerbero.Identity.Modules.Claims.Services;

public interface IClaimService
{
  List<ClaimReadDto> GetAll();
}