
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using RideMe.Core.Interfaces;
using RideMe.EF;
using RideMe.EF.Data;
using System.Text.Json.Serialization;

namespace RideMe.Api
{
	public class Program
	{
		public static void Main(string[] args)
		{

			#region Configure Services

			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.


			builder.Services.AddDbContext<ApplicationDbContext>(options =>
			{
				options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
			});


			builder.Services.AddControllers();
			

			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();



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

			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
