using Kerbero.Identity.Modules.Roles.Entities;
using Kerbero.Identity.Modules.Users.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Kerbero.Identity.Common;

public abstract class KerberoIdentityDbContext : IdentityDbContext<User, Role, Guid>
{
  protected KerberoIdentityDbContext(DbContextOptions options) : base(options)
  {
  }
}


