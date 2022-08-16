using Microsoft.EntityFrameworkCore;
using PC.Services.Core.Models;

namespace PC.Services.DL.DbContext
{
    public class AppDBContext : AuditableIdentityContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options)
        : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AuthorityMatrixCategoryHeader>().HasKey(sc => new { sc.AuthorityId, sc.CategoryHeaderId });
            modelBuilder.Entity<UserMatrix>().HasKey(sc => new { sc.Id, sc.AuthorityId });
            modelBuilder.Entity<UserJobTitle>().HasKey(sc => new { sc.Id, sc.JobTitleId });

            //change AspNet Users tables names
            var entityTypes = modelBuilder.Model.GetEntityTypes();
            foreach (var entityType in entityTypes)
                modelBuilder.Entity(entityType.ClrType)
                       .ToTable(entityType.GetTableName().Replace("AspNet", ""));
        }

        public DbSet<Activity> Activity { get; set; }
        public DbSet<Approval> approval { get; set; }
        public DbSet<ApprovalStatus> ApprovalStatus { get; set; }
        public DbSet<Attachment> Attachment { get; set; }
        public DbSet<AuthorityMatrix> AuthorityMatrix { get; set; }
        public DbSet<AuthorityMatrixCategoryHeader> AuthorityMatrixCategoryHeader { get; set; }
        public DbSet<CategoryHeader> CategoryHeader { get; set; }
        public DbSet<Details> Details { get; set; }
        public DbSet<JobTitle> jobTitle { get; set; }
        public DbSet<Levels> Levels { get; set; }
        public DbSet<MainCategory> MainCategory { get; set; }
        public DbSet<TrsDetails> TrsDetails { get; set; }
        public DbSet<UserJobTitle> UserJobTitle { get; set; }
        public DbSet<UserMatrix> UserMatrix { get; set; }
        public DbSet<UserPassword> UserPassword { get; set; }
        public DbSet<UserTransaction> UserTransaction { get; set; }
    }
}
