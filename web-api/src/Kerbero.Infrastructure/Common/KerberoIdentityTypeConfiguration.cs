using Kerbero.Domain.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kerbero.Infrastructure.Common;

public class KerberoIdentityTypeConfiguration<T>: IEntityTypeConfiguration<T>
	where T: class, IKerberoIdentityEntity
{
	public void Configure(EntityTypeBuilder<T> builder)
	{
		builder
			.HasOne(p => p.User)
			.WithMany()
			.HasForeignKey(p => p.UserId);
	}
}
