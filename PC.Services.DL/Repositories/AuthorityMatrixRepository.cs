using PC.Services.Core.Interfaces;
using PC.Services.Core.Models;
using PC.Services.DL.DbContext;
using PC.Services.DL.Repositories;
using RepositoryPatternWithUOW.Core.Models;

namespace RepositoryPatternWithUOW.EF.Repositories
{
    public class AuthorityMatrixRepository : BaseRepository<AuthorityMatrix>, IAuthorityMatrixRepository
    {
        private readonly AppDBContext _context;

        public AuthorityMatrixRepository(AppDBContext context) : base(context)
        {
        }

        //public IEnumerable<AuthorityMatrix> SpecialMethod()
        //{
        //    throw new NotImplementedException();
        //}
    }
}