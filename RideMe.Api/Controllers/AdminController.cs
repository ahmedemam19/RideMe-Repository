using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RideMe.Api.Dtos;
using RideMe.Api.Token;
using RideMe.Core.Interfaces;
using RideMe.Core.Models;

namespace RideMe.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AdminController : ControllerBase
	{
		private readonly IGenericRepository<Driver> _driversRepo;
		private readonly IGenericRepository<Ride> _ridesRepo;
		private readonly IGenericRepository<Passenger> _passengersRepo;
		private readonly IGenericRepository<Role> _roleRepo;
		private readonly IGenericRepository<City> _citiesRepo;
		private readonly IGenericRepository<Admin> _adminsRepo;
		private readonly IGenericRepository<User> _usersRepo;
		private readonly IGenericRepository<UserStatus> _userStatusRepo;
		private readonly HashingFunctions _hashFunctions;

		public AdminController(
			IGenericRepository<Driver> driversRepo,
			IGenericRepository<Ride> ridesRepo,
			IGenericRepository<Passenger> passengersRepo,
			IGenericRepository<Role> roleRepo,
			IGenericRepository<City> citiesRepo,
			IGenericRepository<Admin> adminsRepo,
			IGenericRepository<User> usersRepo,
			IGenericRepository<UserStatus> userStatusRepo)
		{
			_driversRepo = driversRepo;
			_ridesRepo = ridesRepo;
			_passengersRepo = passengersRepo;
			_roleRepo = roleRepo;
			_citiesRepo = citiesRepo;
			_adminsRepo = adminsRepo;
			_usersRepo = usersRepo;
			_userStatusRepo = userStatusRepo;
			_hashFunctions = new HashingFunctions();
		}



		#region Commented Actions


		// The API will get all rides from table and frontend will use them to
		// Show details of the ride and use the "status" show completed or not
		/// [HttpGet("get-all-rides")]
		/// public async Task<IActionResult> GetAllRidesAsync()
		/// {
		///     var rides = await _context.Rides.ToListAsync();
		/// 
		///     return Ok(rides);
		/// }


		// Same as the previous api, the api will return all values of the table
		// and the front will utilize only the field they want => (avgRating field)
		/// [HttpGet("get-all-drivers")]
		/// public async Task<IActionResult> GetAllDriversAsync()
		/// {
		///     var drivers = await _context.Drivers.ToListAsync();
		/// 
		///     return Ok(drivers);
		/// }



		// Enter Driver ID to block. Returns 404 if id doesnt exist
		/// [HttpPut("block-driver/{id}")]
		/// public async Task<IActionResult> blockDriverByIdAsync(int id)
		/// {
		/// 	var driver = await _context.Drivers
		/// 		.Include(d => d.User)
		/// 		.FirstOrDefaultAsync(d => d.Id == id);
		/// 
		/// 	if (driver == null)
		/// 		return NotFound($"No driver was found with id: {id}");
		/// 
		/// 
		/// 	/// The following piece of code avoids hard-coding
		/// Instead of putting 4 for blocked, the code searches
		/// for the status named "blocked" and gets it's id
		/// 
		/// 	var blockedStatus = await _context.UserStatuses.FirstOrDefaultAsync(s => s.Name == "blocked");
		/// 
		/// 	if (blockedStatus == null)
		/// 		return NotFound("Blocked status not found in the database.");
		/// 
		/// 	driver.User.StatusId = blockedStatus.Id;
		/// 
		/// 	// This will return a driver with its status
		/// 	var response = new
		/// 	{
		/// 		driver.Id,
		/// 		driver.User.Username, // Change this to name later
		/// 		driver.User.StatusId
		/// 	};
		/// 
		/// 	await _context.SaveChangesAsync();
		/// 
		/// 	return Ok(response);
		/// }


		#endregion


		[HttpGet("get-accepted-or-blocked-drivers")] // GET : /api/Admin/get-accepted-or-blocked-drivers
		public async Task<ActionResult> GetAllDrivers()
		{
			var drivers = await _driversRepo.FindAllWithIncludesAsync(
						d => d.User.StatusId == 2 || d.User.StatusId == 4,
						d => d.User,
						d => d.City);

			var driverDetails = drivers.Select(d => new
			{
				Id = d.User.Id,
				Name = d.User.Name,
				PhoneNumber = d.User.PhoneNumber,
				Email = d.User.Email,
				Status = d.User.Status.Name,
				CarType = d.CarType,
				IsSmoking = d.Smoking,
				City = d.City.Name,
				Region = d.Region,
				Isavailable = d.Available,
				Rating = d.AvgRating
			}).ToList();

			return Ok(driverDetails);
		}

		[Authorize(Roles = "admin")]
		[HttpGet("get-all-rides")] // GET : /api/Admin/get-all-rides
		public async Task<ActionResult> GetAllRides()
		{
			var rides = await _ridesRepo.FindAllWithIncludesAsync(
						r => r.Status,
						r => r.Passenger,
						r => r.Driver
						);


			var rideDetails = rides.Select(r => new
			{
				RideId = r.Id,
				DriverName = r.Driver.User.Name,
				PassengerName = r.Passenger.User.Name,
				RideSource = r.RideSource,
				RideDestination = r.RideDestination,
				Status = r.Status.Name,
				Price = r.Price,
				Rating = r.Rating,
				Feedback = r.Feedback,
				RideDate = r.RideDate
			}).ToList();

			return Ok(rideDetails);

		}


		[HttpGet("get-waiting-passengers")] // GET : /api/Admin/get-waiting-passengers
		public async Task<ActionResult> GetWaitingPassengers()
		{
			var passengers = await _passengersRepo.FindAllWithIncludesAsync(p => p.User.StatusId == 1, p => p.User);

			var passengerDetails = passengers.Select(p => new
			{
				Id = p.User.Id,
				Name = p.User.Name,
				Email = p.User.Email,
				PhoneNumber = p.User.PhoneNumber,
			}).ToList();

			return Ok(passengerDetails);
		}


		[HttpGet("get-waiting-drivers")] // GET : /api/Admin/get-waiting-drivers
		public async Task<ActionResult> GetWaitingDrivers()
		{
			var drivers = await _driversRepo.FindAllWithIncludesAsync(d => d.User.StatusId == 1, d => d.User);

			var driverDetails = drivers.Select(d => new
			{
				Id = d.User.Id,
				Name = d.User.Name,
				PhoneNumber = d.User.PhoneNumber,
				Email = d.User.Email,
				Status = d.User.Status.Name,
				CarType = d.CarType,
				IsSmoking = d.Smoking,
				City = d.City.Name,
				Region = d.Region,
				IsAvailable = d.Available
			}).ToList();

			return Ok(driverDetails);
		}


		[HttpPost("add-role/{RName}")] // POST : /api/Admin/add-role/{RName}
		public async Task<ActionResult> AddRole(String RName)
		{
			Role role = new Role
			{
				Name = RName
			};
			await _roleRepo.AddAsync(role);
			return Ok();
		}


		[HttpPost("add-city/{CName}")] // POST : /api/Admin/add-city/{CName}
		public async Task<ActionResult> AddCity(String CName)
		{
			City city = new City
			{
				Name = CName
			};
			await _citiesRepo.AddAsync(city);
			return Ok();
		}


		[HttpPost("add-admin")] // POST : /api/Admin/add-admin
		public async Task<ActionResult> AddAdmin(AddAdminDto dto)
		{
			Admin? adminCheck = await _adminsRepo.FindAsync(a => a.Email == dto.Email);

			if (adminCheck is not null) return BadRequest("This email already exists !!");

			var hashedpassword = _hashFunctions.HashPassword(dto.Password);

			var admin = new Admin
			{
				Name = dto.Name,
				Email = dto.Email,
				Password = hashedpassword
			};

			await _adminsRepo.AddAsync(admin);
			return Ok();

		}


		[HttpPut("accept-user/{id}")] // PUT : /api/Admin/accept-user/{id}
		public async Task<ActionResult> AcceptUser(int id)
		{
			User? user = await _usersRepo.FindAsync(u => u.Id == id);

			if (user is null) return NotFound("Wrong Data !!");

			user.StatusId = 2;

			await _usersRepo.UpdateAsync(user);

			return Ok(user);
		}


		[HttpPut("unblock-driver/{id}")] // PUT : /api/Admin/accept-user/{id} // Enter Driver ID to block. Returns 404 if id doesnt exist
		public async Task<IActionResult> unblockDriverByIdAsync(int id)
		{
			var driver = await _driversRepo.FindWithIncludesAsync(d => d.User.Id == id, d => d.User);

			if (driver == null) return NotFound($"No driver was found with id: {id}");

			var acceptedStatus = await _userStatusRepo.FindAsync(s => s.Name == "accepted");

			if (acceptedStatus == null) return NotFound("Blocked status not found in the database.");

			driver.User.StatusId = acceptedStatus.Id;

			await _driversRepo.UpdateAsync(driver);

			var response = new
			{
				driver.Id,
				driver.User.Email, // Change this to name later
				driver.User.StatusId
			};

			return Ok(response);
		}


		[HttpPut("reject-user/{id}")] // PUT : /api/Admin/reject-user/{id}
		public async Task<ActionResult> RejectUser(int id)
		{
			User? user = await _usersRepo.FindAsync(u => u.Id == id);
			if (user == null) return NotFound("Wrong Id");
			user.StatusId = 3;
			await _usersRepo.UpdateAsync(user);
			return Ok(user);
		}


		[HttpPut("block-driver/{id}")] // PUT : /api/Admin/block-driver/{id}
		public async Task<ActionResult> BlockDriver(int id)
		{
			User? user = await _usersRepo.FindAsync(u => u.Id == id);
			if (user == null) return NotFound("Wrong Id");
			user.StatusId = 4;
			await _usersRepo.UpdateAsync(user);
			return Ok(user);
		}


	}
}
