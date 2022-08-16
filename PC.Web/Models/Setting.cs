using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PC.Web.Models
{
public class Setting
    {
        [Required(ErrorMessage = "Required Field")]
        [Display(Name = "Max Lock User")]
        public int AccessFailedCount { get; set; }

        [Required(ErrorMessage = " Required Field")]
        [Display(Name = "Password Length")]
        public int? UserPasswordLength { get; set; }

        [Required(ErrorMessage = "Required Field")]
        [Display(Name = "Enable Password Require Digits")]
        public bool? PassRequireDigit { get; set; }

        [Required(ErrorMessage = "Required Field")]
        [Display(Name = "Enable Password Require Lowercase")]
        public bool? PassRequireLowercase { get; set; }

        [Required(ErrorMessage = "Required Field")]
        [Display(Name = "Enable Password Require Uppercase")]
        public bool? PassRequireUppercase { get; set; }

        [Required(ErrorMessage = "Required Field")]
        [Display(Name = "Enable Password Require NonAlphanumeric ")]
        public bool? PassRequireNonAlphanumeric { get; set; }

        [Required(ErrorMessage = "Required Field")]
        [Display(Name = "User Session TimeOut / Minutes")]
        public int? UserSessionTimeOut { get; set; }

        [Required(ErrorMessage = "Required Field")]
        [Display(Name = "Max Limit ToLock User / Days")]
        public int? MaxLimitToLockUser { get; set; }

        [Required(ErrorMessage = " Required Field ")]
        [Display(Name = "System Policy Title")]
        public string UserConfirmPolicyTitle { get; set; }

        [Required(ErrorMessage = " Required Field ")]
        [Display(Name = "System Policies")]
        public string UserConfirmPolicy { get; set; }

        [Required(ErrorMessage = " Required Field ")]
        [Display(Name = "System Name")]
        public string ApplicationName { get; set; }

        [Required(ErrorMessage = " Required Field ")]
        [Display(Name = "System Logo")]
        public string ApplicationLogo { get; set; }

        [Required(ErrorMessage = " Required Field ")]
        [Display(Name = "Email API Link")]
        public string EmpDataApiUrl { get; set; }

        [Required(ErrorMessage = " Required Field ")]
        [Display(Name = "Enable System Policy")]
        public string EnableConfirmPolicy { get; set; }

        [Required(ErrorMessage = " Required Field ")]
        [Display(Name = "Close Right Click")]
        public string EnableRightClick { get; set; }
    }
}
