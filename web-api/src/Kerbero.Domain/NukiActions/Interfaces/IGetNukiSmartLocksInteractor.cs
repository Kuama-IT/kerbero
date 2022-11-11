using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.NukiActions.Models.PresentationRequest;
using Kerbero.Domain.NukiActions.Models.PresentationResponse;

namespace Kerbero.Domain.NukiActions.Interfaces;

public interface IGetNukiSmartLocksInteractor: InteractorAsync<NukiSmartLocksPresentationRequest, List<KerberoSmartLockPresentationResponse>>
{
    
}