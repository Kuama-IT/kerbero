using Kerbero.Domain.SmartLockKeys.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kerbero.Infrastructure.SmartLockKeys.Configurations;

public class SmartLockKeyEntityConfiguration: IEntityTypeConfiguration<SmartLockKey>
{
	public void Configure(EntityTypeBuilder<SmartLockKey> builder)
	{
		builder
			.HasOne(c => c.NukiSmartLock)
			.WithMany();
	}
}
