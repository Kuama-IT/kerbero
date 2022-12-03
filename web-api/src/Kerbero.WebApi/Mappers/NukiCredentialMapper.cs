using FluentResults;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.WebApi.Dtos;

namespace Kerbero.WebApi.Mappers;

public static class NukiCredentialMapper
{
  public static NukiCredentialDraftDto Map(Uri uri)
  {
    return new NukiCredentialDraftDto(RedirectUrl: uri.ToString());
  }
}