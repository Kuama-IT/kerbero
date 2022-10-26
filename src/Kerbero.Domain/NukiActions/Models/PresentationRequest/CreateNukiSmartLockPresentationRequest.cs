namespace Kerbero.Domain.NukiActions.Models.PresentationRequest;

public record CreateNukiSmartLockPresentationRequest(string AccessToken, int AccountId, int NukiSmartLockId);