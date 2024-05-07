using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RideMe.Api.Dtos;
using RideMe.Core.Interfaces;
using RideMe.Core.Models;

namespace RideMe.Api.Controllers
{
	[Route("api/Passenger")]
	[ApiController]
	public class PassengerController : ControllerBase
	{
		private readonly IGenericRepository<Passenger> _passengerRepo;
		private readonly IGenericRepository<Ride> _rideRepo;
		private readonly IGenericRepository<Driver> _driverRepo;

		public PassengerController(
			IGenericRepository<Passenger> passengerRepo,
			IGenericRepository<Ride> rideRepo,
			IGenericRepository<Driver> driverRepo)
		{
			_passengerRepo = passengerRepo;
			_rideRepo = rideRepo;
			_driverRepo = driverRepo;
		}


		#region Commented APIs

		/// [HttpGet("get-filtered-drivers")]
		/// public async Task<IActionResult> getFilteredDriver([FromQuery] PassengerPreferenceDto dto)
		/// {
		///	 var acceptedStatus = await _context.UserStatuses.FirstOrDefaultAsync(s => s.Name == "accepted");
		///
		///	 var drivers = await _context.Drivers
		///		 .Include(d => d.User)
		///		 .Where(d => d.Available == true &&
		///					 d.User.Status == acceptedStatus &&
		///					 d.CarType.ToLower() == (dto.CarType).ToLower() &&
		///					 d.CityId == dto.CityId &&
		///					 d.Region.ToLower() == (dto.Region).ToLower())
		///		 .Select(d => new
		///		 {
		///			 d.Id,
		///			 d.UserId,
		///			 d.CarType,
		///			 d.Smoking,
		///			 d.CityId,
		///			 d.Region,
		///			 d.Available,
		///			 d.AvgRating
		///		 })
		///		 .ToListAsync();
		///
		///	 if (drivers == null)
		///		 return NotFound("No drivers found with selected preferences");
		///
		///	 return Ok(drivers);
		///
		///	}

		#endregion


		[HttpGet("get-passenger-ride-history/{PassengerId}")] // GET: /api/Passenger/get-passenger-ride-history/{PassengerId}
		public async Task<ActionResult> GetPassengerRideHistory(int PassengerId)
		{
			var rides = await _rideRepo.FindAllWithIncludesAsync(
											r => r.PassengerId == PassengerId,
											r => r.Driver.User,
											r => r.Passenger.User,
											r => r.Status);

			var rideDetails = rides.Select(r => new
			{
				RideId = r.Id,
				Driver = r.Driver.User.Name,
				DriverPhoneNumber = r.Driver.User.PhoneNumber,
				Passenger = r.Passenger.User.Name,
				Source = r.RideSource,
				Destination = r.RideDestination,
				Status = r.Status.Name,
				Price = r.Price,
				Rating = r.Rating,
				Feedback = r.Feedback,
				Date = r.RideDate
			}).ToList();

			return Ok(rideDetails);
		}


		[HttpGet("get-current-ride-status/{PassengerId}")] // GET: /api/Passenger/get-current-ride-status/{PassengerId}
		public async Task<ActionResult> GetCurrentRideStatus(int PassengerId)
		{
			var rides = await _rideRepo.FindAllWithIncludesAsync(
											r => r.PassengerId == PassengerId && r.StatusId == 3 && r.RideDate.Date == DateTime.Now.Date,
											r => r.Driver.User,
											r => r.Passenger.User,
											r => r.Status);

			var rideDetails = rides.Select(r => new
			{
				RideId = r.Id,
				Driver = r.Driver.User.Name,
				DriverPhoneNumber = r.Driver.User.PhoneNumber,
				Passenger = r.Passenger.User.Name,
				Source = r.RideSource,
				Destination = r.RideDestination,
				Status = r.Status.Name,
				Price = r.Price,
				Rating = r.Rating,
				Feedback = r.Feedback,
				Date = r.RideDate
			}).ToList();

			return Ok(rideDetails);

		}


