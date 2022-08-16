using Microsoft.AspNetCore.Identity;
using PC.Services.Core.Interfaces;
using PC.Services.Core.Models;
using PC.Services.Core.Security;
using RepositoryPatternWithUOW.Core.Models;

namespace PC.Services.Core
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepository<Author> Authors { get; }
        IBaseRepository<JobTitle> JobTitle { get; }
        IBaseRepository<MainCategory> MainCategory { get; }
        IBaseRepository<Details> Details { get; }
        IBaseRepository<Activity> Activity { get; }
        IBaseRepository<UserMatrix> UserMatrix { get; }
        IBaseRepository<AuthorityMatrixCategoryHeader> AuthorityMatrixCategoryHeader { get; }
        IBaseRepository<CategoryHeader> CategoryHeader { get; }
        IBaseRepository<TrsDetails> TrsDetails { get; }
        IBaseRepository<Levels> Levels { get; }
        IBaseRepository<Attachment> Attachment { get; }
        IBaseRepository<Approval> Approval { get; }
        IBaseRepository<ApprovalStatus> ApprovalStatus { get; }

        IAuthorityMatrixRepository AuthorityMatrix { get; }

        int Complete();
    }
}