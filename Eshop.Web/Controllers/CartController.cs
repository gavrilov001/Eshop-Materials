using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Eshop.Infrastructure;
using Eshop.Domain.Entities;

namespace Eshop.Web.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly EshopDbContext _db;

        public CartController(EshopDbContext db)
        {
            _db = db;
        }

        private Guid CurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return idClaim != null ? Guid.Parse(idClaim) : Guid.Empty;
        }

        public IActionResult Index()
        {
            var userId = CurrentUserId();
            
            // Поправено: Вчитување на Product релацијата
            var model = _db.CartItems
                .Include(ci => ci.Product)
                .Include(ci => ci.Cart)
                .Where(ci => ci.Cart.UserId == userId)
                .Select(ci => new 
                {
                    ci.Id,
                    ci.ProductId,
                    ci.Product,
                    ci.Quantity,
                    ci.Price
                })
                .ToList();

            return View(model);
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            var userId = CurrentUserId();
            var items = _db.CartItems
                .Include(ci => ci.Cart)
                .Where(ci => ci.Cart.UserId == userId)
                .ToList();
                
            if (!items.Any())
            {
                TempData["Message"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }

            var vm = new Eshop.Web.Models.CheckoutViewModel();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(Eshop.Web.Models.CheckoutViewModel model)
        {
            var userId = CurrentUserId();
            var items = _db.CartItems
                .Include(ci => ci.Cart)
                .Where(ci => ci.Cart.UserId == userId)
                .ToList();
                
            if (!items.Any())
            {
                ModelState.AddModelError(string.Empty, "Cart is empty.");
                return View(model);
            }

            if (!ModelState.IsValid) return View(model);

            // Create order (без UserId ако не постои)
            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerName = model.Name,
                CustomerSurname = model.Surname,
                CustomerPhone = model.Phone,
                CustomerAddress = model.Address,
                DateCreated = DateTime.Now,
                TotalPrice = items.Sum(i => i.Price * i.Quantity)
            };

            foreach (var ci in items)
            {
                var oi = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    Price = ci.Price
                };
                order.Items.Add(oi);
            }

            _db.Orders.Add(order);

            // Поправено: Побрза и посигурна логика за бришење
            var cart = _db.Carts
                .Include(c => c.Items)
                .FirstOrDefault(c => c.UserId == userId);
                
            if (cart != null)
            {
                _db.CartItems.RemoveRange(cart.Items);
                _db.Carts.Remove(cart);
            }

            _db.SaveChanges();

            TempData["SuccessMessage"] = "Успешно порачавте материјал";
            return RedirectToAction("Success");
        }

        public IActionResult Success()
        {
            ViewBag.Message = TempData["SuccessMessage"];
            return View();
        }

        [HttpPost]
        public IActionResult Add(Guid productId, int quantity)
        {
            if (quantity <= 0) quantity = 1;
            var userId = CurrentUserId();
            if (userId == Guid.Empty) return Challenge();

            // Поправено: Вчитување на Items за да се избегне lazy loading
            var cart = _db.Carts
                .Include(c => c.Items)
                .FirstOrDefault(c => c.UserId == userId);
                
            if (cart == null)
            {
                cart = new Cart { Id = Guid.NewGuid(), UserId = userId };
                _db.Carts.Add(cart);
                _db.SaveChanges(); // Поправено: Снимање на кошничката веднаш
            }

            var product = _db.Products.Find(productId);
            if (product == null) return NotFound();

            var item = cart.Items.FirstOrDefault(ci => ci.ProductId == productId);
            if (item == null)
            {
                item = new CartItem
                {
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity,
                    Price = product.Price
                };
                _db.CartItems.Add(item);
            }
            else
            {
                item.Quantity += quantity;
            }

            _db.SaveChanges();

            // If this is an AJAX request, return JSON so the client can stay on the products page
            var isAjax = Request.Headers.ContainsKey("X-Requested-With") &&
                         Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (isAjax)
            {
                // Compute cart item count for the user to update the badge
                var cartItemCount = _db.CartItems
                    .Include(ci => ci.Cart)
                    .Where(ci => ci.Cart.UserId == cart.UserId)
                    .Sum(ci => ci.Quantity);

                return Json(new { success = true, message = "Производот е додаден во кошничката!", cartItemCount });
            }

            TempData["SuccessMessage"] = "Производот е додаден во кошничката!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Update(Guid id, int quantity)
        {
            var userId = CurrentUserId();
            var item = _db.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefault(ci => ci.Id == id && ci.Cart.UserId == userId);
                
            if (item == null) return NotFound();
            
            if (quantity <= 0)
            {
                _db.CartItems.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
            }
            
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Remove(Guid id)
        {
            var userId = CurrentUserId();
            var item = _db.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefault(ci => ci.Id == id && ci.Cart.UserId == userId);
                
            if (item != null)
            {
                _db.CartItems.Remove(item);
                _db.SaveChanges();
            }
            
            return RedirectToAction("Index");
        }
    }
}