using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RideMe.Core.Models;

namespace RideMe.EF.Configurations
{
	internal class AdminConfig : IEntityTypeConfiguration<Admin>
	{
		public void Configure(EntityTypeBuilder<Admin> builder)
		{
			/// builder.HasKey(e => e.Id).HasName("PK__admin__3213E83F0D86F63E");
			/// 
			/// builder.ToTable("admin");
			/// 
			/// builder.Property(e => e.Id).HasColumnName("id");
			/// builder.Property(e => e.Email)
			/// 	.HasMaxLength(50)
			/// 	.HasColumnName("email");
			/// builder.Property(e => e.Name)
			/// 	.HasMaxLength(50)
			/// 	.HasColumnName("name");
			/// builder.Property(e => e.Password)
			/// 	.HasMaxLength(20)
			/// 	.HasColumnName("password");



			builder.Property(e => e.Email).HasMaxLength(50);
			builder.Property(e => e.Name).HasMaxLength(50);
			builder.Property(e => e.Password).HasMaxLength(200);

		}
	}
}
