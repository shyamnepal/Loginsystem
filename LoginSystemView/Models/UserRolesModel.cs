using System.ComponentModel.DataAnnotations;

namespace LoginSystemView.Models
{

    public class UserRolesModel
    {

        public string Id { get; set; }
        [Required(ErrorMessage ="UserName is required")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }       
        public string RoleId { get; set; }


    }



}
