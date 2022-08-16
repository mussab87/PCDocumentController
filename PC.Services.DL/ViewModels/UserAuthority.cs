using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC.Services.DL.ViewModels
{
    public class UserAuthority
    {
        public int AuthorityId { get; set; }
        public string AuthorityName { get; set; }
        public bool IsSelected { get; set; }
    }
}
