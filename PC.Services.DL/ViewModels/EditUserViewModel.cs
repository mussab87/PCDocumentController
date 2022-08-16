using PC.Services.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC.Services.DL.ViewModels
{
    public class EditUserViewModel
    {
        public EditUserViewModel()
        {
            Claims = new List<string>();
            Roles = new List<string>();
        }

        public string Id { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "First Name Field Required")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name Field Required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone Number Field Required")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        //[Remote(action: "IsEmailInUse", controller: "Account")]
        public string Email { get; set; }

        [Required(ErrorMessage = "User Status Field Required")]
        [Display(Name = "User Status")]
        public bool? UserStatus { get; set; }

        [Required(ErrorMessage = "Job Title Field Required")]
        [Display(Name = "Job Title")]
        public int? Job { get; set; }

        public List<string> Claims { get; set; }

        public IList<string> Roles { get; set; }

        public IList<UserMatrix> UserAuthorityMatrix { get; set; }
    }
}
