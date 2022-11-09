namespace Kerbero.Identity.Modules.Notifier.Events;

public record RoleCreateEvent(Guid RoleId);

public record RoleUpdateEvent(Guid RoleId);

public record RoleDeleteEvent(Guid RoleId);