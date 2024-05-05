﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RideMe.Core.Interfaces;
using RideMe.Core.Models;

namespace RideMe.Api.Controllers
{
	[Route("api/[controller]")]
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
											r => r.Driver,
											r => r.Passenger,
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
											r => r.Driver,
											r => r.Passenger,
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


		[HttpPut("confirm-payment/{id}")] // PUT: /api/Passenger/confirm-payment/{id}
		public async Task<ActionResult> ConfirmPayment(int id)
		{
			// make the ride completed
			Ride? ride = await _rideRepo.FindAsync(r => r.Id == id);
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