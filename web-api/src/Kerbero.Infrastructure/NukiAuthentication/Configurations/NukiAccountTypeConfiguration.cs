using Kerbero.Domain.NukiAuthentication.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kerbero.Infrastructure.NukiAuthentication.Configurations;

public class NukiAccountTypeConfiguration: IEntityTypeConfiguration<NukiAccount>
{
	public void Configure(EntityTypeBuilder<NukiAccount> builder)
	{
		builder
			.HasIndex(a => a.ClientId)
			.IsUnique();
	}
}
