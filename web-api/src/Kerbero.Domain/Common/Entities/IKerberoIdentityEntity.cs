using Kerbero.Identity.Modules.Users.Entities;

namespace Kerbero.Domain.Common.Entities;

public interface IKerberoIdentityEntity
{
	public Guid UserId { get; set; }
	public User User { get; set; }
}
