using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC.Services.Core.Models
{
   public class UserTransaction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int Id { get; set; }
        public string UserId { get; set; }        
        public DateTime? LogginDateTime { get; set; }
        public DateTime? LoggedOutDateTime { get; set; }
        
    }
}
