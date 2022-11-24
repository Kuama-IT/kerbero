using Kerbero.Domain.NukiAuthentication.Entities;
using Kerbero.Infrastructure.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kerbero.Infrastructure.NukiAuthentication.Configurations;

public class NukiAccountTypeConfiguration: KerberoIdentityTypeConfiguration<NukiAccount>
{
	public new void Configure(EntityTypeBuilder<NukiAccount> builder)
	{
		builder
			.HasIndex(a => a.ClientId)
			.IsUnique();
	}
}
