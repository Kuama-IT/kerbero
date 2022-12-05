using FluentResults;

namespace Kerbero.Domain.NukiCredentials.Models;

/// <summary>
/// All Nuki credentials owned by a given user
/// </summary>
/// <param name="NukiCredentials"></param>
/// <param name="OutdatedCredentials">Credentials that could not be refreshed</param>
public record UserNukiCredentialsModel(
  List<NukiCredentialModel> NukiCredentials,
  List<(NukiCredentialModel, List<IError>)> OutdatedCredentials);