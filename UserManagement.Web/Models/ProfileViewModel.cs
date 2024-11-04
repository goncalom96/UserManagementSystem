using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using static UserManagement.DAL.Models.Users.UserProfile;

namespace UserManagement.Web.Models
{
    public class ProfileViewModel
    {
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
        [Column(TypeName = "date")]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Image")]
        public HttpPostedFileBase ImageFile { get; set; }
    }
}