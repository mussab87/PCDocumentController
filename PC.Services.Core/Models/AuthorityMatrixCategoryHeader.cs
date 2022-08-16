using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PC.Services.Core.Models
{
    public class AuthorityMatrixCategoryHeader
    {
        public int AuthorityId { get; set; }
        public AuthorityMatrix AuthorityMatrix { get; set; }

        public int CategoryHeaderId { get; set; }
        public CategoryHeader CategoryHeader { get; set; }
    }
}
