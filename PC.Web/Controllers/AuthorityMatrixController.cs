using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PC.Services.Core;
using PC.Services.Core.Helper.Consts;
using PC.Services.Core.Models;
using PC.Services.Core.Security;
using PC.Services.DL.DbContext;
using System.Security.Claims;

namespace PC.Web.Controllers
{
    //Audit when save add this await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
    [Authorize(Roles = SD.Admin)]
   //[AllowAnonymous]
    public class AuthorityMatrixController : BaseController
    {
        public AuthorityMatrixController(UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager,
           RoleManager<IdentityRole> roleManager,
           AppDBContext context,
           IConfiguration config,
           IUnitOfWork unitOfWork) : base(userManager, signInManager, roleManager, context, config, unitOfWork)
        {
        }

        #region AuthorityMatrix 
        /**************AuthorityMatrix Section******************************************/

        [HttpGet]
        //[Authorize("ListRoles-AdminController")]
        public async Task<IActionResult> Index()
        {
            return View(await _unitOfWork.AuthorityMatrix.GetAllAsync());
            //return View(await _context.jobTitle.ToListAsync());
        }

        //[AllowAnonymous]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AuthorityMatrix authorityMatrix)
        {
            if (ModelState.IsValid)
            {
                var LoggedInuser = await userManager.GetUserAsync(User);
                authorityMatrix.CreatedBy = LoggedInuser;
                authorityMatrix.CreatedById = LoggedInuser.Id;
                authorityMatrix.CreatedDateTime = DateTime.Now;

                await _unitOfWork.AuthorityMatrix.AddAsync(authorityMatrix);
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                return RedirectToAction(nameof(Index));
            }
            return View(authorityMatrix);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var authorityMatrix = await _unitOfWork.AuthorityMatrix.GetByIdAsync(id);
            if (authorityMatrix == null)
            {
                return NotFound();
            }
            return View(authorityMatrix);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int AuthorityId, AuthorityMatrix authorityMatrix)
        {
            if (AuthorityId != authorityMatrix.AuthorityId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var LoggedInuser = await userManager.GetUserAsync(User);
                    authorityMatrix.UpdatedBy = LoggedInuser;
                    authorityMatrix.UpdatedById = LoggedInuser.Id;
                    authorityMatrix.UpdatedDateTime = DateTime.Now;


                    var oldJobTitle = await _unitOfWork.AuthorityMatrix.GetByIdAsync(AuthorityId);
                    _context.Entry(oldJobTitle).CurrentValues.SetValues(authorityMatrix);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!authorityMatrixExists(authorityMatrix.AuthorityId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(authorityMatrix);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {            
            var authorityMatrix = await _unitOfWork.AuthorityMatrix.GetByIdAsync(id);
            _unitOfWork.AuthorityMatrix.Delete(authorityMatrix);

            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

            return RedirectToAction(nameof(Index));
        }


        private bool authorityMatrixExists(int id)
        {
            return _context.AuthorityMatrix.Any(e => e.AuthorityId == id);
        }

        /**************End AuthorityMatrix Section******************************************/
        #endregion



    }
}
