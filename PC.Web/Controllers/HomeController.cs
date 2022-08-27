﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PC.Services.Core;
using PC.Services.Core.Interfaces;
using PC.Services.Core.Security;
using PC.Services.DL.DbContext;
using PC.Web.Models;
using System.Diagnostics;

namespace PC.Web.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        //private readonly ILogger<HomeController> _logger;

        public HomeController(UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager,
           RoleManager<IdentityRole> roleManager,
           AppDBContext context,
           IConfiguration config,
           IUnitOfWork unitOfWork,
           ISendEmail sendEmail) : base(userManager, signInManager, roleManager, context, config, unitOfWork, sendEmail)
        {
        }

        //[AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        //[Authorize]
        //[AllowAnonymous]
        public async Task<IActionResult> Home()
        {
            //check if first time for login redirect user to change password page before process anything on the system
            //var user = await userManager.GetUserAsync(User);
            //if (user.FirstLogin == null)
            //{
            //    ViewBag.changePassword = "true";
            //    return RedirectToAction("ChangePassword", "Account");
            //}

            //var getSetting = _Setting.GetAppSetting();
            //if (getSetting == null)
            //    return RedirectToAction("AppSetting", "Admin");
            //SessionHelper.SetObjectAsJson(HttpContext.Session, "settingObject", getSetting);
            //SessionHelper.GetObjectFromJson<Setting>(HttpContext.Session, "settingObject");
            //HttpContext.Session.se("setting", getSetting);

            //get user active requests using include
            //var LoggedInuser = await userManager.GetUserAsync(User);
            //var user = await userManager.Users.Include(x => x.UserMatrix)
            //            .ThenInclude(z => z.AuthorityMatrix)
            //            .ThenInclude(z => z.AuthorityMatrixCategoryHeader)
            //            .ThenInclude(z => z.CategoryHeader)
            //            .ThenInclude(z => z.MainCategory)
            //            .Where(u => u.Id == LoggedInuser.Id).ToListAsync();

            //throw new Exception("Error has been occured");
            ViewBag.users = _context.Users.Count();
            ViewBag.Approvals = _context.TrsDetails.Count();
            ViewBag.AuthorityMatrix = _context.AuthorityMatrix.Count();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}