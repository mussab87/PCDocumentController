using Microsoft.AspNetCore.Identity;
using PC.Services.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace PC.Services.DL.ViewModels
{
    public class AccountViewModel
    {                
        [Required(ErrorMessage = "Username Field Required")]
        //[Remote(action: "IsEmailInUse", controller: "Account")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password Field Required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [System.ComponentModel.DataAnnotations.Compare("Password",
            ErrorMessage = "Password and Confirm Password not match")]
        public string ConfirmPassword { get; set; }

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
        public int Job { get; set; }

        public List<IdentityRole> Roles { get; set; }
        public List<JobTitle> JobTitle { get; set; }

    }
}
