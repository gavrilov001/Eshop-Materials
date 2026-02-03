using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Eshop.Infrastructure;
using Eshop.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Eshop.Web.Controllers
{
    public class AccountController : Controller
    {
    private readonly EshopDbContext _db;
    private readonly PasswordHasher<Eshop.Domain.Entities.User> _hasher = new PasswordHasher<Eshop.Domain.Entities.User>();

        public AccountController(EshopDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(string.Empty, "Email and password are required.");
                return View();
            }
            if (_db.Users.Any(u => u.Email == email))
            {
                ModelState.AddModelError(string.Empty, "Email already exists.");
                return View();
            }

            var user = new Eshop.Domain.Entities.User
            {
                Id = Guid.NewGuid(),
                Email = email
            };

            user.PasswordHash = _hasher.HashPassword(user, password);
            _db.Users.Add(user);

            // If this is the special admin email, ensure an Admin record exists
            const string superAdminEmail = "vlatko.gavr@hotmail.com";
            if (string.Equals(email, superAdminEmail, StringComparison.OrdinalIgnoreCase))
            {
                if (!_db.Admins.Any(a => a.Email == email))
                {
                    var admin = new Admin { Id = Guid.NewGuid(), Email = email };
                    _db.Admins.Add(admin);
                }
            }

            _db.SaveChanges();

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            var user = _db.Users.SingleOrDefault(u => u.Email == email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View();
            }

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            // If this user is an admin (special email), add Admin role
            if (_db.Admins.Any(a => a.Email == email))
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
