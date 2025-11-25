using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AirlineSeatReservationSystem.Models
{
    public class SignInViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name ="Password")]
        public string? Password { get; set; }
    }
}