using PC.Services.DL.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PC.Services.DL.Models
{
    public class UserJobTitle
    {
        public string Id { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public int JobTitleId { get; set; }
        public JobTitle JobTitle { get; set; }

    }
}
