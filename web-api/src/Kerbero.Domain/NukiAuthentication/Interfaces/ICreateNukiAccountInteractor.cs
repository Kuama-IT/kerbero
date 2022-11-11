using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Domain.NukiAuthentication.Models.PresentationRequests;
using Kerbero.Domain.NukiAuthentication.Models.PresentationResponses;

namespace Kerbero.Domain.NukiAuthentication.Interfaces;

public interface ICreateNukiAccountInteractor: InteractorAsync<NukiAccountPresentationRequest, NukiAccountPresentationResponse>
{
    
}