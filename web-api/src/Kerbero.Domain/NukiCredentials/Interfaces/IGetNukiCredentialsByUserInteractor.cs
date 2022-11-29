using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.NukiCredentials.Dtos;

namespace Kerbero.Domain.NukiCredentials.Interfaces;

public interface
  IGetNukiCredentialsByUserInteractor : InteractorAsync<GetNukiCredentialsByUserInteractorParams,
    List<NukiCredentialDto>>
{
}