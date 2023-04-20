using System.ComponentModel.DataAnnotations;

namespace LoginSystemView.Models
{
    public class RoleAssignModel
    {
     
        [Required(ErrorMessage ="UserName is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Role Name is required")]
        public string Name { get; set; }
    }
}
