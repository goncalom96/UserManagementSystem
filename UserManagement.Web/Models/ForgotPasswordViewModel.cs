using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UserManagement.Web.Models
{
    public class ForgotPasswordViewModel
    {
        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "E-mail is required.")]
        [StringLength(maximumLength: 100, MinimumLength = 5, ErrorMessage = "Email has a limit of 5 to 100 characters")]
        public string Email { get; set; }
    }
}