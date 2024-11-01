using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.DAL.Models.Users
{
    public class UserLogin
    {
        #region Scalar Properties

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Id incremental e automático
        [Display(Name = "UserId")]
        public int UserLoginId { get; set; }

        // Relação 1-N
        [ForeignKey("UserRole")]
        [Display(Name = "RoleId")]
        public int UserRoleId { get; set; }

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

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Password has a limit of 5 to 20 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Reset token")]
        [StringLength(100)]
        public string PasswordRecoveryToken { get; set; }

        [Display(Name = "Creation date")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Status")]
        public bool IsActived { get; set; }

        #endregion Scalar Properties

        #region Navigation Properties

        // Relação 1-1
        public virtual UserProfile UserProfile { get; set; }

        public virtual UserRole UserRole { get; set; }

        #endregion Navigation Properties
    }
}