using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PC.Services.Core;
using PC.Services.Core.Interfaces;
using PC.Services.Core.Models;
using PC.Services.Core.Security;
using PC.Services.DL.DbContext;
using PC.Services.DL.ViewModels;
using System.Security.Claims;

namespace PC.Web.Controllers
{

    public class AccountController : BaseController
    {
        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            AppDBContext context,
            IConfiguration config,
            IUnitOfWork unitOfWork,
           ISendEmail sendEmail) : base(userManager, signInManager, roleManager, context, config, unitOfWork, sendEmail)
        {
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? ReturnUrl)
        {
            if (!string.IsNullOrEmpty(ReturnUrl))
                ViewBag.url = ReturnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var selectedValue = Convert.ToBoolean(config["AppSetting:SecurityAnswer"]);
                if (selectedValue)
                {
                    string[] answerList = model.a.Split(" + ");
                    if (Convert.ToInt32(answerList[0]) + Convert.ToInt32(answerList[1]) != Convert.ToInt32(model.result))
                    {
                        TempData["Message"] = 12;
                        return View(model);
                    }
                }

                var result = await signInManager.PasswordSignInAsync(
                    model.Username, model.Password, false, false);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        //ModelState.AddModelError(string.Empty, "User Can't Login Please Contact IT Department");
                        //TempData["Message"] = 7;
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        //log every user login in db
                        var userName = model.Username;
                        ApplicationUser user = await userManager.FindByNameAsync(userName);

                        //check if user blocked
                        if (user.UserStatus == false)
                        {
                            TempData["Message"] = 8;
                            return View(model);
                        }

                        //check user has not logged in past 3 months if yes lock account
                        //when disable lock must put true in MonthLockStatus to allow user to login after lock
                        if (user.MonthLockStatus != true)
                        {
                            var rersult = await CheckLastUserLoginAsync(user);

                            if (rersult && userManager.FindByNameAsync(userName).Result.MaxMonthLock == true)
                            {
                                TempData["Message"] = 9;
                                return View(model);
                            }
                        }

                        await SaveLoginTransaction(user);

                        return RedirectToAction("Home", "Home");
                    }

                }

                //count login times
                var userNameagain = model.Username;
                ApplicationUser useragain = await userManager.FindByNameAsync(userNameagain);
                //var userFailed = await userManager.GetUserAsync(User);
                if (useragain != null)
                {
                    useragain.AccessFailedCount = useragain.AccessFailedCount + 1;
                    await userManager.UpdateAsync(useragain);
                }
                //in case try login more than 3 times
                var useragain2 = await userManager.FindByNameAsync(userNameagain);
                if (useragain2 != null && useragain2.AccessFailedCount > Convert.ToInt32(config["AppSetting:AccessFailedCount"]))
                {
                    useragain2.LockoutEnabled = true;
                    await userManager.UpdateAsync(useragain2);
                    TempData["Message"] = 8;

                    return View(model);
                }

                TempData["Message"] = 7;
            }

            return View(model);
        }

        private async Task SaveLoginTransaction(ApplicationUser user)
        {
            UserTransaction userLoggedIn = new UserTransaction();
            userLoggedIn.UserId = user.Id;
            userLoggedIn.LogginDateTime = DateTime.Now;

            await _context.UserTransaction.AddAsync(userLoggedIn);
            await _context.SaveChangesAsync(userLoggedIn.UserId);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            //log every user login in db
            var user = await userManager.GetUserAsync(User);

            UserTransaction userLoggedIn = new UserTransaction();
            userLoggedIn.UserId = user.Id;
            userLoggedIn.LoggedOutDateTime = DateTime.Now;

            await _context.UserTransaction.AddAsync(userLoggedIn);
            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

            await signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ChangePasswordAsync()
        {
            var user = await userManager.GetUserAsync(User);
            var LoggedUser = new ChangeUserPasswordViewModel();
            LoggedUser.username = user.UserName;
            return View(LoggedUser);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangeUserPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                // ChangePasswordAsync changes the user password
                var result = await userManager.ChangePasswordAsync(user,
                    model.CurrentPassword, model.NewPassword);

                // The new password did not meet the complexity rules or
                // the current password is incorrect. Add these errors to
                // the ModelState and rerender ChangePassword view
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }

                // Upon successfully changing the password refresh sign-in cookie
                await signInManager.RefreshSignInAsync(user);

                //Log the password values in 
                UserPassword userPassword = new UserPassword();
                userPassword.UserId = user.Id;
                userPassword.PasswordChange = DateTime.Now;
                userPassword.OldPassword = model.CurrentPassword;
                userPassword.NewPassword = model.NewPassword;

                await _context.UserPassword.AddAsync(userPassword);
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

                //update isFisrt sign in field in db
                user.FirstLogin = false;
                await userManager.UpdateAsync(user);

                TempData["Message"] = 1;
                return View(model);
            }

            return View(model);
        }

        public async Task<bool> CheckLastUserLoginAsync(ApplicationUser user)
        {
            try
            {
                var LoginTrs = _context.UserTransaction.Where(u => u.UserId == user.Id).ToList();
                //var count = LoginTrs.Count-1;
                if (LoginTrs.Count > 0)
                {
                    if (LoginTrs.Last().LogginDateTime != null)
                    {
                        var TotalAvg1 = DateTime.Now.Date - LoginTrs.Last().LogginDateTime.Value.Date;
                        if (TotalAvg1.TotalDays > Convert.ToInt32(config["AppSetting:MaxLimitToLockUser"]))
                        {
                            var useragain2 = await userManager.FindByNameAsync(user.UserName);

                            useragain2.MaxMonthLock = true;
                            await userManager.UpdateAsync(useragain2);
                            return true;
                        }
                        return false;
                    }
                    var TotalAvg = DateTime.Now.Date - LoginTrs.Last().LoggedOutDateTime.Value.Date;
                    if (TotalAvg.TotalDays > Convert.ToInt32(config["AppSetting:MaxLimitToLockUser"]))
                    {
                        var useragain2 = await userManager.FindByNameAsync(user.UserName);

                        useragain2.MaxMonthLock = true;
                        await userManager.UpdateAsync(useragain2);
                        return true;
                    }
                    return false;
                }
                return false;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
