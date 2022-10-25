namespace Kerbero.Domain.NukiActions.Models.PresentationRequest;

public record CloseNukiSmartLockPresentationRequest(string AccessToken, int SmartLockId);
