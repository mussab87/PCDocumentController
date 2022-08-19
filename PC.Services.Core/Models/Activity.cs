using PC.Services.Core.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PC.Services.Core.Models
{
    public class Activity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int ActivityId { get; set; }

        [Required(ErrorMessage = "ActivityName Field Required")]
        [Display(Name = "Activity Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Main Category Field Required")]
        [Display(Name = "Main Category")]
        public int MainCategoryId { get; set; }
        [ForeignKey("MainCategoryId")]
        public virtual MainCategory MainCategory { get; set; }

        public DateTime? CreatedDateTime { get; set; }
        public DateTime? UpdatedDateTime { get; set; }

        public string CreatedById { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }

        public string UpdatedById { get; set; }
        public virtual ApplicationUser UpdatedBy { get; set; }

    }
}
