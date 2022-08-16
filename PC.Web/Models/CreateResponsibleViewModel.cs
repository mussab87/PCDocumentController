using PC.Services.Core.Models;
using PC.Services.Core.Security;
using PC.Services.DL.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC.Web.Models
{
    public class CreateResponsibleViewModel
    {
        public CreateResponsibleViewModel()
        {
            AttachmentFile = new SingleFileModel();
            AuthorityDataNames = new AuthorityNames();
            UserLevels = new List<UserLevels>();
            TrsData = new List<TrsDetailData>();
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

        [Required(ErrorMessage = "Number of Workflow Levels")]
        [Display(Name = "Category")]
        public int LevelCount { get; set; }

        public string CreatedById { get; set; }

        public string UpdatedById { get; set; }

        public DateTime? CreatedDateTime { get; set; }
        public DateTime? UpdatedDateTime { get; set; }

        public AuthorityNames AuthorityDataNames { get; set; }

        public SingleFileModel AttachmentFile { get; set; }

        public string ResponsibleComments { get; set; }

        public IList<TrsDetailData> TrsData { get; set; }

        public IList<UserLevels> UserLevels { get; set; }
    }

    public class AuthorityNames
    {
        public AuthorityNames()
        {
            ////Claims = new List<string>();
            //categoryHeaders = new List<Category>();
        }

        public string authorityMatrixName { get; set; }
        public string mainCategoryName { get; set; }
        public string activityName { get; set; }
        public string detailsName { get; set; }

    }

    public class TrsDetailData
    {
        public TrsDetails TrsDetails { get; set; }
        public int ApprovalCount { get; set; }
        public IList<Approval> Approvals { get; set; }
        public IList<Attachment> FileAttachment { get; set; }
    }
}