		[HttpGet("get-available-drivers")] // GET: /api/Passenger/get-available-drivers
		public async Task<ActionResult> GetAvailableDrivers()
		{
			var drivers = await _driverRepo.FindAllWithIncludesAsync(
												d => d.Available == true,
												d => d.User,
												d => d.City);

			var driversDetails = drivers.Select(d => new
			{
				id = d.Id,
				name = d.User.Name,
				car = d.CarType,
				city = d.City.Name,
				region = d.Region,
				smoking = d.Smoking,
				rating = d.AvgRating
			}).ToList();

			if (driversDetails == null)
				return Ok("No avaiable drivers");

			return Ok(driversDetails);
		}


		[HttpGet("get-available-car-types")] // GET: /api/Passenger/get-available-car-types
		public async Task<ActionResult> GetAvailableCarTypes()
		{
			var driverAvailable = await _driverRepo.FindAllAsync(d => d.Available == true);

			var carType = driverAvailable.Select(d => d.CarType).Distinct().ToList();

			return Ok(carType);
		}


		[HttpGet("get-filtered-drivers")] // GET: /api/Passenger/get-filtered-drivers
		public async Task<ActionResult> GetFilteredDrivers(
			[FromQuery] string carType = null, // A defualt value of null is important
			[FromQuery] bool? smoking = null,
			[FromQuery] string city = null)
		{
			var drivers = await _driverRepo.FindAllWithIncludesAsync(
												d => d.Available == true,
												d => d.User,
												d => d.City);

			// Apply filters
			if (!string.IsNullOrEmpty(carType)) 
			{
				drivers = drivers.Where(d => d.CarType == carType).ToList();
			}
			if (smoking.HasValue)
			{
				drivers = drivers.Where(d => d.Smoking == smoking).ToList();
			}
			if (!string.IsNullOrEmpty(city))
			{
				drivers = drivers.Where(d => d.City.Name == city).ToList();
			}

			var filteredDrivers = drivers.Select(d => new
			{
				id = d.Id,
				name = d.User.Name,
				car = d.CarType,
				city = d.City.Name,
				region = d.Region,
				smoking = d.Smoking,
				rating = d.AvgRating
			}).ToList();

			return Ok(filteredDrivers);

		}



		[HttpPost("rate-ride")] // POST: /api/Passenger/rate-ride
		public async Task<ActionResult> addRatingAsync(RateAndFeedbackDto dto)
		{
			Ride? ride = await _rideRepo.FindAsync(r => r.Id == dto.Id);
			if(ride is null) return NotFound("wrong id");

			if (dto.Rating < 0 || dto.Rating > 5)
				return BadRequest("Rating must be between 0 and 5");

			ride.Rating = dto.Rating;

			ride.Feedback = dto.Feedback;

			await _rideRepo.UpdateAsync(ride);

			var response = new
			{
				id = ride.Id,
				rating = ride.Rating,
				feedback = ride.Feedback
			};


			// calculating avg rating for driver part
			var driverId = ride.DriverId;


			var driverRatings = (await _rideRepo.FindAllAsync(d => d.DriverId == driverId && d.Rating != -1.0))
				.Select(d => d.Rating).ToList();

			//var selectedRating = driverRatings.Select(d => d.Rating).ToList();


			// calculate average rating
			double averageRating = driverRatings.Any() ? driverRatings.Average() ?? 0 : 0;
			double formattedRating = Math.Round(averageRating, 2, MidpointRounding.AwayFromZero);

			// update the average rating in the driver table
			Driver? driver = await _driverRepo.FindAsync(d => d.Id == driverId);
			if (driver != null)
			{
				// Store the formatted rating (rounded to two decimal places) as a double in the database
				driver.AvgRating = formattedRating;
				await _rideRepo.UpdateAsync(ride);
			}

			return Ok(response);

		}

		
		[HttpPut("confirm-payment/{rideId}")] // PUT: /api/Passenger/confirm-payment/{id}
		public async Task<ActionResult> ConfirmPayment(int rideId)
		{
			// make the ride completed
			Ride? ride = await _rideRepo.FindAsync(r => r.Id == rideId);
			if(ride is null) return NotFound("wrong id");
			ride.StatusId = 4;

			// make the driver available
			Driver? driver = await _driverRepo.FindAsync(d => d.Id == ride.DriverId);

			if (driver is null) return NotFound("wrong id");

			driver.Available = true;

			await _driverRepo.UpdateAsync(driver);

			return Ok(ride);
		}


	}
}
