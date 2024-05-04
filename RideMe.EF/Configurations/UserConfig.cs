using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RideMe.Core.Models;

namespace RideMe.EF.Configurations
{
	internal class UserConfig : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			/// builder.HasKey(e => e.Id).HasName("PK__user__3213E83FCB7433B5");
			/// 
			/// builder.ToTable("user");
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
			/// builder.Property(e => e.PhoneNumber)
			/// 	.HasMaxLength(15)
			/// 	.HasColumnName("phone_number");
			/// builder.Property(e => e.RoleId).HasColumnName("role_id");
			/// builder.Property(e => e.StatusId).HasColumnName("status_id");
			/// 
			/// builder.HasOne(d => d.Role).WithMany(p => p.Users)
			/// 	.HasForeignKey(d => d.RoleId)
			/// 	.HasConstraintName("FK__user__role_id__412EB0B6");
			/// 
			/// builder.HasOne(d => d.Status).WithMany(p => p.Users)
			/// 	.HasForeignKey(d => d.StatusId)
			/// 	.HasConstraintName("FK__user__status_id__4222D4EF");



			builder.Property(e => e.Email).HasMaxLength(50);

			builder.Property(e => e.Name).HasMaxLength(50);

			builder.Property(e => e.Password).HasMaxLength(200);

			builder.Property(e => e.PhoneNumber).HasMaxLength(15);

			builder.HasOne(d => d.Role)
				.WithMany(p => p.Users)
				.HasForeignKey(d => d.RoleId);

			builder.HasOne(d => d.Status)
				.WithMany(p => p.Users)
				.HasForeignKey(d => d.StatusId);

		}
	}
}
