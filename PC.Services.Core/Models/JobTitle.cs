using PC.Services.Core.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PC.Services.Core.Models
{
    public class JobTitle
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int JobTitleId { get; set; }

        [Required(ErrorMessage = "Job Title Name Field Required")]
        [Display(Name = "Job Title Name")]
        public string JobTitleName { get; set; }

        public IList<UserJobTitle> UserJobTitle { get; set; }

        public string CreatedById { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }
        public string UpdatedById { get; set; }
        public virtual ApplicationUser UpdatedBy { get; set; }

        public DateTime? CreatedDateTime { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
    }
}
