using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Eshop.Infrastructure;

namespace Eshop.Web.ViewComponents
{
    public class CartIconViewComponent : ViewComponent
    {
        private readonly EshopDbContext _db;

        public CartIconViewComponent(EshopDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int itemCount = 0;

            if (HttpContext?.User?.Identity?.IsAuthenticated ?? false)
            {
                var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (Guid.TryParse(userIdClaim, out var userId))
                {
                    var cart = await _db.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);
                    if (cart != null)
                    {
                        itemCount = cart.Items.Sum(i => i.Quantity);
                    }
                }
            }

            return View(itemCount);
        }
    }
}
