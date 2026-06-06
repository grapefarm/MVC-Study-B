
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication3
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();
			// 💡 註冊 DbContext（記得引用對應的命名空間，例如 using WebApplication3.Models;）
			builder.Services.AddDbContext<NorthwindContext>(options =>
				options.UseSqlServer(builder.Configuration.GetConnectionString("NorthwindConnection")));

			builder.Services.AddCors(options =>
			{
				options.AddPolicy("AllowAll", policy =>
				{
					policy.AllowAnyOrigin()   // 允許任何網域呼交（例如你的 MVC 專案）
						  .AllowAnyMethod()   // 允許 GET, POST, PUT, DELETE
						  .AllowAnyHeader();  // 允許任何 Headers
				});
			});

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseCors("AllowAll");

			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
