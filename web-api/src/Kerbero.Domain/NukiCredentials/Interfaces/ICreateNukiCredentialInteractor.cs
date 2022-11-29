using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.NukiCredentials.Dtos;

namespace Kerbero.Domain.NukiCredentials.Interfaces;

public interface ICreateNukiCredentialInteractor: InteractorAsync<CreateNukiCredentialParams, NukiCredentialDto>
{
    
}
