using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.NukiActions.Models.PresentationRequest;

namespace Kerbero.Domain.NukiActions.Interfaces;

public interface IGetNukiSmartLockListInteractor: InteractorAsyncNoParam<List<KerberoSmartLockPresentationRequest>>
{
    
}