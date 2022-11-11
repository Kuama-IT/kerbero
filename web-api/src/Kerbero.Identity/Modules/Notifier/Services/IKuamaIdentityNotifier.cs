using Kerbero.Identity.Modules.Notifier.Events;

namespace Kerbero.Identity.Modules.Notifier.Services;

public interface IKerberoIdentityNotifier
{
  event Func<UserCreateEvent, Task>? OnUserCreate;
  void EmitUserCreateEvent(UserCreateEvent @event);
  event Func<UserUpdateEvent, Task>? OnUserUpdate;
  void EmitUserUpdateEvent(UserUpdateEvent @event);
  event Func<UserDeleteEvent, Task>? OnUserDelete;
  void EmitUserDeleteEvent(UserDeleteEvent @event);
  event Func<RoleCreateEvent, Task>? OnRoleCreate;
  void EmitRoleCreateEvent(RoleCreateEvent @event);
  event Func<RoleUpdateEvent, Task>? OnRoleUpdate;
  void EmitRoleUpdateEvent(RoleUpdateEvent @event);
  event Func<RoleDeleteEvent, Task>? OnRoleDelete;
  void EmitRoleDeleteEvent(RoleDeleteEvent @event);
}