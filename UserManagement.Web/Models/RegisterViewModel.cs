using System.ComponentModel.DataAnnotations;

namespace UserManagement.Web.Models
{
    public class RegisterViewModel
    {
        [Display(Name = "Username")]
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(maximumLength: 15, ErrorMessage = "Username must be a string with a maximum length of 15")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "E-mail is required.")]
        [StringLength(maximumLength: 100, MinimumLength = 5, ErrorMessage = "E-mail has a limit of 5 to 100 characters")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string EmailAddress { get; set; }

        [Display(Name = "Phone number")]
        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"([0-9]+)", ErrorMessage = "Must be a Number.")]
        [MaxLength(9, ErrorMessage = "9 character limit.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Password has a limit of 5 to 20 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}