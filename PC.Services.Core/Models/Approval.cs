using PC.Services.Core.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PC.Services.Core.Models
{
    public class Approval
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int ApprovalId { get; set; }

        public int TrsDetailsId { get; set; }

        [ForeignKey("TrsDetailsId")]
        public virtual TrsDetails TrsDetails { get; set; }

        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        public int ApprovalStatusId { get; set; }
        public virtual ApprovalStatus ApprovalStatus { get; set; }

        public string RejectionReason { get; set; }

        public string ConsultedComments { get; set; }

        public DateTime? ApprovalDateTime { get; set; }

        public string CreatedById { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }

        public string UpdatedById { get; set; }
        public virtual ApplicationUser UpdatedBy { get; set; }

        public DateTime? CreatedDateTime { get; set; }
        public DateTime? UpdatedDateTime { get; set; }

    }
}
