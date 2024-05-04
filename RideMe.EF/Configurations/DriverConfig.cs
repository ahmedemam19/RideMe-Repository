using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RideMe.Core.Models;

namespace RideMe.EF.Configurations
{
	internal class DriverConfig : IEntityTypeConfiguration<Driver>
	{
		public void Configure(EntityTypeBuilder<Driver> builder)
		{
			/// builder.HasKey(e => e.Id).HasName("PK__driver__3213E83F9DA00124");
			/// 
			/// builder.ToTable("driver");
			/// 
			/// builder.HasIndex(e => e.UserId, "UQ__driver__B9BE370EDB777182").IsUnique();
			/// 
			/// builder.Property(e => e.Id).HasColumnName("id");
			/// builder.Property(e => e.Available).HasColumnName("available");
			/// builder.Property(e => e.AvgRating).HasColumnName("avg_rating");
			/// builder.Property(e => e.CarType)
			/// 	.HasMaxLength(50)
			/// 	.HasColumnName("car_type");
			/// builder.Property(e => e.CityId).HasColumnName("city_id");
			/// builder.Property(e => e.Region)
			/// 	.HasMaxLength(100)
			/// 	.HasColumnName("region");
			/// builder.Property(e => e.Smoking).HasColumnName("smoking");
			/// builder.Property(e => e.UserId).HasColumnName("user_id");
			/// 
			/// builder.HasOne(d => d.City).WithMany(p => p.Drivers)
			/// 	.HasForeignKey(d => d.CityId)
			/// 	.HasConstraintName("FK__driver__city_id__46E78A0C");
			/// 
			/// builder.HasOne(d => d.User).WithOne(p => p.Driver)
			/// 	.HasForeignKey<Driver>(d => d.UserId)
			/// 	.OnDelete(DeleteBehavior.Cascade)
			/// 	.HasConstraintName("FK__driver__user_id__45F365D3");
			/// 	



			builder.HasIndex(e => e.UserId).IsUnique();

			builder.Property(e => e.CarType).HasMaxLength(50);

			builder.Property(e => e.Region).HasMaxLength(100);

			builder.HasOne(d => d.City)
				.WithMany(p => p.Drivers)
				.HasForeignKey(d => d.CityId);

			builder.HasOne(d => d.User).WithOne(p => p.Driver)
				.HasForeignKey<Driver>(d => d.UserId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
