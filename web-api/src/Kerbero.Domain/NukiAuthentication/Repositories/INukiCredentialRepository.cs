using FluentResults;
using Kerbero.Domain.NukiAuthentication.Models;

namespace Kerbero.Domain.NukiAuthentication.Repositories;

public interface INukiCredentialRepository
{
	Task<Result<NukiCredential>> Create(NukiCredential model,Guid userId);
	Task<Result<NukiCredential>> GetById(int id);
	Task<Result<NukiCredential>> Update(NukiCredential model);
	Task<Result<List<NukiCredential>>> GetAllByUserId(Guid userId);
}
