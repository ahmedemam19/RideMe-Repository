
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RideMe.Core.Interfaces;
using RideMe.EF;
using RideMe.EF.Data;
using System.Text;
using System.Text.Json.Serialization;

namespace RideMe.Api
{
	public class Program
	{
		public static void Main(string[] args)
		{

			#region Configure Services

			var builder = WebApplication.CreateBuilder(args);

			var config = builder.Configuration;


			// Add services to the container.


			builder.Services.AddDbContext<ApplicationDbContext>(options =>
			{
				options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
			});


			// jwt configurations start here 
			builder.Services.AddAuthentication(x =>
			{
				x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(x =>
			{
				x.TokenValidationParameters = new TokenValidationParameters
				{
					ValidIssuer = config["JwtSettings:issuer"],
					ValidAudience = config["JwtSettings:audience"],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSettings:key"]!)),
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true
				};
			});


			// add auth
			builder.Services.AddAuthorization();


			builder.Services.AddControllers();
			

			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			/// this line essentially tells the dependency injection. container that when an IGenericRepository<T> is requested, 
			/// it should resolve it to an instance of GenericRepository<T>. This allows for loosely coupling 
			/// your application components through interfaces and enables easier testing and swapping of implementations.
			builder.Services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));

			builder.Services.AddControllers().AddJsonOptions(x =>
				x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

			#endregion


			var app = builder.Build();



			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

			app.UseAuthentication();

			app.UseAuthorization();

			app.MapControllers();

			app.Run();
		}
	}
}
