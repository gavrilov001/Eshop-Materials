using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Eshop.Infrastructure;

namespace Eshop.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly EshopDbContext _db;

        public OrdersController(EshopDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var orders = _db.Orders.OrderByDescending(o => o.DateCreated).ToList();
            return View(orders);
        }

        public IActionResult Details(Guid id)
        {
            // Include items and the related products so admin can see exactly what was ordered
            var order = _db.Orders
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefault(o => o.Id == id);

            if (order == null) return NotFound();
            return View(order);
        }

        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var order = _db.Orders.FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Eshop.Domain.Entities.Order model)
        {
            var order = _db.Orders.FirstOrDefault(o => o.Id == model.Id);
            if (order == null) return NotFound();

            // update editable fields
            order.CustomerName = model.CustomerName;
            order.CustomerSurname = model.CustomerSurname;
            order.CustomerPhone = model.CustomerPhone;
            order.CustomerAddress = model.CustomerAddress;

            _db.Orders.Update(order);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Guid id)
        {
            var order = _db.Orders.Include(o => o.Items).FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound();

            // remove related order items first (migration uses FK without cascade)
            if (order.Items != null && order.Items.Any())
            {
                _db.OrderItems.RemoveRange(order.Items);
            }

            _db.Orders.Remove(order);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
