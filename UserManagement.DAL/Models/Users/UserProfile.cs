using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.DAL.Models.Users
{
    public class UserProfile
    {
        #region Enums

        public enum EnumGender
        {
            Male,
            Female = 1,
            Unknown = 2,
            Other = 3
        }

        #endregion Enums

        #region Scalar Properties

        // Relação 1-1
        [Key, ForeignKey("UserLogin")]
        [Display(Name = "UserId")]
        public int UserLoginId { get; set; }

        [Display(Name = "First name")]
        [Required(ErrorMessage = "FirstName is required.")]
        [StringLength(30)]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        [Required(ErrorMessage = "LastName is required.")]
        [StringLength(30)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        [EnumDataType(typeof(EnumGender))]
        public EnumGender Gender { get; set; }

        [Display(Name = "Date of Birth")]
        [Required(ErrorMessage = "Date of Birth is required.")]
        [Column(TypeName = "date")] // Grava apenas a data na DB em vez de gravar data e hora (datetime)
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Picture")]
        [StringLength(200)]
        public string ProfilePictureUrl { get; set; }

        public DateTime LastModified { get; set; }

        #endregion Scalar Properties

        #region Navigation Properties

        public virtual UserLogin UserLogin { get; set; }

        #endregion Navigation Properties
    }
}