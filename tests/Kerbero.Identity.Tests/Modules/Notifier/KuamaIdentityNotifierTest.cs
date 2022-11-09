using System;
using System.Threading.Tasks;
using FluentAssertions;
using Kerbero.Identity.Modules.Notifier.Events;
using Kerbero.Identity.Modules.Notifier.Services;
using Xunit;

namespace Kerbero.Identity.Tests.Modules.Notifier;

public class KerberoIdentityNotifierTest
{
  private readonly KerberoIdentityNotifier _kerberoIdentityNotifier;

  public KerberoIdentityNotifierTest()
  {
    _kerberoIdentityNotifier = new KerberoIdentityNotifier();
  }

  #region Users

  [Fact]
  public void EmitUserCreateEvent_InvokeCorrectEvent()
  {
    var tGuid = Guid.NewGuid();
    var tEvent = new UserCreateEvent(tGuid);

    _kerberoIdentityNotifier.OnUserCreate += @event =>
    {
      @event.Should().BeEquivalentTo(tEvent);
      return Task.CompletedTask;
    };

    _kerberoIdentityNotifier.EmitUserCreateEvent(tEvent);
  }

  [Fact]
  public void EmitUserUpdateEvent_InvokeCorrectEvent()
  {
    var tGuid = Guid.NewGuid();
    var tEvent = new UserUpdateEvent(tGuid);

    _kerberoIdentityNotifier.OnUserUpdate += @event =>
    {
      @event.Should().BeEquivalentTo(tEvent);
      return Task.CompletedTask;
    };

    _kerberoIdentityNotifier.EmitUserUpdateEvent(tEvent);
  }

  [Fact]
  public void EmitUserDeleteEvent_InvokeCorrectEvent()
  {
    var tGuid = Guid.NewGuid();
    var tEvent = new UserDeleteEvent(tGuid);

    _kerberoIdentityNotifier.OnUserDelete += @event =>
    {
      @event.Should().BeEquivalentTo(tEvent);
      return Task.CompletedTask;
    };

    _kerberoIdentityNotifier.EmitUserDeleteEvent(tEvent);
  }

  #endregion

  #region Roles

  [Fact]
  public void EmitRoleCreateEvent_InvokeCorrectEvent()
  {
    var tGuid = Guid.NewGuid();
    var tEvent = new RoleCreateEvent(tGuid);

    _kerberoIdentityNotifier.OnRoleCreate += @event =>
    {
      @event.Should().BeEquivalentTo(tEvent);
      return Task.CompletedTask;
    };
    
    _kerberoIdentityNotifier.EmitRoleCreateEvent(tEvent);
  }
  
  [Fact]
  public void EmitRoleUpdateEvent_InvokeCorrectEvent()
  {
    var tGuid = Guid.NewGuid();
    var tEvent = new RoleUpdateEvent(tGuid);

    _kerberoIdentityNotifier.OnRoleUpdate += @event =>
    {
      @event.Should().BeEquivalentTo(tEvent);
      return Task.CompletedTask;
    };
    
    _kerberoIdentityNotifier.EmitRoleUpdateEvent(tEvent);
  }
  
  [Fact]
  public void EmitRoleDeleteEvent_InvokeCorrectEvent()
  {
    var tGuid = Guid.NewGuid();
    var tEvent = new RoleDeleteEvent(tGuid);

    _kerberoIdentityNotifier.OnRoleDelete += @event =>
    {
      @event.Should().BeEquivalentTo(tEvent);
      return Task.CompletedTask;
    };
    
    _kerberoIdentityNotifier.EmitRoleDeleteEvent(tEvent);
  }
  

  #endregion
}