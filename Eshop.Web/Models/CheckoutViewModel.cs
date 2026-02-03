using System.ComponentModel.DataAnnotations;

namespace Eshop.Web.Models
{
    public class CheckoutViewModel
    {
        [Required]
        [Display(Name = "Име")]
        public string Name { get; set; } = null!;

        [Required]
        [Display(Name = "Презиме")]
        public string Surname { get; set; } = null!;

        [Required]
        [Display(Name = "Број")]
        public string Phone { get; set; } = null!;

        [Required]
        [Display(Name = "Адреса")]
        public string Address { get; set; } = null!;

        // Total amount (computed from the current cart) - not required from user input
        [Display(Name = "Вкупно за плаќање")]
        public decimal Total { get; set; }
    }
}
