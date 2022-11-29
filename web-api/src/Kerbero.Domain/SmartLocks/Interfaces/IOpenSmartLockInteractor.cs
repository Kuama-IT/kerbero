using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.NukiActions.Models.PresentationRequest;
using Kerbero.Domain.SmartLocks.Dtos;

namespace Kerbero.Domain.SmartLocks.Interfaces;

public interface IOpenSmartLockInteractor : InteractorAsync<OpenSmartLockParams>
{
}