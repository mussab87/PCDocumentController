﻿using PC.Services.DL.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PC.Services.DL.Models
{
    public class Details
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int DetailsId { get; set; }
        public string Name { get; set; }

        [ForeignKey("ActivityId")]
        public virtual Activity Activity { get; set; }

        public string CreatedById { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }

        public string UpdatedById { get; set; }
        public virtual ApplicationUser UpdatedBy { get; set; }

    }
}
