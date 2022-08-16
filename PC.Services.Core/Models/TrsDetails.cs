﻿using PC.Services.Core.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PC.Services.Core.Models
{
    public class TrsDetails
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int TrsDetailsId { get; set; }
        public int CategoryHeaderId { get; set; }

        public string ResponsibleComments { get; set; }

        [ForeignKey("CategoryHeaderId")]
        public virtual CategoryHeader CategoryHeader { get; set; }

        public string CreatedById { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }

        public string UpdatedById { get; set; }
        public virtual ApplicationUser UpdatedBy { get; set; }

        public DateTime? CreatedDateTime { get; set; }
        public DateTime? UpdatedDateTime { get; set; }

    }
}
