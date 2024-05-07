using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RideMe.Api.Dtos;
using RideMe.Core.Interfaces;
using RideMe.Core.Models;
using RideMe.EF.Data;

namespace RideMe.Api.Controllers
{
	[Route("api/Driver")]
	[ApiController]
	public class DriverController : ControllerBase
	{
		private readonly IGenericRepository<Ride> _ridesRepo;
		private readonly IGenericRepository<Driver> _driversRepo;
		private readonly IGenericRepository<Passenger> _passengersRepo;

		public DriverController(
				IGenericRepository<Ride> ridesRepo,
				IGenericRepository<Driver> driversRepo,
				IGenericRepository<Passenger> passengersRepo)
        {
			_ridesRepo = ridesRepo;
			_driversRepo = driversRepo;
			_passengersRepo = passengersRepo;
		}



		// The api gets all requested for a driver using the driver id
		[HttpGet("get-requested-ride/{driverId}")] // GET: /api/Driver/get-requested-ride/{driverId}
		public async Task<IActionResult> GetRequestedRideAsync(int driverId)
		{
			var ride = await _ridesRepo.FindAllWithIncludesAsync(
											r => r.DriverId == driverId && r.StatusId == 1,
											r => r.Passenger.User ); // Include User related to Passenger

			var selectedRide = ride.Select( r => new
			{
				Ride = r,
				PassengerName = r.Passenger.User.Name,
				PassengerPhone = r.Passenger.User.PhoneNumber
			});

			return Ok(selectedRide);
		}


		// takes ride id and gets its status
		[HttpGet("get-ride-status/{rideId}")] // GET: /api/Driver/get-ride-status/{rideId}
		public async Task<IActionResult> GetRideStatusAsync(int rideId)
		{
			var rides = await _ridesRepo.FindAllWithIncludesAsync(r => r.Id == rideId, r => r.Status);

			var ride = rides.FirstOrDefault();
			if (ride == null)
			{
				return NotFound("Invalid ride ID");
			}

			var rideStatus = rides.Select(r => new
			{
				rideId = r.Id,
				rideStatus = r.Status.Name
			}).FirstOrDefault();

			if(rideStatus is null) return NotFound("Invalid ride Id");

			return Ok(rideStatus);
		}


		[HttpGet("get-current-ride-status/{DriverId}")] // GET: /api/Driver/get-current-ride-status/{DriverId}
		public async Task<ActionResult> GetCurrentRideStatus(int DriverId)
		{
			var rides = await _ridesRepo.FindAllWithIncludesAsync(
												r => r.DriverId == DriverId && r.StatusId == 3,
												r => r.Driver.User,
												r => r.Passenger.User,
												r => r.Status);

			var selectedRide = rides.Select(r => new
			{
				RideId = r.Id,
				Driver = r.Driver.User.Name,
				Passenger = r.Passenger.User.Name,
				PassengerPhoneNumber = r.Passenger.User.PhoneNumber,
				Source = r.RideSource,
				Destination = r.RideDestination,
				Status = r.Status.Name,
				Price = r.Price,
				Rating = r.Rating,
				Feedback = r.Feedback,
				Date = r.RideDate
			}).ToList();

			return Ok(selectedRide);
		}


		[HttpPost("get-driver-daily-income")] // POST: /api/Driver/get-driver-daily-income
		public async Task<ActionResult> GetDriversDailyIncome(DailyIncomeDto dto)
		{
			DateOnly date = DateOnly.Parse(dto.DateString);

			var driverRides = await _ridesRepo.FindAllAsync(r => (r.DriverId == dto.DriverId) &&
																 (r.RideDate.Day == date.Day) &&
																 (r.RideDate.Month == date.Month) &&
																 (r.RideDate.Year == date.Year) && 
																 (r.StatusId == 4));

			double income = 0;

            foreach (var ride in driverRides) 
				income += (double)ride.Price;

			return Ok(income);
		}


