using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PC.Services.Core;
using PC.Services.Core.EmailModel;
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

        public async Task<IActionResult> TestEmail()
        {
            try
            {
                //test send emnail
                string userEmail = null;
                //if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
                userEmail = "ahmed.mohamed@procare.com.sa"; //userManager.FindByIdAsync(userLevels.Level.ApplicationUserId).Result.Email;
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                    userEmail = "mussab87@gmail.com";// "ahmed.mohamed@procare.com.sa"; //"mussab87@gmail.com";

                //get user logged in url
                var Userurl = HttpContext.Request.GetEncodedUrl();
                var url = Userurl.Split("/").ToList();
                var sendUrl = url[0] + "//" + url[2];
                sendUrl = sendUrl + "test link";

                string messageBody = "<font><h2>New Authority Matrix Action has been added  : </h2></font><br />" +

                "<br />Click below link to navigate to the request: " +
                "<br /> " + sendUrl +
                "<br /><br /> <h1> ProCare Notification Email </h1>";

                //send email here
                EmailInfo emailInfo = new EmailInfo();
                emailInfo.From = config.GetValue<string>("AppSetting:FromEmail");
                emailInfo.SmtpCredentials = config.GetValue<string>("AppSetting:SmtpCredentials");
                emailInfo.To = userEmail;
                emailInfo.Subject = config.GetValue<string>("AppSetting:Subject");
                emailInfo.SmtpClient = config.GetValue<string>("AppSetting:SmtpClient");
                emailInfo.SmtpPort = config.GetValue<int>("AppSetting:SmtpPort");
                emailInfo.UseDefaultCredentials = config.GetValue<bool>("AppSetting:UseDefaultCredentials");
                emailInfo.EnableSsl = config.GetValue<bool>("AppSetting:EnableSsl");
                emailInfo.messageBody = messageBody;

                await _sendEmail.SendEmail(emailInfo);

                ViewBag.error = "success";
                return View();
                //******************************
            }
            catch (Exception ex)
            {
                ViewBag.error = "Error " + " " + ex.ToString();
                return View();
            }

        }
    }
}