using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC.Services.DL.ViewModels
{
    public class EditRoleViewModel
    {
        public EditRoleViewModel()
        {
            Users = new List<string>();
        }

        [Display(Name = "Role Id")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Role Name Field Required")]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }

        public List<string> Users { get; set; }
    }
}
