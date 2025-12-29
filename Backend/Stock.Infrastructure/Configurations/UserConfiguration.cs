using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stock.Infrastructure.Entities;

namespace Stock.Infrastructure.Configurations
{
	public class UserConfiguration : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			builder.ToTable("Users");

			builder.HasKey(x => x.Id);

			builder.Property(x => x.Username)
				.IsRequired()
				.HasMaxLength(50);

			builder.HasIndex(x => x.Username).IsUnique();

			builder.Property(x => x.PasswordHash)
				.IsRequired()
				.HasMaxLength(255);

			builder.Property(x => x.Role)
				.HasConversion<string>() 
				.IsRequired();

			builder.Property(x => x.IsActive).IsRequired();
			builder.Property(x => x.CreatedAt).IsRequired();
		}
	}
}

