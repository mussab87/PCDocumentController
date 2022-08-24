using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PC.Services.Core;
using PC.Services.Core.Interfaces;
using PC.Services.Core.Security;
using PC.Services.DL.DbContext;
using System.Net.Http.Headers;
using System.Text;

namespace PC.Web.Controllers
{
    public class BaseController : Controller
    {
        protected readonly UserManager<ApplicationUser> userManager;
        protected readonly SignInManager<ApplicationUser> signInManager;
        protected readonly RoleManager<IdentityRole> roleManager;
        protected readonly AppDBContext _context;
        protected readonly IConfiguration config;
        protected readonly ISendEmail _sendEmail;

        protected readonly IUnitOfWork _unitOfWork;

        public BaseController(UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager,
           RoleManager<IdentityRole> roleManager,
           AppDBContext context,
           IConfiguration config,
           IUnitOfWork unitOfWork,
           ISendEmail sendEmail)

        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            _context = context;
            this.config = config;
            _unitOfWork = unitOfWork;
            _sendEmail = sendEmail;
        }

        public BaseController() { }

    }
}
