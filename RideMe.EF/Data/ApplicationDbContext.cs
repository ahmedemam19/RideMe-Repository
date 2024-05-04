using Microsoft.EntityFrameworkCore;
using RideMe.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RideMe.EF.Data
{
	public class ApplicationDbContext : DbContext
	{
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {}


		protected override void OnModelCreating(ModelBuilder modelBuilder)
			=> modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());


		public DbSet<Admin> Admins { get; set; }

		public DbSet<City> Cities { get; set; }

		public DbSet<Driver> Drivers { get; set; }

		public DbSet<Passenger> Passengers { get; set; }

		public DbSet<Ride> Rides { get; set; }

		public DbSet<RideStatus> RideStatuses { get; set; }

		public DbSet<Role> Roles { get; set; }

		public DbSet<User> Users { get; set; }

		public DbSet<UserStatus> UserStatuses { get; set; }



	}
}
