using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RideMe.Core.Models;

namespace RideMe.EF.Configurations
{
	internal class RideStatusConfig : IEntityTypeConfiguration<RideStatus>
	{
		public void Configure(EntityTypeBuilder<RideStatus> builder)
		{
			/// builder.HasKey(e => e.Id).HasName("PK__ride_sta__3213E83F8AA70FC9");
			/// 
			/// builder.ToTable("ride_status");
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
