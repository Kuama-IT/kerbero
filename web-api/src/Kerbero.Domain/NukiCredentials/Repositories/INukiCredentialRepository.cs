using FluentResults;
using Kerbero.Domain.NukiCredentials.Models;

namespace Kerbero.Domain.NukiCredentials.Repositories;

public interface INukiCredentialRepository
{
  Task<Result<NukiCredentialModel>> Create(NukiCredentialModel model, Guid userId);

  Task<Result<NukiCredentialModel>> CreateDraft(NukiCredentialDraftModel model);
  Task<Result<NukiCredentialModel>> GetById(int id);
  Task<Result<List<NukiCredentialModel>>> GetAllByUserId(Guid userId);
  Task<Result> ValidateApiToken(string apiToken);
}