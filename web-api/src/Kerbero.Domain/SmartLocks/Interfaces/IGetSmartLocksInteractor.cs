using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.SmartLocks.Dtos;
using Kerbero.Domain.SmartLocks.Params;

namespace Kerbero.Domain.SmartLocks.Interfaces;

public interface IGetSmartLocksInteractor : InteractorAsync<GetSmartLocksInteractorParams, List<SmartLockDto>>
{
}