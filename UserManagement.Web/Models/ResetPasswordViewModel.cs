using System.ComponentModel.DataAnnotations;

namespace UserManagement.Web.Models
{
    public class ResetPasswordViewModel
    {
        [Display(Name = "Reset code")]
        [Required(ErrorMessage = "Code is required.")]
        public string ResetCode { get; set; }

        [Display(Name = "New password")]
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Password has a limit of 5 to 20 characters")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}