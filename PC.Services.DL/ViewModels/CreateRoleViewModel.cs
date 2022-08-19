using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC.Services.DL.ViewModels
{
    public class CreateRoleViewModel
    {
        [Required(ErrorMessage = "Role Name Field Required")]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }

        public List<IdentityRole> Roles { get; set; }
    }
}
