using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class ActivityController : BaseController
    {
        public ActivityController(UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager,
           RoleManager<IdentityRole> roleManager,
           AppDBContext context,
           IConfiguration config,
           IUnitOfWork unitOfWork) : base(userManager, signInManager, roleManager, context, config, unitOfWork)
        {
        }

        #region Activity
        /**************Activity Section******************************************/

        [HttpGet]
        //[Authorize("ListRoles-AdminController")]
        public async Task<IActionResult> Index()
        {
            string[] includes = { "MainCategory" }; 
            var allActivity = await _unitOfWork.Activity.FindAllIncludeAsync(includes);
            return View(allActivity);
            //return View(await _context.jobTitle.ToListAsync());
        }

        //[AllowAnonymous]
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["MainCategory"] = new SelectList(_unitOfWork.MainCategory.GetAllAsync().Result.ToList(), "MainCategoryId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Activity model)
        {
            if (ModelState.IsValid)
            {
                var LoggedInuser = await userManager.GetUserAsync(User);
                model.CreatedBy = LoggedInuser;
                model.CreatedById = LoggedInuser.Id;
                model.CreatedDateTime = DateTime.Now;

                model.MainCategory = await _unitOfWork.MainCategory.GetByIdAsync(model.MainCategoryId);
                await _unitOfWork.Activity.AddAsync(model);
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                return RedirectToAction(nameof(Index));
            }
            ViewData["MainCategory"] = new SelectList(_unitOfWork.MainCategory.GetAllAsync().Result.ToList(), "MainCategoryId", "Name");
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activityCategory = await _unitOfWork.Activity.GetByIdAsync(id);
            activityCategory.MainCategory = await _unitOfWork.MainCategory.GetByIdAsync(activityCategory.MainCategoryId);
            if (activityCategory == null)
            {
                return NotFound();
            }
            ViewData["MainCategory"] = new SelectList(_unitOfWork.MainCategory.GetAllAsync().Result.ToList(), "MainCategoryId", "Name");
            return View(activityCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int ActivityId, Activity model)
        {
            if (ActivityId != model.ActivityId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var LoggedInuser = await userManager.GetUserAsync(User);
                    model.UpdatedBy = LoggedInuser;
                    model.UpdatedById = LoggedInuser.Id;
                    model.UpdatedDateTime = DateTime.Now;

                    //new maincategory Id
                    //model.MainCategory = await _unitOfWork.MainCategory.GetByIdAsync(model.MainCategory.MainCategoryId);

                    //old maincategory Id
                    var oldactivityCategory = await _unitOfWork.Activity.GetByIdAsync(ActivityId);
                    //oldactivityCategory.MainCategory = await _unitOfWork.MainCategory.GetByIdAsync(model.MainCategoryId);

                    _context.Entry(oldactivityCategory).CurrentValues.SetValues(model);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!activityExists(model.ActivityId))
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
            ViewData["MainCategory"] = new SelectList(_unitOfWork.MainCategory.GetAllAsync().Result.ToList(), "MainCategoryId", "Name");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {            
            var activity = await _unitOfWork.Activity.GetByIdAsync(id);
            _unitOfWork.Activity.Delete(activity);

            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

            return RedirectToAction(nameof(Index));
        }


        private bool activityExists(int id)
        {
            return _context.Activity.Any(e => e.ActivityId == id);
        }

        /**************End Activity Section******************************************/
        #endregion



    }
}
