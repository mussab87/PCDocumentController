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
    public class MainCategoryController : BaseController
    {
        public MainCategoryController(UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager,
           RoleManager<IdentityRole> roleManager,
           AppDBContext context,
           IConfiguration config,
           IUnitOfWork unitOfWork) : base(userManager, signInManager, roleManager, context, config, unitOfWork)
        {
        }

        #region MainCategory
        /**************MainCategory Section******************************************/

        [HttpGet]
        //[Authorize("ListRoles-AdminController")]
        public async Task<IActionResult> Index()
        {
            return View(await _unitOfWork.MainCategory.GetAllAsync());
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
        public async Task<IActionResult> Create(MainCategory model)
        {
            if (ModelState.IsValid)
            {
                var LoggedInuser = await userManager.GetUserAsync(User);
                model.CreatedBy = LoggedInuser;
                model.CreatedById = LoggedInuser.Id;
                model.CreatedDateTime = DateTime.Now;

                await _unitOfWork.MainCategory.AddAsync(model);
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mainCategory = await _unitOfWork.MainCategory.GetByIdAsync(id);
            if (mainCategory == null)
            {
                return NotFound();
            }
            return View(mainCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int MainCategoryId, MainCategory mainCategory)
        {
            if (MainCategoryId != mainCategory.MainCategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var LoggedInuser = await userManager.GetUserAsync(User);
                    mainCategory.UpdatedBy = LoggedInuser;
                    mainCategory.UpdatedById = LoggedInuser.Id;
                    mainCategory.UpdatedDateTime = DateTime.Now;


                    var oldmainCategory = await _unitOfWork.MainCategory.GetByIdAsync(MainCategoryId);
                    _context.Entry(oldmainCategory).CurrentValues.SetValues(mainCategory);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!mainCategoryMatrixExists(mainCategory.MainCategoryId))
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
            return View(mainCategory);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {            
            var mainCategory = await _unitOfWork.MainCategory.GetByIdAsync(id);
            _unitOfWork.MainCategory.Delete(mainCategory);

            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

            return RedirectToAction(nameof(Index));
        }


        private bool mainCategoryMatrixExists(int id)
        {
            return _context.MainCategory.Any(e => e.MainCategoryId == id);
        }

        /**************End MainCategory Section******************************************/
        #endregion



    }
}
