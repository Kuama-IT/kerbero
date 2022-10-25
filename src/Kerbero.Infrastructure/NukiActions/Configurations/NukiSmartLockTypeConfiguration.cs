using Kerbero.Domain.NukiActions.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kerbero.Infrastructure.NukiActions.Configurations;

public class NukiSmartLockTypeConfiguration: IEntityTypeConfiguration<NukiSmartLock>
{
	public void Configure(EntityTypeBuilder<NukiSmartLock> builder)
	{
		builder
			.HasOne(c => c.State)
			.WithOne(c => c.NukiSmartLock);
	}
}
