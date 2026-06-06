using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Services
{
	public class ProductService : IProductService
	{
		private readonly NorthwindContext _context;

		public ProductService(NorthwindContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Product>> GetFilteredProductsAsync(string keyword, int? categoryId, string sortBy)
		{
			// 3. 宣告查詢基底
			var products = _context.Products
				.Include(p => p.Category)
				.Include(p => p.Supplier)
				.AsQueryable();

			// 4. 條件一：關鍵字篩選
			if (!string.IsNullOrEmpty(keyword))
			{
				products = products.Where(p => p.ProductName.Contains(keyword));
			}

			// 5. 條件二：下拉選單篩選
			if (categoryId.HasValue)
			{
				products = products.Where(p => p.CategoryId == categoryId);
			}

			// 6. 條件三：排序邏輯（使用 C# Switch 運算式）
			products = sortBy switch
			{
				"PriceAsc" => products.OrderBy(p => p.UnitPrice),
				"PriceDesc" => products.OrderByDescending(p => p.UnitPrice),
				_ => products.OrderBy(p => p.ProductId) // ⚠️ 這裡的 Id 記得對齊你 Model 的大小寫喔（例如 ProductId 或 ProductID）
			};

			// 7. 最後一氣呵成轉成 List 吐給前端
			return await products.ToListAsync();
		}
	}
}
