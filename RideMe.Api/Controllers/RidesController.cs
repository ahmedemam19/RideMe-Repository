using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RideMe.Api.Dtos;
using RideMe.Core.Interfaces;
using RideMe.Core.Models;

namespace RideMe.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RidesController : ControllerBase
	{
		private readonly IGenericRepository<Ride> _ridesRepo;
		private readonly IGenericRepository<Passenger> _passenegrRepo;
		private readonly IGenericRepository<Driver> _driverRepo;
		private readonly IGenericRepository<RideStatus> _rideStatusRepo;

		public RidesController(
			IGenericRepository<Ride> ridesRepo,
			IGenericRepository<Passenger> passenegrRepo,
			IGenericRepository<Driver> driverRepo,
			IGenericRepository<RideStatus> rideStatusRepo)
		{
			_ridesRepo = ridesRepo;
			_passenegrRepo = passenegrRepo;
			_driverRepo = driverRepo;
			_rideStatusRepo = rideStatusRepo;
		}

        [HttpPost("request-ride")] // POST : /api/Rides/request-ride
		public async Task<IActionResult> addRidesAsync([FromBody] RequestRideDto dto)
		{
			var passenger = await _passenegrRepo.GetByIdAsync(dto.PassengerId);
			if(passenger is null) return NotFound($"No passenger with id: {dto.PassengerId}");

			var driver = await _driverRepo.GetByIdAsync(dto.DriverId);
			if (driver is null) return NotFound($"No driver with id: {dto.DriverId}");

			var requestedStatus = await _rideStatusRepo.FindAsync(rs => rs.Name == "requested");

			var ride = new Ride
			{
				PassengerId = dto.PassengerId,
				DriverId = dto.DriverId,
				RideSource = dto.RideSource,
				RideDestination = dto.RideDestination,
				StatusId = requestedStatus.Id,
				Price = dto.Price,
				Rating = -1,
				Feedback = "",
				RideDate = DateTime.Today
			};

			await _ridesRepo.AddAsync(ride);

			return Ok(new
			{
				ride.Id,
				ride.PassengerId,
				ride.DriverId,
				ride.RideSource,
				ride.RideDestination,
				ride.StatusId,
				ride.Price,
				ride.RideDate
			});
		}


		[HttpDelete("cancel-ride/{id}")] // POST : /api/Rides/cancel-ride/{id}
		public async Task<IActionResult> cancelRideAsync(int id)
		{
			var ride = await _ridesRepo.GetByIdAsync(id);

			if (ride is null) return NotFound($"No ride was found with id: {id}");

			await _ridesRepo.DeleteAsync(id);

			return Ok(ride);
		}

	}
}
