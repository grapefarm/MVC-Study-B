using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class ProductsController : Controller
    {
        private readonly NorthwindContext _context;

        public ProductsController(NorthwindContext context)
        {
            _context = context;
        }

		// GET: Products
		public async Task<IActionResult> Index(string keyword = null, int? categoryId = null, string sortBy = null)
		{
			// 1. 下拉選單門票（無條件載入，並記住上次選的 categoryId）
			ViewBag.CategoryId = new SelectList(_context.Categories, "CategoryId", "CategoryName", categoryId);

			// 2. 狀態保留：把關鍵字和當前的排序狀態丟給前端 View
			ViewData["CurrentKeyword"] = keyword;

			// 💡 排序開關切換邏輯：如果現在是正序(PriceAsc)，下次點擊超連結就要變倒序(PriceDesc)，反之亦然
			ViewData["PriceSortParam"] = sortBy == "PriceAsc" ? "PriceDesc" : "PriceAsc";
			ViewData["CurrentSort"] = sortBy;

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
				_ => products.OrderBy(p => p.ProductName) // 預設以商品名稱排序
			};

			// 7. 最後一氣呵成轉成 List 吐給前端
			return View(await products.ToListAsync());
		}

		// GET: Products/Details/5
		public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,SupplierId,CategoryId,QuantityPerUnit,UnitPrice,UnitsInStock")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName", product.SupplierId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName", product.SupplierId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,SupplierId,CategoryId,QuantityPerUnit,UnitPrice,UnitsInStock")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName", product.SupplierId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

		// 寫在原本的 Delete 旁邊，這個是專門接 Fetch 呼叫的 API
		[HttpPost]
		public async Task<IActionResult> DeleteApi(int id)
		{
			var product = await _context.Products.FindAsync(id);
			if (product == null)
			{
				return Json(new { success = false, message = "找不到該商品！" });
			}

			_context.Products.Remove(product);
			await _context.SaveChangesAsync();

			// 回傳 JSON 給前端 Fetch 接收
			return Json(new { success = true });
		}
	}
}
