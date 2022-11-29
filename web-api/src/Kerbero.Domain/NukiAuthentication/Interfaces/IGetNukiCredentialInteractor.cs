using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.NukiAuthentication.Dtos;

namespace Kerbero.Domain.NukiAuthentication.Interfaces;

public interface IGetNukiCredentialInteractor : InteractorAsync<GetNukiCredentialParams, NukiCredentialDto>
{
}