using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RideMe.Core.Models;

namespace RideMe.EF.Configurations
{
	internal class PassengerConfig : IEntityTypeConfiguration<Passenger>
	{
		public void Configure(EntityTypeBuilder<Passenger> builder)
		{
			/// builder.HasKey(e => e.Id).HasName("PK__passenge__3213E83FEC15F03B");
			/// 
			/// builder.ToTable("passenger");
			/// 
			/// builder.HasIndex(e => e.UserId, "UQ__passenge__B9BE370E881AFA14").IsUnique();
			/// 
			/// builder.Property(e => e.Id).HasColumnName("id");
			/// builder.Property(e => e.UserId).HasColumnName("user_id");
			/// 
			/// builder.HasOne(d => d.User).WithOne(p => p.Passenger)
			/// 	.HasForeignKey<Passenger>(d => d.UserId)
			/// 	.OnDelete(DeleteBehavior.Cascade)
			/// 	.HasConstraintName("FK__passenger__user___4AB81AF0");



			builder.HasIndex(e => e.UserId).IsUnique();

			builder.HasOne(d => d.User).WithOne(p => p.Passenger)
				.HasForeignKey<Passenger>(d => d.UserId)
				.OnDelete(DeleteBehavior.Cascade);

		}
	}
}
