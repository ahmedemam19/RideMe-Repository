using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RideMe.Core.Models;

namespace RideMe.EF.Configurations
{
	internal class RideConfig : IEntityTypeConfiguration<Ride>
	{
		public void Configure(EntityTypeBuilder<Ride> builder)
		{
			/// builder.HasKey(e => e.Id).HasName("PK__ride__3213E83FDB858685");
			/// 
			/// builder.ToTable("ride");
			/// 
			/// builder.Property(e => e.Id).HasColumnName("id");
			/// builder.Property(e => e.DriverId).HasColumnName("driver_id");
			/// builder.Property(e => e.Feedback)
			/// 	.HasMaxLength(200)
			/// 	.HasColumnName("feedback");
			/// builder.Property(e => e.PassengerId).HasColumnName("passenger_id");
			/// builder.Property(e => e.Price).HasColumnName("price");
			/// builder.Property(e => e.Rating).HasColumnName("rating");
			/// builder.Property(e => e.RideDate)
			/// 	.HasColumnType("datetime")
			/// 	.HasColumnName("ride_date");
			/// builder.Property(e => e.RideDestination)
			/// 	.HasMaxLength(50)
			/// 	.HasColumnName("ride_destination");
			/// builder.Property(e => e.RideSource)
			/// 	.HasMaxLength(50)
			/// 	.HasColumnName("ride_source");
			/// builder.Property(e => e.StatusId).HasColumnName("status_id");
			/// 
			/// builder.HasOne(d => d.Driver).WithMany(p => p.Rides)
			/// 	.HasForeignKey(d => d.DriverId)
			/// 	.HasConstraintName("FK__ride__driver_id__4E88ABD4");
			/// 
			/// builder.HasOne(d => d.Passenger).WithMany(p => p.Rides)
			/// 	.HasForeignKey(d => d.PassengerId)
			/// 	.HasConstraintName("FK__ride__passenger___4D94879B");
			/// 
			/// builder.HasOne(d => d.Status).WithMany(p => p.Rides)
			/// 	.HasForeignKey(d => d.StatusId)
			/// 	.HasConstraintName("FK__ride__status_id__4F7CD00D");



			builder.Property(e => e.RideDate).HasColumnType("datetime");

			builder.Property(e => e.RideDestination).HasMaxLength(100);

			builder.Property(e => e.RideSource).HasMaxLength(100);
			
			builder.HasOne(d => d.Driver)
				.WithMany(p => p.Rides)
				.HasForeignKey(d => d.DriverId);

			builder.HasOne(d => d.Passenger)
				.WithMany(p => p.Rides)
				.HasForeignKey(d => d.PassengerId);

			builder.HasOne(d => d.Status)
				.WithMany(p => p.Rides)
				.HasForeignKey(d => d.StatusId);

		}
	}
}
