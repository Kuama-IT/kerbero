using FluentResults;
using Kerbero.Domain.NukiAuthentication.Models;

namespace Kerbero.Domain.NukiAuthentication.Repositories;

public interface INukiCredentialRepository
{
	Task<Result<NukiCredential>> Create(NukiCredential model);
	Task<Result<NukiCredential>> GetById(int id);
	Task<Result<NukiCredential>> Update(NukiCredential model);
}
