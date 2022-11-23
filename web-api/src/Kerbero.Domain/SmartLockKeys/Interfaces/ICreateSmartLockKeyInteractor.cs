using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.SmartLockKeys.Dtos;

namespace Kerbero.Domain.SmartLockKeys.Interfaces;

public interface ICreateSmartLockKeyInteractor: InteractorAsync<CreateSmartLockKeyParams, SmartLockKeyDto>
{
	
}
