using Kerbero.Domain.NukiActions.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kerbero.Infrastructure.NukiActions.Configurations;

public class NukiSmartLockStateTypeConfiguration: IEntityTypeConfiguration<NukiSmartLockState>
{
	public void Configure(EntityTypeBuilder<NukiSmartLockState> builder)
	{
		builder
			.HasOne(c => c.NukiSmartLock)
			.WithOne(c => c.State)
			.HasForeignKey<NukiSmartLockState>(p => p.NukiSmartLockId);
		builder
			.HasKey(p => p.NukiSmartLockId);
	}
}
