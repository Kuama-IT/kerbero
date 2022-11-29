using FluentResults;
using Kerbero.Domain.NukiAuthentication.Models;

namespace Kerbero.Domain.NukiAuthentication.Repositories;

public interface INukiCredentialRepository
{
	Task<Result<NukiCredentialModel>> Create(NukiCredentialModel model, Guid userId);
	Task<Result<NukiCredentialModel>> GetById(int id);
	Task<Result<NukiCredentialModel>> Update(NukiCredentialModel model);
	Task<Result<List<NukiCredentialModel>>> GetAllByUserId(Guid userId);
}
