using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.WebApi.Dto;

namespace Kerbero.WebApi.Mappers;

public static class NukiCredentialMapper
{
  public static NukiCredentialDraftDto Map(NukiCredentialDraftModel model)
  {
    return new NukiCredentialDraftDto(RedirectUrl: model.RedirectUrl);
  }
}