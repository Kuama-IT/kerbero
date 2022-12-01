using FluentResults;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.SmartLockKeys.Models;

namespace Kerbero.Domain.SmartLockKeys.Repositories;

public interface ISmartLockKeyRepository
{
	Task<Result<SmartLockKeyModel>> Create(SmartLockKeyModel model);
	Task<Result<List<SmartLockKeyModel>>> GetAllByCredentials(List<NukiCredentialModel> credentials);
}
