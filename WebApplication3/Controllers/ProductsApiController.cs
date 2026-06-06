using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using WebApplication3.DTOs;

namespace WebApplication3.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductsApiController : ControllerBase
	{
		// 💡 1. 宣告唯讀的資料庫上下文私有變數
		private readonly NorthwindContext _context;

		// 💡 2. 透過構造函式（建構子）進行 DI 注入，考官最看重這段！
		public ProductsApiController(NorthwindContext context)
		{
			_context = context;
		}

		// 💡 3. 將方法改為 async Task<IActionResult> 實作非同步真邏輯
		[HttpPost("toggle-discontinued")]
		public async Task<IActionResult> ToggleDiscontinued([FromBody] ToggleStatusDto dto)
		{
			if (dto == null) return BadRequest(new { success = false, message = "傳送資料不完整" });

			// 💡 4. 真・直球去資料庫撈出該筆商品
			var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == dto.Id);
			if (product == null) return NotFound(new { success = false, message = "找不到該商品" });

			// 💡 5. 真・商業邏輯：狀態直接取反向 (原本是 true 變 false，false 變 true)
			product.Discontinued = !product.Discontinued;

			// 💡 6. 存檔下班！
			await _context.SaveChangesAsync();

			return Ok(new
			{
				success = true,
				isDiscontinued = product.Discontinued, // 把最新狀態傳回前端
				message = product.Discontinued ? "❌ 該商品已設定為【停售狀態】" : "✅ 該商品已設定為【正常販售】"
			});
		}
	}
}
