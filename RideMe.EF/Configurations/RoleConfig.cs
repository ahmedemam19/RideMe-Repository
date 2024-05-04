using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RideMe.Core.Models;

namespace RideMe.EF.Configurations
{
	internal class RoleConfig : IEntityTypeConfiguration<Role>
	{
		public void Configure(EntityTypeBuilder<Role> builder)
		{
			/// builder.HasKey(e => e.Id).HasName("PK__role__3213E83FCC11FAE3");
			/// 
			/// builder.ToTable("role");
			/// 
			/// builder.Property(e => e.Id).HasColumnName("id");
			/// builder.Property(e => e.Name)
			/// 	.HasMaxLength(50)
			/// 	.HasColumnName("name");



			builder.Property(e => e.Name)
				.HasMaxLength(50);
		}
	}
}
