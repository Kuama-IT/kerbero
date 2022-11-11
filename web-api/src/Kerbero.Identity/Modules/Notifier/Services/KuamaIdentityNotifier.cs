using Kerbero.Identity.Modules.Notifier.Events;

namespace Kerbero.Identity.Modules.Notifier.Services;

public class KerberoIdentityNotifier : IKerberoIdentityNotifier
{
  #region Users

  public event Func<UserCreateEvent, Task>? OnUserCreate;

  public void EmitUserCreateEvent(UserCreateEvent @event)
  {
    OnUserCreate?.Invoke(@event);
  }

  public event Func<UserUpdateEvent, Task>? OnUserUpdate;

  public void EmitUserUpdateEvent(UserUpdateEvent @event)
  {
    OnUserUpdate?.Invoke(@event);
  }

  public event Func<UserDeleteEvent, Task>? OnUserDelete;

  public void EmitUserDeleteEvent(UserDeleteEvent @event)
  {
    OnUserDelete?.Invoke(@event);
  }

  #endregion

  #region Roles

  public event Func<RoleCreateEvent, Task>? OnRoleCreate;

  public void EmitRoleCreateEvent(RoleCreateEvent @event)
  {
    OnRoleCreate?.Invoke(@event);
  }

  public event Func<RoleUpdateEvent, Task>? OnRoleUpdate;

  public void EmitRoleUpdateEvent(RoleUpdateEvent @event)
  {
    OnRoleUpdate?.Invoke(@event);
  }

  public event Func<RoleDeleteEvent, Task>? OnRoleDelete;

  public void EmitRoleDeleteEvent(RoleDeleteEvent @event)
  {
    OnRoleDelete?.Invoke(@event);
  }

  #endregion
}