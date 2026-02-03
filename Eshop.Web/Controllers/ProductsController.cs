using Microsoft.AspNetCore.Mvc;
using Eshop.Infrastructure;

namespace Eshop.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly EshopDbContext _db;

        public ProductsController(EshopDbContext db)
        {
            _db = db;
        }

        // Optional filtering by category and subcategory
        public IActionResult Index(string? category = null, string? subcategory = null)
        {
            var q = _db.Products.AsQueryable();

            // If a specific subcategory was requested, filter exactly by that
            if (!string.IsNullOrWhiteSpace(subcategory))
            {
                q = q.Where(p => p.Category == subcategory);
                ViewBag.CategoryTitle = subcategory;
            }
            else if (!string.IsNullOrWhiteSpace(category))
            {
                // Special handling for top-level grouping "Градежни материјали"
                if (string.Equals(category, "Градежни материјали", StringComparison.OrdinalIgnoreCase))
                {
                    var subs = new[] { "Песок / Цемент", "Железо", "Блокови", "Керамиди и покривки" };
                    q = q.Where(p => subs.Contains(p.Category));
                }
                else
                {
                    q = q.Where(p => p.Category == category);
                }

                ViewBag.CategoryTitle = category;
            }
            else
            {
                ViewBag.CategoryTitle = "Производи";
            }

            var products = q.ToList();
            return View(products);
        }
    }
}
