using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.NukiAuthentication.Models;

namespace Kerbero.Domain.NukiAuthentication.Interfaces;

public interface ICreateNukiAccountInteractor: InteractorAsync<NukiAccountExternalRequestDto, NukiAccountPresentationDto>
{
    
}