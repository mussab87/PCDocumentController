using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PC.Services.Core;
using PC.Services.Core.Helper.Consts;
using PC.Services.Core.Interfaces;
using PC.Services.Core.Models;
using PC.Services.Core.Security;
using PC.Services.DL.DbContext;
using System.Security.Claims;

namespace PC.Web.Controllers
{
    //Audit when save add this await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
    [Authorize(Roles = SD.Admin)]
    public class DetailsController : BaseController
    {
        public DetailsController(UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager,
           RoleManager<IdentityRole> roleManager,
           AppDBContext context,
           IConfiguration config,
           IUnitOfWork unitOfWork,
           ISendEmail sendEmail) : base(userManager, signInManager, roleManager, context, config, unitOfWork, sendEmail)
        {
        }

        #region Details
        /**************Details Section******************************************/

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //var q = _context.Details.Include(a=> a.Activity).Include(b=>b.Activity.MainCategory);
            string[] includes = { "Activity", "Activity.MainCategory" };
            var allDetails = await _unitOfWork.Details.FindAllIncludeAsync(includes);

            return View(allDetails);
        }

        [HttpGet]
        public IActionResult Create()
        {
            GetActivityList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Details model)
        {
            if (ModelState.IsValid)
            {
                var LoggedInuser = await userManager.GetUserAsync(User);
                model.CreatedBy = LoggedInuser;
                model.CreatedById = LoggedInuser.Id;
                model.CreatedDateTime = DateTime.Now;

                model.Activity = await _unitOfWork.Activity.GetByIdAsync(model.ActivityId);
                await _unitOfWork.Details.AddAsync(model);
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

                TempData["Message"] = 1;
                return RedirectToAction(nameof(Index));
            }

            GetActivityList();
            TempData["Message"] = 5;
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var details = await _unitOfWork.Details.GetByIdAsync(id);
            details.Activity = await _unitOfWork.Activity.GetByIdAsync(details.ActivityId);
            details.Activity.MainCategory = await _unitOfWork.MainCategory.GetByIdAsync(details.Activity.MainCategoryId);

            if (details == null)
            {
                return NotFound();
            }

            GetActivityList();

            return View(details);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int detailsId, Details model)
        {
            if (detailsId != model.DetailsId)
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

                    //old details Id
                    var olddetails = await _unitOfWork.Details.GetByIdAsync(detailsId);

                    _context.Entry(olddetails).CurrentValues.SetValues(model);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

                    TempData["Message"] = 1;
                }
                catch (DbUpdateConcurrencyException)
                {
                    TempData["Message"] = 5;
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
            ViewData["Activity"] = new SelectList(_unitOfWork.Activity.GetAllAsync().Result.ToList(), "ActivityId", "Name");
            TempData["Message"] = 5;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var details = await _unitOfWork.Details.GetByIdAsync(id);
            _unitOfWork.Details.Delete(details);

            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
            TempData["Message"] = 1;
            return RedirectToAction(nameof(Index));
        }

        private void GetActivityList()
        {
            ViewData["Activity"] = new SelectList(_unitOfWork.Activity.GetAllAsync().Result.ToList(), "ActivityId", "Name");
        }
        private bool activityExists(int id)
        {
            return _context.Activity.Any(e => e.ActivityId == id);
        }

        /**************End Details Section******************************************/
        #endregion



    }
}
