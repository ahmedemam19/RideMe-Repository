using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RideMe.Core.Models;

namespace RideMe.EF.Configurations
{
	internal class UserStatusConfig : IEntityTypeConfiguration<UserStatus>
	{
		public void Configure(EntityTypeBuilder<UserStatus> builder)
		{
			/// builder.HasKey(e => e.Id).HasName("PK__user_sta__3213E83FBAABFCF6");
			/// 
			/// builder.ToTable("user_status");
			/// 
			/// builder.Property(e => e.Id).HasColumnName("id");
			/// builder.Property(e => e.Name)
			/// 				.HasMaxLength(50)
			/// 				.HasColumnName("name");


			builder.Property(e => e.Name).HasMaxLength(50);
		}
	}
}
