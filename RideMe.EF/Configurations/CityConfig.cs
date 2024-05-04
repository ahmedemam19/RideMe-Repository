using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RideMe.Core.Models;

namespace RideMe.EF.Configurations
{
	internal class CityConfig : IEntityTypeConfiguration<City>
	{
		public void Configure(EntityTypeBuilder<City> builder)
		{
			/// builder.HasKey(e => e.Id).HasName("PK__city__3213E83FBCB89D78");
			/// 
			/// builder.ToTable("city");
			/// 
			/// builder.Property(e => e.Id).HasColumnName("id");
			/// builder.Property(e => e.Name)
			/// 	.HasMaxLength(50)
			/// 	.HasColumnName("name");



			builder.Property(e => e.Name).HasMaxLength(50);

		}
	}
}
