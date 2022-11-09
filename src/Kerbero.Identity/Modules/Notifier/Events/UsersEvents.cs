namespace Kerbero.Identity.Modules.Notifier.Events;

public record UserCreateEvent(Guid UserId);

public record UserUpdateEvent(Guid UserId);

public record UserDeleteEvent(Guid UserId);