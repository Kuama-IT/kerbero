using Kerbero.Domain.NukiAuthentication.Dtos;
using Kerbero.Domain.NukiAuthentication.Models;

namespace Kerbero.Domain.NukiAuthentication.Mappers;

public static class NukiCredentialDraftMapper
{
  public static NukiCredentialDraftDto Map(NukiCredentialDraft nukiCredentialDraft)
  {
    return new NukiCredentialDraftDto(
      ClientId: nukiCredentialDraft.ClientId,
      UserId: nukiCredentialDraft.UserId,
      RedirectUrl: nukiCredentialDraft.RedirectUrl
    );
  }
}