using PC.Services.Core.Models;
using PC.Services.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PC.Services.DL.Interfaces
{
    public interface IUserAuthorityMatrixHelper
    {
        public Task<List<AuthorityMatrix>> getUserAuthorityMatrixAsync(ApplicationUser User);
    }
}
