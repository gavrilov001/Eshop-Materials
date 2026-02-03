using System;

namespace Eshop.Domain.Entities
{
    public class Admin
    {
        public Guid Id { get; set; }
        // Use Email for registration/login
        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;
    }
}
