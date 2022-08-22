using PC.Services.Core.Models;
using PC.Services.Core.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC.Services.DL.ViewModels
{
    public class CategoryHeaderViewModel
    {
        public CategoryHeaderViewModel()
        {
            //Claims = new List<string>();
            Roles = new List<string>();
            users = new List<ApplicationUser>();
            UserLevels = new List<UserLevels>();
            AuthorityDataNames = new AuthorityDataNames();
        }

        public int CategoryHeaderId { get; set; } = 0;

        //link MainCategory with AuthorityMatrix
        [Required(ErrorMessage = "Authority Matrix Field Required")]
        [Display(Name = "Authority Matrix")]
        public int AuthorityId { get; set; }

        [Required(ErrorMessage = "Main Category Field Required")]
        [Display(Name = "Category")]
        public int MainCategoryId { get; set; }

        [Required(ErrorMessage = "Activity Field Required")]
        [Display(Name = "Activity")]
        public int ActivityId { get; set; }

        [Required(ErrorMessage = "Dteails Field Required")]
        [Display(Name = "Dteails")]
        public int DetailsId { get; set; }

        [Required(ErrorMessage = "Number of Workflow Levels Field Required")]
        [Display(Name = "Number of Workflow Levels")]
        public int LevelCount { get; set; }

        //[Required(ErrorMessage = "Workflow Level User Field Required")]
        [Display(Name = "Workflow Level User")]
        public string LevelUser { get; set; }

        //[Required(ErrorMessage = "Workflow Level Role Field Required")]
        [Display(Name = "Workflow Level Role")]
        public string LevelRole { get; set; }


        public string CreatedById { get; set; }

        public string UpdatedById { get; set; }

        public DateTime? CreatedDateTime { get; set; }
        public DateTime? UpdatedDateTime { get; set; }

        public IList<UserLevels> UserLevels { get; set; }

        public IList<ApplicationUser> users { get; set; }
        public IList<string> Roles { get; set; }

        public AuthorityDataNames AuthorityDataNames { get; set; }
    }

    public class AuthorityDataNames
    {
        public AuthorityDataNames()
        {
            ////Claims = new List<string>();
            //categoryHeaders = new List<Category>();
        }

        public string authorityMatrixName { get; set; }
        public string mainCategoryName { get; set; }
        public string activityName { get; set; }
        public string detailsName { get; set; }

    }

    public class Category
    {
        public CategoryHeader categoryHeaders { get; set; }

        public AuthorityMatrixCategoryHeader authority { get; set; }

    }

    public class UserLevels
    {
        public Levels Level { get; set; }

        public string roleId { get; set; }
        public string roleName { get; set; }

    }
}
