using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.NukiAuthentication.Dtos;
using Kerbero.Domain.NukiAuthentication.Models;

namespace Kerbero.Domain.NukiAuthentication.Interfaces;

public interface ICreateNukiAccountInteractor: InteractorAsync<CreateNukiCredentialParams, NukiCredentialDto>
{
    
}
