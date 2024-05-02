using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebBanGiay.Models
{
    public class EditUserModel
    {
        public EditUserModel()
        {
            // Khởi tạo AllRoles trong constructor
            AllRoles = new List<string>();
        }

        public string UserId { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "At least one role must be selected")]
        public List<string> Roles { get; set; }

        public List<string> AllRoles { get; set; }
    }
}