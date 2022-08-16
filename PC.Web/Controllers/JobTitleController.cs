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
    public class JobTitleController : BaseController
    {
        public JobTitleController(UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager,
           RoleManager<IdentityRole> roleManager,
           AppDBContext context,
           IConfiguration config,
           IUnitOfWork unitOfWork) : base(userManager, signInManager, roleManager, context, config, unitOfWork)
        {
        }

        #region Job Title 
        /**************JobTitle Section******************************************/

        [HttpGet]
        //[Authorize("ListRoles-AdminController")]
        public async Task<IActionResult> Index()
        {
            return View(await _unitOfWork.JobTitle.GetAllAsync());
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
        public async Task<IActionResult> Create(JobTitle jobTitle)
        {
            if (ModelState.IsValid)
            {
                var LoggedInuser = await userManager.GetUserAsync(User);
                jobTitle.CreatedBy = LoggedInuser;
                jobTitle.CreatedById = LoggedInuser.Id;
                jobTitle.CreatedDateTime = DateTime.Now;

                //_context.Add(jobTitle);
                await _unitOfWork.JobTitle.AddAsync(jobTitle);
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                return RedirectToAction(nameof(Index));
            }
            return View(jobTitle);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var jobTitle = await _context.jobTitle.FindAsync(id);
            var jobTitle = await _unitOfWork.JobTitle.GetByIdAsync(id);
            if (jobTitle == null)
            {
                return NotFound();
            }
            return View(jobTitle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int JobTitleId, JobTitle jobTitle)
        {
            if (JobTitleId != jobTitle.JobTitleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var LoggedInuser = await userManager.GetUserAsync(User);
                    jobTitle.UpdatedBy = LoggedInuser;
                    jobTitle.UpdatedById = LoggedInuser.Id;
                    jobTitle.UpdatedDateTime = DateTime.Now;

                    //var oldJobTitle = await _context.jobTitle.FindAsync(id);
                    //_context.Entry(oldJobTitle).CurrentValues.SetValues(jobTitle);
                    //await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    var oldJobTitle = await _unitOfWork.JobTitle.GetByIdAsync(JobTitleId);
                    _context.Entry(oldJobTitle).CurrentValues.SetValues(jobTitle);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobTitleExists(jobTitle.JobTitleId))
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
            return View(jobTitle);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            //var jobTitle = await _context.jobTitle.FindAsync(id);
            //_context.jobTitle.Remove(jobTitle);
            //await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
            var jobTitle = await _unitOfWork.JobTitle.GetByIdAsync(id);
            _unitOfWork.JobTitle.Delete(jobTitle);
            //_context.jobTitle.Remove(jobTitle);
            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

            return RedirectToAction(nameof(Index));
        }


        private bool JobTitleExists(int id)
        {
            //return await _unitOfWork.JobTitle.GetByIdAsync(id);
            return _context.jobTitle.Any(e => e.JobTitleId == id);
        }

        /**************End JobTitle Section******************************************/
        #endregion



    }
}
