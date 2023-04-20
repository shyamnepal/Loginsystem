using System.ComponentModel.DataAnnotations;

namespace LoginSystemView.Models
{
    public class UserLogin
    {
        [Required(ErrorMessage ="UserName is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "password is required")]
        public string Password { get; set; }
    }
}