		[HttpPost("get-driver-monthly-income")] // POST: /api/Driver/get-driver-monthly-income
		public async Task<ActionResult> GetDriversMonthlyIncome(MonthlyIncomeDto dto)
		{
			var driverRides = await _ridesRepo.FindAllAsync(r => (r.DriverId == dto.DriverId) && (r.StatusId == 4));

			double income = 0;

            foreach (var ride in driverRides)
				if (ride.RideDate.Month == dto.Month)
					income += (double)ride.Price;

			return Ok(income);
		}


		[HttpPut("available/{id}")] // PUT: /api/Driver/available/{id}
		public async Task<ActionResult> Available(int id)
		{
			Driver? driver = await _driversRepo.FindAsync(d => d.Id == id);

			if(driver is null) return NotFound("wrong id");

			driver.Available = true;

			await _driversRepo.UpdateAsync(driver);

			return Ok(driver);
		}


		[HttpPut("not-available/{id}")] // PUT: /api/Driver/not-available/{id}
		public async Task<ActionResult> NOtAvailable(int id)
		{
			Driver? driver = await _driversRepo.FindAsync(d => d.Id == id);

			if (driver is null) return NotFound("wrong id");

			driver.Available = false;

			await _driversRepo.UpdateAsync(driver);

			return Ok(driver);
		}


		[HttpPut("accept-ride/{id}")] // PUT: /api/Driver/accept-ride/{id}
		public async Task<ActionResult> AcceptRide(int id)
		{
			// validation
			Ride? ride = await _ridesRepo.FindAsync(r => r.Id == id);
			if(ride is null) return NotFound("wrong ride id");

			// accepting ride
			ride.StatusId = 3;
			await _ridesRepo.UpdateAsync(ride);

			// rejecting other driver requested rides
			Driver? driver = await _driversRepo.FindAsync(d => d.Id == ride.DriverId);

			if (driver == null) return NotFound("Driver not found");

			var otherDriverRides = await _ridesRepo.FindAllAsync(r => r.DriverId == ride.DriverId && r.StatusId == 1);

			foreach (var otherRide in otherDriverRides)
			{
				otherRide.StatusId = 2;
				await _ridesRepo.UpdateAsync(otherRide);
			}


			// rejecting other passenger requested rides
			Passenger? passenger = await _passengersRepo.FindAsync(p => p.Id == ride.PassengerId);

			if (passenger == null) return NotFound("Passenger not found");

			var otherPassengerRides = await _ridesRepo.FindAllAsync(r => r.PassengerId == ride.PassengerId && r.StatusId == 1);

			foreach (var otherRide in otherPassengerRides)
			{
				otherRide.StatusId = 2;
				await _ridesRepo.UpdateAsync(otherRide);
			}

			// changing driver to not available
			driver.Available = false;
			await _driversRepo.UpdateAsync(driver);

			// returning the ride
			var response = await _ridesRepo.FindAllWithIncludesAsync(r => r.Id == id,
																	 r => r.Driver.User,
																	 r => r.Passenger.User,
																	 r => r.Status);


			var responseData = response.Select(r => new
			{
				RideId = r.Id,
				Driver = r.Driver?.User?.Name,
				Passenger = r.Passenger?.User?.Name,
				Source = r.RideSource,
				Destination = r.RideDestination,
				Status = r.Status?.Name,
				Price = r.Price,
			}).FirstOrDefault();

			return Ok(responseData);

		}


		[HttpPut("reject-ride/{id}")] // PUT: /api/Driver/reject-ride/{id}
		public async Task<ActionResult> RejectRide(int id)
		{
			Ride? ride = await _ridesRepo.FindAsync(r => r.Id == id);
			if (ride is null) return NotFound("Wrong Id");

			ride.StatusId = 2;
			await _ridesRepo.UpdateAsync(ride);

			return Ok(ride);
		}


	}
}
