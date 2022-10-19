using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.Common.Models;

namespace Kerbero.Domain.NukiActions.Interfaces;

public interface IGetNukiSmartLockListInteractor: InteractorAsyncNoParam<List<KerberoSmartLockPresentationDto>>
{
    
}