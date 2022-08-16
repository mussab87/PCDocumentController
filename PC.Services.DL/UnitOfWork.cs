using PC.Services.Core;
using PC.Services.Core.Interfaces;
using PC.Services.Core.Models;
using PC.Services.DL.DbContext;
using PC.Services.DL.Repositories;
using RepositoryPatternWithUOW.Core.Models;
using RepositoryPatternWithUOW.EF.Repositories;

namespace PC.Services.DL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDBContext _context;

        public IBaseRepository<Author> Authors { get; private set; }
        public IAuthorityMatrixRepository AuthorityMatrix { get; private set; }
        public IBaseRepository<JobTitle> JobTitle { get; private set; }

        public IBaseRepository<UserMatrix> UserMatrix { get; private set; }

        public IBaseRepository<MainCategory> MainCategory { get; private set; }

        public IBaseRepository<Details> Details { get; private set; }

        public IBaseRepository<Activity> Activity { get; private set; }

        public IBaseRepository<AuthorityMatrixCategoryHeader> AuthorityMatrixCategoryHeader { get; private set; }

        public IBaseRepository<CategoryHeader> CategoryHeader { get; private set; }

        public IBaseRepository<TrsDetails> TrsDetails { get; private set; }

        public IBaseRepository<Levels> Levels { get; private set; }

        public IBaseRepository<Attachment> Attachment { get; private set; }

        public IBaseRepository<Approval> Approval { get; private set; }

        public IBaseRepository<ApprovalStatus> ApprovalStatus { get; private set; }

        public UnitOfWork(AppDBContext context)
        {
            _context = context;

            Authors = new BaseRepository<Author>(_context);
            AuthorityMatrix = new AuthorityMatrixRepository(_context);
            JobTitle = new BaseRepository<JobTitle>(_context);
            UserMatrix = new BaseRepository<UserMatrix>(_context);

            MainCategory = new BaseRepository<MainCategory>(_context);
            Details = new BaseRepository<Details>(_context);
            Activity = new BaseRepository<Activity>(_context);

            AuthorityMatrixCategoryHeader = new BaseRepository<AuthorityMatrixCategoryHeader>(_context);
            CategoryHeader = new BaseRepository<CategoryHeader>(_context);
            TrsDetails = new BaseRepository<TrsDetails>(_context);

            Levels = new BaseRepository<Levels>(_context);
            Attachment = new BaseRepository<Attachment>(_context);

            Approval = new BaseRepository<Approval>(_context);
            ApprovalStatus = new BaseRepository<ApprovalStatus>(_context);
            //
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}