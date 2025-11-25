
using System.ComponentModel.DataAnnotations;

namespace AirlineSeatReservationSystem.Models
{
        public class SignUpViewModel
        {

                [Required]
                [Display(Name = "UserName")]
                public string? UserName { get; set; }

                [Required]
                [Display(Name = "Phone")]
                public string? Phone { get; set; }

                [Required]
                [EmailAddress]
                [Display(Name = "E-mail")]
                public string? Email { get; set; }

                [Required]
                [DataType(DataType.Password)]
                [Display(Name = "Password")]
                public string? Password { get; set; }

                [Required]
                [DataType(DataType.Password)]
                [Compare(nameof(Password), ErrorMessage = "Your password does not match.")]
                [Display(Name = "Password Repeat ")]
                public string? ConfirmPassword { get; set; }
        }
}