using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.DAL.Models.Users
{
    public class UserRole
    {
        #region Enums
        public enum EnumRole
        {
            Administrator = 1,
            Guest = 2,
            Employee = 3
        }

        #endregion
        #region Scalar Properties

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserRoleId { get; set; }

        [Required(ErrorMessage = "Role name is required.")]
        [EnumDataType(typeof(EnumRole))]
        [Display(Name = "Role")]
        public EnumRole RoleType { get; set; }

        #endregion Scalar Properties

        #region Navigation Properties

        // Relação 1-N
        public virtual ICollection<UserLogin> UserLogins { get; set; }

        #endregion Navigation Properties
    }
}