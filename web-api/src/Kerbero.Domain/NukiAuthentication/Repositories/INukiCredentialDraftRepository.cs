using FluentResults;
using Kerbero.Domain.NukiAuthentication.Models;

namespace Kerbero.Domain.NukiAuthentication.Repositories;

public interface INukiCredentialDraftRepository
{
  Task<Result> Create(NukiCredentialDraft nukiCredentialDraft);

  Task<Result<NukiCredentialDraft>> GetByClientId(string clientId);
  
  Task<Result> DeleteByClientId(string clientId);
}