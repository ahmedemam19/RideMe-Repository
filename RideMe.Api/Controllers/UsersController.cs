using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RideMe.Api.Dtos;
using RideMe.Api.Helpers;
using RideMe.Core.Interfaces;
using RideMe.Core.Models;
using RideMe.EF.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RideMe.Api.Controllers
{
	[Route("api/User")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IGenericRepository<City> _cityRepo;
		private readonly IGenericRepository<User> _usersRepo;
		private readonly IGenericRepository<Driver> _driversRepo;
		private readonly IGenericRepository<Passenger> _passengerRepo;
		private readonly IGenericRepository<Admin> _adminsRepo;
		private readonly TokenService _tokenService; 
		private readonly HashingFunctions _hashFunctions; 


		public UsersController( 
					IGenericRepository<City> cityRepo,
					IGenericRepository<User> usersRepo,
					IGenericRepository<Driver> driversRepo,
					IGenericRepository<Passenger> passengerRepo,
					IGenericRepository<Admin> adminsRepo)
        {
			_cityRepo = cityRepo;
			_usersRepo = usersRepo;
			_driversRepo = driversRepo;
			_passengerRepo = passengerRepo;
			_adminsRepo = adminsRepo;
			_tokenService = new TokenService();
			_hashFunctions = new HashingFunctions();
		}


		[HttpGet("get-cities")] // GET: /api/Users/get-cities
		public async Task<ActionResult> getCities()
		{
			var cities = await _cityRepo.GetAllAsync();
			var citiesData = cities.Select(c => new
			{
				Id = c.Id,
				Name = c.Name
			}).ToList();

			return Ok(citiesData);
		}


		[HttpPost("add-driver")] // POST: /api/Users/add-driver
		public async Task<ActionResult> AddDriver(AddDriverDto dto)
		{
			User? checkUser = await _usersRepo.FindAsync(u => u.Email == dto.Email);
			if(checkUser is not null) return BadRequest("this email already exists");

			var hashedpassword = _hashFunctions.HashPassword(dto.Password);

			User user = new User
			{
				Name = dto.Name,
				PhoneNumber = dto.PhoneNumber,
				Email = dto.Email,
				Password = hashedpassword,
				RoleId = 1,
				StatusId = 1
			};

			await _usersRepo.AddAsync(user);

			Driver driver = new Driver
			{
				UserId = user.Id,
				CityId = dto.CityId,
				Region = dto.Region,
				CarType = dto.CarType,
				Smoking = dto.Smoking,
				Available = false,
				AvgRating = -1
			};

			await _driversRepo.AddAsync(driver);

			return Ok();

		}


		[HttpPost("add-passenger")] // POST: /api/Users/add-passenger
		public async Task<ActionResult> AddPassenger(AddPassengerDto dto)
		{
			User? userCheck = await _usersRepo.FindAsync(u => u.Email == dto.Email);
			if(userCheck is not null) return BadRequest("this email already exists");

			var hashedpassword = _hashFunctions.HashPassword(dto.Password);

			User user = new User
			{
				Name = dto.Name,
				PhoneNumber = dto.PhoneNumber,
				Email = dto.Email,
				Password = hashedpassword,
				RoleId = 2,
				StatusId = 1
			};

			await _usersRepo.AddAsync(user);

			Passenger passenger = new Passenger
			{
				UserId = user.Id
			};

			await _passengerRepo.AddAsync(passenger);

			return Ok();

		}


		[HttpPost("login")] // POST: /api/Users/login
		public async Task<ActionResult> Login(LoginDto dto)
		{
			User? user = await _usersRepo.FindAsync(u => u.Email == dto.Email);

			if (user != null)
			{
				if (_hashFunctions.verifyPassword(user.Password, dto.Password))
				{
					if (user.RoleId == 1)
					{

						var driver = await _driversRepo.FindAllWithIncludesAsync(d => d.UserId == user.Id, d => d.User, d => d.City, d => d.User.Status, d => d.User.Role);
						var driverDto = driver.Select(d => new DriverTokenDto
						{
							Id = d.Id,
							UserId = d.UserId,
							Email = d.User.Email,
							Name = d.User.Name,
							Role = d.User?.Role.Name,
							Status = d.User?.Status.Name,
							PhoneNumber = d.User.PhoneNumber,
							CarType = d.CarType,
							Smoking = d.Smoking,
							City = d.City.Name,
							Region = d.Region,
							Available = d.Available,
							Rating = d.AvgRating
						}).FirstOrDefault();

						var token = _tokenService.CreateDriverToken(driverDto);
						return Ok(token);

					}
					else
					{

						var passenger = await _passengerRepo.FindAllWithIncludesAsync(p => p.UserId == user.Id, p => p.User, p => p.User.Status, p => p.User.Role);
						var passengerDto = passenger.Select(p => new PassengerTokenDto
						{
							Id = p.Id,
							UserId = p.UserId,
							Email = p.User.Email,
							Name = p.User.Name,
							Role = p.User?.Role.Name,
							Status = p.User?.Status.Name,
							PhoneNumber = p.User.PhoneNumber,
						}).FirstOrDefault();

						var token = _tokenService.CreatePassengerToken(passengerDto);
						return Ok(token);
					} 
				}
				else
				{
					return Unauthorized("wrong email or password");
				}
			}
			else
			{

				Admin? admin = await _adminsRepo.FindAsync(a => a.Email == dto.Email);
				
				if (admin is not null)
				{
					if (_hashFunctions.verifyPassword(admin.Password, dto.Password))
					{
						var token = _tokenService.CreateAdminToken(admin);
						return Ok(token); 
					}
					else
					{
						return Unauthorized("wrong email or password");
					}
				}
				else
				{
					return NotFound("wrong email or password !!");
				}
			}
		}


	}
}
