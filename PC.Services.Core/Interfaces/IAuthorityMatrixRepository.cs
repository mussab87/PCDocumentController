using PC.Services.Core.Models;
using RepositoryPatternWithUOW.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC.Services.Core.Interfaces
{

    public interface IAuthorityMatrixRepository : IBaseRepository<AuthorityMatrix>
    {
        //IEnumerable<AuthorityMatrix> SpecialMethod();
    }
}
