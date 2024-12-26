
using System.ComponentModel.DataAnnotations;

namespace NiceAdmin.Models
{
    public class UserModel
    {
        [Key]
        public int UserId { get; set; }

        [Display(Name = "User Name")]
        [Required(ErrorMessage = "Enter Name")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string MobileNo { get; set; }
        [Required]
        public string Address { get; set; }
        public bool IsActive { get; set; }

        //public int? RoleID { get; set; }
        
        //public IFormFile? file { get; set; }
        //public string? PhotoPath { get; set; }
    }

    public class UserDropDownModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
    }
}

public class UserLoginModel
{
    [Required(ErrorMessage = "Username is required.")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; }
}
