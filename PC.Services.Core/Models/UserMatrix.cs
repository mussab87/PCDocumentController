using PC.Services.Core.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PC.Services.Core.Models
{
    public class UserMatrix
    {
        public string Id { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public int AuthorityId { get; set; }
        public AuthorityMatrix AuthorityMatrix { get; set; }

    }
}
