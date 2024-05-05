using Microsoft.IdentityModel.Tokens;
using RideMe.Api.Dtos;
using RideMe.Core.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RideMe.Api.Token
{
	public class TokenService
	{
		private readonly string _secretKey = "sz8eI7OdHBrjrIo8j9jkloLnTHEIdovpdvecW/KrQymjuyhnO1OvY0pAQ2wDKQZw/0=";

		private string CreateToken(List<Claim> claims)
		{
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

			var token = new JwtSecurityToken(
				issuer: "RideMe",
				audience: "Riders",
				claims: claims,
				expires: DateTime.Now.AddDays(1),
				signingCredentials: creds);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}


		public string CreateAdminToken(Admin admin)
		{
			var claims = new List<Claim>
			{
				new Claim("Id", admin.Id.ToString()),
				new Claim(ClaimTypes.Role, "admin"),
				new Claim("Role", "admin"),
				new Claim("Email", admin.Email),
				new Claim("Name", admin.Name),
			};
			return CreateToken(claims);
		}


		public string CreateDriverToken(DriverTokenDto driver)
		{
			var claims = new List<Claim>
			{
				new Claim("UserId", driver.UserId.ToString()),
				new Claim("Id", driver.Id.ToString()),
				new Claim(ClaimTypes.Role, driver.Role),
				new Claim("Role", driver.Role),
				new Claim("Name", driver.Name),
				new Claim("Email", driver.Email),
				new Claim("Status", driver.Status),
				new Claim("Smoking", driver.Smoking.ToString()),
				new Claim("Available", driver.Available.ToString()),
				new Claim("City", driver.City),
				new Claim("Region", driver.Region),
				new Claim("CarType", driver.CarType),
				new Claim("PhoneNumber", driver.PhoneNumber),
				new Claim("Rating", driver.Rating.ToString())
			};

			return CreateToken(claims);
		}


		public string CreatePassengerToken(PassengerTokenDto passenger)
		{
			var claims = new List<Claim>
			{
				new Claim("UserId", passenger.UserId.ToString()),
				new Claim("Id", passenger.Id.ToString()),
				new Claim(ClaimTypes.Role, passenger.Role),
				new Claim("Role", passenger.Role),
				new Claim("Name", passenger.Name),
				new Claim("Email", passenger.Email),
				new Claim("Status", passenger.Status),
				new Claim("PhoneNumber", passenger.PhoneNumber),
			};

			return CreateToken(claims);
		}



	}
}
