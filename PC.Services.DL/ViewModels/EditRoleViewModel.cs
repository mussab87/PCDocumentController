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

        [Display(Name = "رقم الصلاحية")]
        public string Id { get; set; }

        [Required(ErrorMessage = "اسم الصلاحيةحقل مطلوب")]
        [Display(Name = "إسم الصلاحية")]
        public string RoleName { get; set; }

        public List<string> Users { get; set; }
    }
}
