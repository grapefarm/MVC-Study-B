using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using WebApplication2.Services;

namespace WebApplication2
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews();

			// 讀取連線字串並註冊 DbContext
			var connectionString = builder.Configuration.GetConnectionString("NorthwindConnection");
			builder.Services.AddDbContext<NorthwindContext>(options =>
				options.UseSqlServer(connectionString));

			// 告訴 .NET 宇宙：未來只要有人跟你要 IProductService，你就實作一個 ProductService 給他
			builder.Services.AddScoped<IProductService, ProductService>();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.Run();
		}
	}
}
