using System.ComponentModel.DataAnnotations;

namespace LoginSystemView.Models
{
    public class AddRoles
    {
        [Required(ErrorMessage ="Role Name is required")]
        public string Name { get; set; }

    }
}
