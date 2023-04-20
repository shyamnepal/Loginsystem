using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LoginSystemView.Models
{
    public class User
    {

        [Required(ErrorMessage ="first Name is required")]
        public string FirstName { get; set; }
        
        [Required(ErrorMessage ="Last Name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage ="Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone Number is required")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage="UserName is required")]
        public string UserName { get; set; }

        [Required (ErrorMessage = "Password is required")]
        public string PasswordHash { get; set; }

    }
}
