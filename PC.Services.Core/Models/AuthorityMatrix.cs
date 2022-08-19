using PC.Services.Core.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PC.Services.Core.Models
{
    public class AuthorityMatrix
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int AuthorityId { get; set; }

        [Required(ErrorMessage = "Authority Name Field Required")]
        [Display(Name = "Authority Name")]
        public string Name { get; set; }

        public IList<AuthorityMatrixCategoryHeader> AuthorityMatrixCategoryHeader { get; set; }
        public IList<UserMatrix> UserMatrix { get; set; }

        public string CreatedById { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }

        public string UpdatedById { get; set; }
        public virtual ApplicationUser UpdatedBy { get; set; }

        public DateTime? CreatedDateTime { get; set; }
        public DateTime? UpdatedDateTime { get; set; }

    }
}
