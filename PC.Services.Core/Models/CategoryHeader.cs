using PC.Services.Core.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PC.Services.Core.Models
{
    public class CategoryHeader
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int CategoryHeaderId { get; set; }

        public int MainCategoryId { get; set; }

        [ForeignKey("MainCategoryId")]
        public virtual MainCategory MainCategory { get; set; }

        public int ActivityId { get; set; }

        [ForeignKey("ActivityId")]
        public virtual Activity Activity { get; set; }

        public int DetailsId { get; set; }

        [ForeignKey("DetailsId")]
        public virtual Details Details { get; set; }

        public int LevelCount { get; set; }

        public string CreatedById { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }

        public string UpdatedById { get; set; }
        public virtual ApplicationUser UpdatedBy { get; set; }

        public DateTime? CreatedDateTime { get; set; }
        public DateTime? UpdatedDateTime { get; set; }

        public IList<AuthorityMatrixCategoryHeader> AuthorityMatrixCategoryHeader { get; set; }

    }
}
