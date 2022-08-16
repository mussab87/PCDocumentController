using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PC.Services.Core;
using PC.Services.Core.Models;
using PC.Services.Core.Security;
using PC.Services.DL.DbContext;

namespace PC.Services.DL.Interfaces.Repos
{
    public class UserAuthorityMatrixHelper : IUserAuthorityMatrixHelper
    {
        protected readonly UserManager<ApplicationUser> userManager;
        protected readonly RoleManager<IdentityRole> roleManager;
        protected readonly AppDBContext _context;

        protected readonly IUnitOfWork _unitOfWork;

        public UserAuthorityMatrixHelper(UserManager<ApplicationUser> userManager,
           RoleManager<IdentityRole> roleManager,
           AppDBContext context,
           IUnitOfWork unitOfWork)

        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<AuthorityMatrix>> getUserAuthorityMatrixAsync(ApplicationUser User)
        {
            //var LoggedInuser = await userManager.FindByIdAsync(User.Id);

            // Get Logged In user authority matrix permission
            var authorityMatrices = new List<AuthorityMatrix>();
            var userMatrix = await _unitOfWork.UserMatrix.FindAllAsync(criteria: q => q.Id == User.Id);
            if (userMatrix != null)
            {
                foreach (var authority in userMatrix.ToList())
                {
                    //AuthorityMatrix authorityMatrix = new AuthorityMatrix();
                    var UserAuthorityMatrix =
                        await _unitOfWork.AuthorityMatrix.GetByIdAsync(authority.AuthorityId);

                    authorityMatrices.Add(UserAuthorityMatrix);
                }
                return authorityMatrices;
            }
            return null;
        }
    }
}
