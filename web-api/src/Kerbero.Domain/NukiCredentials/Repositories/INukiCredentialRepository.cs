using FluentResults;
using Kerbero.Domain.NukiCredentials.Models;

namespace Kerbero.Domain.NukiCredentials.Repositories;

public interface INukiCredentialRepository
{
  Task<Result<NukiCredentialModel>> Create(NukiCredentialModel model, Guid userId);
  
  Task<Result<NukiCredentialModel>> GetById(int id);
  
  Task<Result<List<NukiCredentialModel>>> GetAllByUserId(Guid userId);
  
  Task<Result> ValidateNotRefreshableApiToken(string apiToken);
  
  Task<Result<NukiCredentialModel>> CreateDraft(NukiCredentialDraftModel model);
  
  Task<Result<NukiCredentialDraftModel>> GetDraftCredentialsByUserId(Guid userId);
  
  Task<Result<NukiRefreshableCredentialModel>> GetRefreshableCredential(string oAuthCode, string redirectUri);
  
  Task<Result<NukiCredentialModel>> ConfirmDraft(NukiCredentialDraftModel draft, NukiRefreshableCredentialModel model);

  Task<Result> DeleteDraftByUserId(Guid userId);
}