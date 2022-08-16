using PC.Services.DL.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PC.Services.DL.Models
{
    public class Approval
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int ApprovalId { get; set; }

        [ForeignKey("TrsDetailsId")]
        public virtual TrsDetails TrsDetails { get; set; }

        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        public string ApprovalStatusId { get; set; }
        public virtual ApprovalStatus ApprovalStatus { get; set; }

        public string RejectionReason { get; set; }

        public string ConsultedComments { get; set; }

        public DateTime ApprovalDateTime { get; set; }

        public string CreatedById { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }

        public string UpdatedById { get; set; }
        public virtual ApplicationUser UpdatedBy { get; set; }

    }
}
