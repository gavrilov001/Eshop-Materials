using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Eshop.Infrastructure;
using Eshop.Domain.Entities;

namespace Eshop.Web.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class ProductAdminController : Controller
    {
        private readonly EshopDbContext _db;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _env;

        public ProductAdminController(EshopDbContext db, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        private bool IsAuthorizedAdmin()
        {
            return User.Identity?.Name?.ToLower() == "vlatko.gavr@hotmail.com";
        }

        public IActionResult Index()
        {
            var products = _db.Products.ToList();
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product model)
        {
            if (!ModelState.IsValid) return View(model);
            model.Id = Guid.NewGuid();
            _db.Products.Add(model);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        // Development-only helper to create a product without requiring authentication.
        // This is useful for debugging on local machines where cookie login may be problematic.
        [HttpPost]
        [AllowAnonymous]
        public IActionResult CreateNoAuth(Product model)
        {
            if (!_env.IsDevelopment()) return Forbid();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            model.Id = Guid.NewGuid();
            _db.Products.Add(model);
            _db.SaveChanges();
            return Ok(new { success = true, id = model.Id });
        }

        public IActionResult Edit(Guid id)
        {
            var product = _db.Products.Find(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product model)
        {
            if (!ModelState.IsValid) return View(model);
            _db.Products.Update(model);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(Guid id)
        {
            var p = _db.Products.Find(id);
            if (p != null)
            {
                _db.Products.Remove(p);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
