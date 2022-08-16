using PC.Services.DL.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PC.Services.DL.Models
{
    public class CategoryHeader
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int CategoryHeaderId { get; set; }

        [ForeignKey("MainCategoryId")]
        public virtual MainCategory MainCategory { get; set; }

        [ForeignKey("ActivityId")]
        public virtual Activity Activity { get; set; }

        [ForeignKey("DetailsId")]
        public virtual Details Dteails { get; set; }

        public int LevelCount { get; set; }

        public string CreatedById { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }

        public string UpdatedById { get; set; }
        public virtual ApplicationUser UpdatedBy { get; set; }

        public IList<AuthorityMatrixCategoryHeader> AuthorityMatrixCategoryHeader { get; set; }

    }
}
