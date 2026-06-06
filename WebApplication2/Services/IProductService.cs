using WebApplication2.Models;

namespace WebApplication2.Services
{
	public interface IProductService
	{
		// 宣告一個方法：傳入關鍵字、分類ID、排序條件，吐回處理好的 Product 清單
		Task<IEnumerable<Product>> GetFilteredProductsAsync(string keyword, int? categoryId, string sortBy);
	}
}
