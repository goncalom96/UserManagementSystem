using System.ComponentModel.DataAnnotations;

namespace UserManagement.Web.Models
{
    public class LoginViewModel
    {
        [Display(Name = "Username")]
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(maximumLength: 15, ErrorMessage = "Username must be a string with a maximum length of 15")]
        public string UserName { get; set; }

        //[Display(Name = "E-mail")]
        //[Required(ErrorMessage = "E-mail is required.")]
        //[StringLength(maximumLength: 100, MinimumLength = 5, ErrorMessage = "Email has a limit of 5 to 100 characters")]
        //public string Email { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Password has a limit of 5 to 20 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}