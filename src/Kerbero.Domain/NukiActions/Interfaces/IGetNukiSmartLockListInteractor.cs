using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.NukiActions.Models.PresentationRequest;
using Kerbero.Domain.NukiActions.Models.PresentationResponse;

namespace Kerbero.Domain.NukiActions.Interfaces;

public interface IGetNukiSmartLockListInteractor: InteractorAsync<NukiSmartLocksPresentationRequest, List<KerberoSmartLockPresentationResponse>>
{
    
}